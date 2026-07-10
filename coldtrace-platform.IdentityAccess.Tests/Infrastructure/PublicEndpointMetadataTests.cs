using System.Reflection;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ColdTrace.Platform.IdentityAccess.Tests.Infrastructure;

public class PublicEndpointMetadataTests
{
    [Fact]
    public void AllowAnonymous_IsLimitedToDocumentedPublicControllerActions()
    {
        var controllerAssembly = typeof(AuthenticationController).Assembly;
        var anonymousActions = controllerAssembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type))
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Select(method => new { Controller = type, Method = method }))
            .Where(action => action.Method.GetCustomAttributes<HttpMethodAttribute>().Any())
            .Where(action => HasAllowAnonymous(action.Controller, action.Method))
            .Select(action =>
            {
                var verbs = action.Method.GetCustomAttributes<HttpMethodAttribute>()
                    .SelectMany(attribute => attribute.HttpMethods);
                return $"{action.Controller.Name}.{action.Method.Name} {string.Join(',', verbs)}";
            })
            .Order(StringComparer.Ordinal)
            .ToArray();

        string[] expected =
        [
            "AuthenticationController.SignIn POST",
            "AuthenticationController.SocialOrganizationSignUp POST",
            "AuthenticationController.SocialProfilePreview POST",
            "AuthenticationController.SocialSignIn POST",
            "BillingStripeWebhooksController.ProcessStripeWebhook POST",
            "OrganizationSignUpsController.CreateOrganizationSignUp POST",
            "SubscriptionPlansController.GetSubscriptionPlans GET"
        ];

        Assert.Equal(expected, anonymousActions);
    }

    private static bool HasAllowAnonymous(Type controller, MethodInfo method) =>
        controller.GetCustomAttributes(inherit: true).OfType<IAllowAnonymous>().Any() ||
        method.GetCustomAttributes(inherit: true).OfType<IAllowAnonymous>().Any();
}

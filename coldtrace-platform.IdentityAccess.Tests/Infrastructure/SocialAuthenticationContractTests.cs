using ColdTrace.Platform.IdentityAccess.Application.Errors;
using ColdTrace.Platform.IdentityAccess.Application.Internal.CommandServices;
using ColdTrace.Platform.IdentityAccess.Application.Internal.OutboundServices.Social;
using ColdTrace.Platform.IdentityAccess.Application.Results;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Domain.Model.ValueObjects;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Resources;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.IdentityAccess.Tests.Infrastructure;

public class SocialAuthenticationContractTests
{
    [Theory]
    [InlineData("google", SocialProvider.Google)]
    [InlineData(" APPLE ", SocialProvider.Apple)]
    public void SocialProviderFromCode_ParsesSupportedProviders(string code, SocialProvider expected)
    {
        Assert.Equal(expected, SocialProviderExtensions.FromCode(code));
    }

    [Fact]
    public void SocialProviderFromCode_RejectsUnsupportedProvider()
    {
        var exception = Assert.Throws<ArgumentException>(() => SocialProviderExtensions.FromCode("github"));
        Assert.Equal("identity-access.authentication.error.provider.unsupported", exception.Message);
    }

    [Fact]
    public void SocialSignInCommand_RequiresIdTokenOrAuthorizationCode()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new SocialSignInCommand(SocialProvider.Google, null, " ", null, null));
        Assert.Equal("identity-access.authentication.error.social-token.required", exception.Message);
    }

    [Fact]
    public async Task ProfileService_ReturnsOpenSourceCompatibleProfileShape()
    {
        var command = new SocialSignInCommand(SocialProvider.Google, "provider-token", null, null, "nonce");
        var service = new SocialIdentityProfileCommandService(new StubExternalIdentityProviderService(
            new ProviderIdentity(
                SocialProvider.Google,
                "subject-1",
                "jane.smith@coldtrace.example",
                null,
                null,
                null,
                "verified-token")));

        var result = await service.Handle(command);

        var success = Assert.IsType<Result<SocialIdentityProfileResult, SocialAuthenticationError>.Success>(result);
        Assert.Equal("verified-token", success.Value.IdToken);
        Assert.Equal("jane.smith@coldtrace.example", success.Value.Email);
        Assert.Equal("Jane Smith", success.Value.FullName);
    }

    [Fact]
    public void SocialErrorAssembler_UsesStableOnboardingContract()
    {
        var result = new Result<AuthenticatedUserResult, SocialAuthenticationError>.Failure(
            SocialAuthenticationError.RequiresOnboarding());

        var action = ActionResultFromSocialAuthenticationResultAssembler.ToAuthenticatedUserActionResult(
            result,
            new TestController());

        var response = Assert.IsType<ObjectResult>(action);
        Assert.Equal(422, response.StatusCode);
        var resource = Assert.IsType<AuthenticationErrorResource>(response.Value);
        Assert.Equal("SOCIAL_IDENTITY_REQUIRES_ONBOARDING", resource.Code);
        Assert.Equal(
            "identity-access.authentication.error.social-identity-requires-onboarding",
            resource.Details);
    }

    [Fact]
    public void ExternalIdentity_UserRelationship_PreventsOrphanedProviderLinks()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySQL("server=localhost;database=coldtrace_model_test;user=root;password=root")
            .Options;
        using var context = new AppDbContext(options);

        var relationship = context.Model
            .FindEntityType(typeof(ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates.ExternalIdentity))!
            .GetForeignKeys()
            .Single(key => key.PrincipalEntityType.ClrType == typeof(ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates.User));

        Assert.Equal(DeleteBehavior.Restrict, relationship.DeleteBehavior);
        Assert.Equal("f_k_external_identities_users_user_id", relationship.GetConstraintName());
    }

    private sealed class StubExternalIdentityProviderService(ProviderIdentity identity)
        : IExternalIdentityProviderService
    {
        public Task<Result<ProviderIdentity, SocialAuthenticationError>> ValidateAsync(
            SocialSignInCommand command,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<Result<ProviderIdentity, SocialAuthenticationError>>(
                new Result<ProviderIdentity, SocialAuthenticationError>.Success(identity));
    }

    private sealed class TestController : ControllerBase
    {
    }
}

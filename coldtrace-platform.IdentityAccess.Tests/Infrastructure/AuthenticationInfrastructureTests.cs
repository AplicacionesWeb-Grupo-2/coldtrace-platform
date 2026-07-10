using System.Security.Claims;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Commands;
using ColdTrace.Platform.IdentityAccess.Infrastructure.Authorization.Claims;
using ColdTrace.Platform.IdentityAccess.Infrastructure.Hashing.BCrypt.Services;
using ColdTrace.Platform.IdentityAccess.Infrastructure.Tokens.Jwt.Configuration;
using ColdTrace.Platform.IdentityAccess.Infrastructure.Tokens.Jwt.Services;
using ColdTrace.Platform.IdentityAccess.Interfaces.REST.Transform;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ColdTrace.Platform.IdentityAccess.Tests.Infrastructure;

public class AuthenticationInfrastructureTests
{
    private const string JwtSecret = "ColdTraceTestJwtSecretKeyForHS256WithAtLeast32Bytes";

    [Fact]
    public void HashPassword_StoresOnlyVerifiableBcryptHash()
    {
        var service = new HashingService();

        var hash = service.HashPassword("ColdTrace123");

        Assert.NotEqual("ColdTrace123", hash);
        Assert.StartsWith("$2", hash, StringComparison.Ordinal);
        Assert.True(service.VerifyPassword("ColdTrace123", hash));
        Assert.False(service.VerifyPassword("Incorrect123", hash));
    }

    [Fact]
    public async Task GenerateToken_ReturnsHs256TokenWithUserIdentity()
    {
        var user = CreateUser(42);
        var service = CreateTokenService();

        var token = service.GenerateToken(user);
        var jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);

        Assert.Equal("HS256", jwt.Alg);
        Assert.Equal(user.Email, jwt.Subject);
        Assert.Equal("7", jwt.Claims.Single(claim => claim.Type == ColdTraceClaimTypes.OrganizationId).Value);
        Assert.Equal("3", jwt.Claims.Single(claim => claim.Type == ColdTraceClaimTypes.RoleId).Value);
        Assert.Equal("3", jwt.Claims.Single(claim => claim.Type == ClaimTypes.Role).Value);
        Assert.Equal(42, await service.ValidateToken(token));
    }

    [Fact]
    public async Task ValidateToken_WithTamperedSignature_ReturnsNull()
    {
        var service = CreateTokenService();
        var token = service.GenerateToken(CreateUser(42));
        var replacement = token[^1] == 'a' ? 'b' : 'a';
        var tamperedToken = token[..^1] + replacement;

        var userId = await service.ValidateToken(tamperedToken);

        Assert.Null(userId);
    }

    [Fact]
    public void SignInCommand_NormalizesEmailAndPreservesPassword()
    {
        var command = new SignInCommand(" Operator@ColdTrace.Test ", " ColdTrace123 ");

        Assert.Equal("operator@coldtrace.test", command.Email);
        Assert.Equal("ColdTrace123", command.Password);
    }

    [Fact]
    public void AuthenticatedUserAssembler_ReturnsOpenSourceCompatibleFlatContract()
    {
        var user = CreateUser(42);

        var resource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(user, "jwt-token");

        Assert.Equal(42, resource.Id);
        Assert.Equal("USR-42", resource.Uuid);
        Assert.Equal(42, resource.OrganizationUserId);
        Assert.Equal("operator@coldtrace.test", resource.Email);
        Assert.Equal(7, resource.OrganizationId);
        Assert.Equal(3, resource.RoleId);
        Assert.Equal("jwt-token", resource.Token);
    }

    private static TokenService CreateTokenService() =>
        new(Options.Create(new TokenSettings
        {
            Secret = JwtSecret,
            ExpirationDays = 7
        }));

    private static User CreateUser(int id)
    {
        var command = new CreateUserCommand(
            "Cold",
            "Operator",
            "operator@coldtrace.test",
            "ColdTrace123",
            7,
            3);
        var user = new User(command, new HashingService().HashPassword(command.Password));
        typeof(User).GetProperty(nameof(User.Id))!.SetValue(user, id);
        return user;
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SourceName.Test.Integration.Auth;

public static class TestingIdentity
{
    public static ClaimsIdentity GenerateClaimsIdentity() =>
        new(
        [
            new(JwtRegisteredClaimNames.Sub, "testing@gardnerwebtech.com"),
            new(JwtRegisteredClaimNames.NameId, "f080fbab-5ccc-4f79-aa15-c07959e1b1b5")
        ]);
}

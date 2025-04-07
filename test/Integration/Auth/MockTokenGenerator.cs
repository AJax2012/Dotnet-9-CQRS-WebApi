using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace SourceName.Test.Integration.Auth;

public static class MockTokenGenerator
{
    public static string GenerateJwtToken()
    {
        var key = Encoding.UTF8.GetBytes("4c0920653bf64d379378c5fbd5fbab071b145a95e5ad31ed1756cf52c8155662f58f3782165c47ff207d200d1c08a0ad5237b7f644bafe415902232ac365c8330c888712c67707266c115c62be7e7b11c184d946c697fc16617f4ff6306dd3adc5c88e6449834adb432666317d6961e5623c74ad70bc76871279dbf88ed11626399b06c204979a299061507f78520500a60a1545fc9afd735be55100d666a48ca40c0ceec045d2e5cdba1ef8fbf1c2f54de53a5679e5a9684785b458cc0c63c997f57a01b87798bdc228a2fa55a98b969b3dab47d3d319a11be6c81303af4e0c72a4ad592c7f3f74dc9fa31be8996537f520cb99275bbb69214fd9c38930219d");
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = signingCredentials,
            Audience = "https://localhost:7124",
            Issuer = "https://localhost:7124",
            Claims = new Dictionary<string, object>
            {
                {JwtRegisteredClaimNames.Sub, "testing@gardnerwebtech.com"},
                {JwtRegisteredClaimNames.NameId, "f080fbab-5ccc-4f79-aa15-c07959e1b1b5"}
            }
        };

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var token = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
        return jwtSecurityTokenHandler.WriteToken(token);
    }
}

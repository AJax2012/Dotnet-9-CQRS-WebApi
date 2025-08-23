using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SourceName.Api.Loaders.JwtAuth;

/// <summary>
/// Configuration service for JWT Bearer authentication.
/// </summary>
/// <param name="options"></param>
public class JwtTokenService(IOptions<BearerAuthorizationScheme> options)
{
    private readonly IOptions<BearerAuthorizationScheme> _options = options;

    /// <summary>
    /// Generates a JWT token, primarily for testing purposes.
    /// </summary>
    /// <param name="identity"><see cref="ClaimsIdentity"/></param>
    /// <returns></returns>
    public string? GenerateJwtToken(ClaimsIdentity identity)
    {
        var bearerSchema = GetTokenValidationParameters();
        var securityKey = GetSecurityKey();
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = identity,
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = signingCredentials,
            Audience = bearerSchema.ValidAudience,
            Issuer = bearerSchema.ValidIssuer
        };
        
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var token = jwtSecurityTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        return jwtSecurityTokenHandler.WriteToken(token);
    }
    
    internal TokenValidationParameters GetTokenValidationParameters() =>
        new()
        {
            ValidateIssuerSigningKey = true,
            ValidIssuer = _options.Value.ValidIssuer,
            ValidAudience = _options.Value.ValidAudiences.First(),
            IssuerSigningKey = GetSecurityKey(),
            TryAllIssuerSigningKeys = true
        };
    
    internal string GetSigningKey() => _options.Value.SigningKeys.First().Value;

    private SymmetricSecurityKey GetSecurityKey()
    {
        var signingKey = _options.Value.SigningKeys.FirstOrDefault();
        ArgumentNullException.ThrowIfNull(signingKey);
        
        return new(Encoding.UTF8.GetBytes(signingKey.Value))
        {
            KeyId = signingKey.Id
        };
    }
}

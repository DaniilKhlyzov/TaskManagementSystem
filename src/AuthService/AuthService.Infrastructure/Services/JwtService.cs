using System;
using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;

namespace AuthService.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;

        public JwtService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public string GenerateToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_settings.SecretKey);
            var handler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(_settings.ExpirationHours),
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }

        public Guid? ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new Exception("Token is empty");

            var handler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_settings.SecretKey);
            try
            {
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _settings.Audience,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                var jwt = (JwtSecurityToken)validatedToken;
                var sub = jwt.Claims.FirstOrDefault(c =>
                        c.Type == ClaimTypes.NameIdentifier ||
                        c.Type == JwtRegisteredClaimNames.Sub ||
                        c.Type == "nameid")
                    ?.Value;
                return Guid.TryParse(sub, out var id) ? id : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid token", ex);
            }
        }
    }
}



using AutoMapper.Execution;
using Microsoft.IdentityModel.Tokens;
using OnlineQRMenuApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OnlineQRMenuApp.Service
{
    public class TokenService
    {
        private readonly RSA _privateKey;
        private readonly RSA _publicKey;

        public TokenService()
        {
            _privateKey = KeyHelper.GetPrivateKey();
            _publicKey = KeyHelper.GetPublicKey();
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.UserType),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds().ToString())
            };

            var creds = new SigningCredentials(new RsaSecurityKey(_privateKey), SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: "RaiYugi",
                audience: "Saint",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "RaiYugi",
                    ValidAudience = "Saint",
                    IssuerSigningKey = new RsaSecurityKey(_publicKey)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    return principal;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xác thực token: {ex.Message}");
            }

            return null;
        }
    }
}

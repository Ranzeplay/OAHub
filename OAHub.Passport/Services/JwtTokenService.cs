using Microsoft.IdentityModel.Tokens;
using OAHub.Passport.Data;
using OAHub.Passport.Models.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OAHub.Passport.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly PassportDbContext _context;

        public JwtTokenService(PassportDbContext context)
        {
            _context = context;
        }

        public string GenerateToken(string appid, string appsecret, string forUserId)
        {
            JwtOptions _options = new JwtOptions();

            List<Claim> claims = new List<Claim>
            {
                new Claim("ForUser", forUserId),
                new Claim("GenerateTime", _options.NotBefore.ToString())
            };

            var jwtToken = new JwtSecurityToken(
                issuer: _options.DefaultIssuer,
                audience: appid,
                claims: claims,
                notBefore: _options.NotBefore,
                expires: _options.ExpireTime,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(appsecret)
                    ),
                    algorithm: SecurityAlgorithms.HmacSha256
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public JwtSecurityToken ValidateToken(string encodedToken, string appId)
        {
            JwtOptions _options = new JwtOptions();

            var app = _context.Apps.FirstOrDefault(x => x.AppId == appId);
            if (app != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                /*
                var validationParamaters = new TokenValidationParameters
                {
                    ValidateLifetime = false,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidIssuer = _options.DefaultIssuer,
                    ValidAudience = app.AppId,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(app.AppSecret)),
                };

                ClaimsPrincipal decodedData = tokenHandler.ValidateToken(encodedToken, validationParamaters, out SecurityToken validatedToken);
                */

                var decodedData = tokenHandler.ReadJwtToken(encodedToken);
                return decodedData;
            }

            return null;
        }
    }
}

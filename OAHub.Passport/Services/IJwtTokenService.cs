using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OAHub.Passport.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string appid, string appsecret, string forUserId);

        JwtSecurityToken ValidateToken(string encodedToken, string appId);
    }
}

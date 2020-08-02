using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Quizee.Utilities
{
    public static class JWTUtils
    {
        public static string GenerateJWT(
            IdentityUser user,
            IList<string> roles,
            string jwtKey,
            string jwtIssuer,
            string jwtId = null
            )
        {
            //Hash Security Key Object from the JWT Key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Generate list of claims with general and universally recommended claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            if(!string.IsNullOrEmpty(jwtId))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, jwtId));
            }

            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            //Generate final token adding Issuer and Subscriber data, claims, expriation time and Key
            var token = new JwtSecurityToken(
                jwtIssuer,
                jwtIssuer,
                claims,
                null,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials
            );

            //Return token string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

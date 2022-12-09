using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TaF_WebAPI.JwtToken
{
    public static class JWToken
    {
        public static string GenerateToken(string username, bool isAuthor)
        {
            var listOfclaims = new List<Claim>();
            listOfclaims.Add(new Claim(ClaimTypes.Name, username));
            listOfclaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            listOfclaims.Add(new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(
                                                                    DateTime.Now.AddHours(3)).ToUnixTimeSeconds().ToString()));

            if (isAuthor)
            {
                listOfclaims.Add(new Claim(ClaimTypes.Role, "Author"));         
            }
            else
            {
                listOfclaims.Add(new Claim(ClaimTypes.Role, "Reader"));
            }
            var claims = listOfclaims.ToArray();
            var token = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet, consectetur adipiscing elit")),
                                             SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims));

            var handler = new JwtSecurityTokenHandler();
            string tokenString = handler.WriteToken(token);
            return tokenString;
        }
    }
}

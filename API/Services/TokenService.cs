using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;
        public TokenService(IConfiguration config , UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public async Task<string> CreateToken(AppUser user)
        {
            // create claims for the token and add them to the list of claims 

            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()), // this is the user id 
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName) // this is the username
            };

            // get the roles of the user
            var roles = await _userManager.GetRolesAsync(user); 

            // add the roles to the claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role))); 

            // generate signing credentials 

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // describe how token is going to look like
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler(); // create token handler

            var token  = tokenHandler.CreateToken(tokenDescriptor); // create token

            return tokenHandler.WriteToken(token); // return token
        }    
    }
}
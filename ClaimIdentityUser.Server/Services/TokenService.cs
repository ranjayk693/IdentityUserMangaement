using ClaimIdentityUser.Server.Dtos;
using ClaimIdentityUser.Server.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClaimIdentityUser.Server.Services
{
    public class TokenService:ITokenService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        public TokenService(RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _roleManager = roleManager;
            _config = config;
        }
        
       

        public async Task<string> GenerateToken(ClaimsDto user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.username),
                new Claim(ClaimTypes.Name,user.username),
                new Claim(ClaimTypes.Role,user.role),
            };
            var roledata = new IdentityRole(user.role);
           
            await _roleManager.AddClaimAsync(roledata, new Claim(ClaimTypes.Role, user.role));

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}

using System;
using System.IdentityModel.Tokens.Jwt; //Install Through Nuget
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;//Install Through Nuget

namespace JwtTokenExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly string _signingKey = "S2pJALMZOhRLGhH5KxSmEM6hSwC3S2W6a1lpqUv5rR4="; // Replace with a strong secret key
        private readonly string _issuer = "Demo"; // Replace with your token issuer

        [HttpPost]
        public IActionResult CreateToken(string userId, string password)
        {
            // Simulated validation - Replace with actual user authentication logic
            if (userId == "niraj" && password == "niraj")
            {
                var token = GenerateToken(userId);
                return Ok(new { token });
            }

            return Unauthorized();
        }

        private string GenerateToken(string userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userId),
                new Claim(ClaimTypes.Role, "User") // You can add more claims as needed
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _issuer, // For simplicity, audience is the same as issuer
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token expiration time
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

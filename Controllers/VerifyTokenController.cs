using System;
using System.IdentityModel.Tokens.Jwt; //Install Through Nuget
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;//Install Through Nuget

namespace JwtTokenExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerifyTokenController : ControllerBase
    {
        private readonly string _signingKey = "S2pJALMZOhRLGhH5KxSmEM6hSwC3S2W6a1lpqUv5rR4="; // Replace with a strong secret key
        private readonly string _issuer = "Demo";

        [HttpPost]
        public IActionResult VerifyToken([FromBody] TokenRequestModel model)
        {
            string token = model.Token; // Pass the token in the request body

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireSignedTokens = true, 
                RequireExpirationTime = true,
                ValidIssuer = _issuer,
                ValidAudience = _issuer // You might need to adjust this based on your setup

            };

            SecurityToken validatedToken;
            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                // Convert claims to a dictionary for a more structured format
                var claimsDictionary = principal.Claims.ToDictionary(c => c.Type, c => c.Value);

                // Use JsonSerializerOptions with ReferenceHandler.Preserve to avoid circular reference issues
                var jsonOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true // Optionally, for better readability in the response
                };

                var serializedClaims = JsonSerializer.Serialize(claimsDictionary, jsonOptions);

                return Ok(new { Message = "Token is valid.", Claims = serializedClaims });
            }
            catch (SecurityTokenException ex)
            {
                return BadRequest(new { Message = $"Token validation failed: {ex.Message}" });
            }
        }
    }

    public class TokenRequestModel
    {
        public string Token { get; set; }
    }
}

using Identity_Auth.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtTokenHelper
{
	private readonly IConfiguration _configuration;

	public JwtTokenHelper(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string GenerateJwtToken(ApplicationUser user)
	{
		var jwtSettings = _configuration.GetSection("JwtSettings");
		var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);
		var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]);

		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id),
			new Claim(JwtRegisteredClaimNames.Email, user.Email),
			new Claim("FullName", $"{user.FirstName} {user.LastName}"),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())  // Unique identifier
        };

		var token = new JwtSecurityToken(
			issuer: jwtSettings["Issuer"],
			audience: jwtSettings["Audience"],
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
			signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256)
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}

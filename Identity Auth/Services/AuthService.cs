using Identity_Auth.DTOs;
using Identity_Auth.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public class AuthService : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager;

	public AuthService(UserManager<ApplicationUser> userManager)
	{
		_userManager = userManager;
	}

	public async Task<IdentityResult> RegisterAsync(RegisterDto model)
	{
		var user = new ApplicationUser
		{
			UserName = model.Email,
			Email = model.Email,
			FirstName = model.FirstName,
			LastName = model.LastName,
			DateOfBirth = model.DateOfBirth,
			Address = model.Address
		};

		return await _userManager.CreateAsync(user, model.Password);
	}
}

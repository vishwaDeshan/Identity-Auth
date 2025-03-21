using Identity_Auth.DTOs;
using Identity_Auth.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public class AuthService : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly JwtTokenHelper _jwtTokenHelper;

	public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JwtTokenHelper jwtTokenHelper)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_jwtTokenHelper = jwtTokenHelper;
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

	public async Task<string> LoginAsync(LoginDto model)
	{
		var user = await _userManager.FindByEmailAsync(model.Email);
		if (user == null)
			return null; 

		var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
		if (!result.Succeeded)
			return null;

		return _jwtTokenHelper.GenerateJwtToken(user);
	}
}

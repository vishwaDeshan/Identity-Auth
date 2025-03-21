using Identity_Auth.Models;
using Identity_Auth.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly UserManager<ApplicationUser> _userManager;

	public AuthController(UserManager<ApplicationUser> userManager)
	{
		_userManager = userManager;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterDto model)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var user = new ApplicationUser
		{
			UserName = model.Email,
			Email = model.Email,
			FirstName = model.FirstName,
			LastName = model.LastName,
			DateOfBirth = model.DateOfBirth,
			Address = model.Address
		};

		var result = await _userManager.CreateAsync(user, model.Password);
		if (!result.Succeeded)
			return BadRequest(result.Errors);

		return Ok("User registered successfully");
	}
}

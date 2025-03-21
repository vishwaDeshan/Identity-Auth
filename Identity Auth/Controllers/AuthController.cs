using Identity_Auth.DTOs;
using Microsoft.AspNetCore.Mvc;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;

	public AuthController(IAuthService authService)
	{
		_authService = authService;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterDto model)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await _authService.RegisterAsync(model);
		if (!result.Succeeded)
			return BadRequest(result.Errors);

		return Ok("User registered successfully");
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginDto model)
	{
		var token = await _authService.LoginAsync(model);
		if (token == null)
			return Unauthorized("Invalid credentials");

		return Ok(new { Token = token });
	}
}

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

		// Get the base URL dynamically
		var requestBaseUrl = $"{Request.Scheme}://{Request.Host}";

		var result = await _authService.RegisterAsync(model, requestBaseUrl);
		if (!result.Succeeded)
			return BadRequest(result.Errors);

		return Ok("User registered successfully. Please check your email to confirm your account.");
	}

	[HttpGet("confirm-email")]
	public async Task<IActionResult> ConfirmEmail(string userId, string token)
	{
		var result = await _authService.ConfirmEmailAsync(userId, token);
		if (!result)
			return BadRequest("Email confirmation failed.");

		return Ok("Email confirmed successfully!");
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginDto model)
	{
		var token = await _authService.LoginAsync(model);
		if (token == null)
			return Unauthorized("Invalid credentials or email not confirmed.");

		return Ok(new { Token = token });
	}

	[HttpPost("forgot-password")]
	public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
	{
		var result = await _authService.ForgotPasswordAsync(model.Email, Request.Scheme + "://" + Request.Host);
		if (!result)
			return BadRequest("Invalid email address");

		return Ok("Password reset link has been sent to your email");
	}

	[HttpPost("reset-password")]
	public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
	{
		var result = await _authService.ResetPasswordAsync(model);
		if (!result)
			return BadRequest("Invalid token or email");

		return Ok("Password reset successful");
	}

	[HttpPost("logout")]
	public async Task<IActionResult> Logout()
	{
		await _authService.SignOutAsync();
		return Ok("User logged out successfully");
	}

}

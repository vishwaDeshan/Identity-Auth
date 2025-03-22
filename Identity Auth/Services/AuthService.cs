using Identity_Auth.DTOs;
using Identity_Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

public class AuthService : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly JwtTokenHelper _jwtTokenHelper;
	private readonly IEmailService _emailService;

	public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JwtTokenHelper jwtTokenHelper, IEmailService emailService)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_jwtTokenHelper = jwtTokenHelper;
		_emailService = emailService;
	}

	public async Task<IdentityResult> RegisterAsync(RegisterDto model, string requestBaseUrl)
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

		var result =  await _userManager.CreateAsync(user, model.Password);

		// Generate email confirmation token
		var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

		// Construct confirmation URL
		var confirmationLink = $"{requestBaseUrl}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";

		// Send confirmation email
		await _emailService.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.");

		return result;

	}

	public async Task<bool> ConfirmEmailAsync(string userId, string token)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return false;

		token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
		var result = await _userManager.ConfirmEmailAsync(user, token);
		return result.Succeeded;
	}

	public async Task<string> LoginAsync(LoginDto model)
	{
		var user = await _userManager.FindByEmailAsync(model.Email);
		if (user == null)
			return null;

		// Check if the user has confirmed their email
		if (!await _userManager.IsEmailConfirmedAsync(user))
			throw new UnauthorizedAccessException("Email not confirmed. Please verify your email before logging in.");

		var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
		if (!result.Succeeded)
			return null;

		return _jwtTokenHelper.GenerateJwtToken(user);
	}

	public async Task<bool> ForgotPasswordAsync(string email, string requestBaseUrl)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user == null)
			return false;

		var token = await _userManager.GeneratePasswordResetTokenAsync(user);
		var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

		var resetLink = $"{requestBaseUrl}/api/auth/reset-password?email={email}&token={encodedToken}";

		await _emailService.SendEmailAsync(email, "Reset Password", $"Click here to reset your password: {resetLink}");

		return true;
	}

	public async Task<bool> ResetPasswordAsync(ResetPasswordDto model)
	{
		var user = await _userManager.FindByEmailAsync(model.Email);
		if (user == null)
			return false;

		var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));

		var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
		return result.Succeeded;
	}

	public async Task<bool> SignOutAsync()
	{
		await _signInManager.SignOutAsync();
		return true;
	}

}

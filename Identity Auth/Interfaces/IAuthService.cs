﻿using Identity_Auth.DTOs;
using Identity_Auth.Models;
using Microsoft.AspNetCore.Identity;

public interface IAuthService
{
	Task<IdentityResult> RegisterAsync(RegisterDto model, string requestBaseUrl);

	Task<bool> ConfirmEmailAsync(string userId, string token);

	Task<string> LoginAsync(LoginDto model);

	Task<bool> ForgotPasswordAsync(string email, string requestBaseUrl);

	Task<bool> ResetPasswordAsync(ResetPasswordDto model);

	Task<bool> SignOutAsync();
}

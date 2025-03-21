using Identity_Auth.DTOs;
using Identity_Auth.Models;
using Microsoft.AspNetCore.Identity;

public interface IAuthService
{
	Task<IdentityResult> RegisterAsync(RegisterDto model);
	Task<string> LoginAsync(LoginDto model);
}

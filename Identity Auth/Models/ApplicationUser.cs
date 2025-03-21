using Microsoft.AspNetCore.Identity;

namespace Identity_Auth.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public DateTime DateOfBirth { get; set; }
		public string Address { get; set; } = string.Empty;
	}
}

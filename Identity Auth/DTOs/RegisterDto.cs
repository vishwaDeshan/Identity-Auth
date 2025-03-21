using System.ComponentModel.DataAnnotations;

namespace Identity_Auth.DTOs
{
	public class RegisterDto
	{
			[Required] public string FirstName { get; set; }
			[Required] public string LastName { get; set; }
			[Required] public string Email { get; set; }
			[Required] public string Password { get; set; }
			[Required] public DateTime DateOfBirth { get; set; }
			public string Address { get; set; }
	}
}

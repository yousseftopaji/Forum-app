namespace DTOs.Models;

using System.ComponentModel.DataAnnotations;

public class CreateUserDTO
{
    [Required]
    [MinLength(3)]
    public required string Username { get; set; }
    //I want the password to be required, minimum length of 8 characters, and maximum length of 20 characters
    //it has to contain at least one uppercase letter, one lowercase letter, one digit and one special character
    [MinLength(8)]
    [Required]
    [MaxLength(20)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$")]
    public required string Password { get; set; }
}
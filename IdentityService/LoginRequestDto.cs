using System.ComponentModel.DataAnnotations;

namespace IdentityService.Models.DTOs;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

}
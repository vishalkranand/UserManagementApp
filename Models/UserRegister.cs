using System.ComponentModel.DataAnnotations;

public class UserRegister
{
    [Key]
    [Required]
    public string? Email { get; set; }

    [Required]
    public string? Password { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? PhoneNumber { get; set; }

    [Required]
    public string? Gender { get; set; }

    public DateTime UpdatedOn { get; set; } = DateTime.Now;
}

using System.ComponentModel.DataAnnotations;

public class UpdateUserDetails
{
    public string? Email { get; set; }

    public string? Name { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Gender { get; set; }

    public DateTime UpdatedOn { get; set; } = DateTime.Now;
}

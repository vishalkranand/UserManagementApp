using System.ComponentModel.DataAnnotations;

namespace FivD.Models
{
    public class UserLogin
    {
        [Key]
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}

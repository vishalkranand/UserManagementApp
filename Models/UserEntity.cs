using System.ComponentModel.DataAnnotations;

namespace FivD.Models
{
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }

     
        public string Email { get; set; }

     
        public byte[] PasswordHash { get; set; }

      
        public byte[] PasswordSalt { get; set; }

  
        public string Name { get; set; }

   
        public string PhoneNumber { get; set; }

      
        public string Gender { get; set; }

        public DateTime RegisteredOn { get; set; } = DateTime.Now;

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}

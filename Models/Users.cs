using System.ComponentModel.DataAnnotations;

namespace BattleShits.Models
{
    public class Users
    {
        [Key]
        [Required]
        public string Username { get; set; }

        [Required] 
        public string Password { get; set; }
    }
}

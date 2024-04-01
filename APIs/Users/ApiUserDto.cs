using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Users
{
    public class ApiUserDto : LoginDto
    {
        [Required]
        public string? PhoneNumber { get; set; }
    }
}

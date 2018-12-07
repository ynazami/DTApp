using System.ComponentModel.DataAnnotations;

namespace DTApp.API.DTO
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(8, MinimumLength=5, ErrorMessage="Password Minimum 5 and Maximum 8")]
        public string Password { get; set; }
    }
}
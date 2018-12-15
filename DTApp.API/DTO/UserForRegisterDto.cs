using System;
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
[Required]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }

        public UserForRegisterDto()
        {
            Created = DateTime.Now;            
            LastActive = DateTime.Now;
        }

    }
}
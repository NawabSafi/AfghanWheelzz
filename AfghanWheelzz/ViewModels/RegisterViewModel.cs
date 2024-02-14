using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AfghanWheelzz.ViewModels
{
    public class RegisterViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Plz Enter Email")]
        public string Email { get; set; } 
        public string PhoneNumber { get; set; } 
        public string Address { get; set; }
        [Required(ErrorMessage = "Plz Enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "ConfirmPassword")]
        [Required(ErrorMessage = "Plz Enter cofirmm password")]
        [Compare("Password",ErrorMessage ="Password Doesnt Matched")]
        public string ConfirmPassword { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? ProfilePicturePath { get; set; }
       
    }
}

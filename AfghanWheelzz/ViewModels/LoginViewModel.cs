using System.ComponentModel.DataAnnotations;

namespace AfghanWheelzz.ViewModels
{
    public class LoginViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Enter Email Address")]
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Plz Enter Password")]
        public string? Password { get; set; }
        [Display(Name ="RememberMe")]
        public bool RememberMe { get; set; }
    }
}

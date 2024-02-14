using Microsoft.AspNetCore.Identity;

namespace AfghanWheelzz.Models.UserModels
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? ProfilePicturePath { get; set; }
        public ICollection<Car>? Cars { get; set; }
    }
}

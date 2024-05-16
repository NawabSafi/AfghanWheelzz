using Microsoft.AspNetCore.Identity;

namespace AfghanWheelzz.Models.UserModels
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? ProfilePicturePath { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime DateCreated { get; set; }
        public int CarCount { get; set; }
        public ICollection<Car>? Cars { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; }
    }
}

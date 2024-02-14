using AfghanWheelzz.Models.UserModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfghanWheelzz.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public DateOnly? DatePublished { get; set; }
        public string Year { get; set; }
        public string Price { get; set; }
        public string Mileage { get; set; }
        public string description { get; set; }
        public string? ImagePath { get; set; }

        [NotMapped] // This property will not be mapped to the database
        public IFormFile File { get; set; }
        // Foreign key to represent the relationship with ApplicationUser
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? RegistrationId { get; set; }
        public Registration? Registration { get; set; }
        public int? LocationId { get; set; }
        public Location? Location { get; set; }
    }
}

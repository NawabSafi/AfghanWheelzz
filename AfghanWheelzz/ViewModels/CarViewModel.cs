namespace AfghanWheelzz.ViewModels
{
    public class CarViewModel
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public string Price { get; set; }
        public string Mileage { get; set; }
        public DateOnly? DatePublished { get; set; }
        public string Description { get; set; }
        public string? ImagePath { get; set; } // Path to the car's image
        public int? CategoryId { get; set; }
        public int? LocationId { get; set; }
        public int? RegistrationId { get; set; }
        public string? UserId { get; set; }

        // Form file property to handle image upload in the view
        public IFormFile File { get; set; }
    }
}

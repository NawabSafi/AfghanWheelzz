namespace AfghanWheelzz.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string City { get; set; }
        public ICollection<Car>? Cars { get; set; }
    }
}

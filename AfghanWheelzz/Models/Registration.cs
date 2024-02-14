namespace AfghanWheelzz.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public string RegistrationName { get; set; }
        public ICollection<Car>? Cars { get; set; }
    }
}

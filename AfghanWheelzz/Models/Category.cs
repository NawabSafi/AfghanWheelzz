namespace AfghanWheelzz.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Car>? Cars { get; set; }

    }
}

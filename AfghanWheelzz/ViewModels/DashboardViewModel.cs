namespace AfghanWheelzz.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalNewUsers { get; set; }
        public int TotalNewCars { get; set; }
        // Add more properties as needed for other statistics

        public List<dynamic>? CarStatistics { get; set; }
    }
}

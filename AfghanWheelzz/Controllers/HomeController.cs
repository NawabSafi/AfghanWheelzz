using AfghanWheelzz.Data;
using AfghanWheelzz.Models;
using AfghanWheelzz.Models.UserModels;
using AfghanWheelzz.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace AfghanWheelzz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICarRepository _carRepository;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICarRepository carRepository)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _carRepository = carRepository;

        }
/*
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetSellerDetails(int carId)
        {
            var car = await _carRepository.GetCarByIdAsync(carId);

            if (car == null || car.UserId == null)
            {
                return NotFound(); // Handle the case where car or seller is not found
            }

            var user = await _userManager.FindByIdAsync(car.UserId);

            if (user == null)
            {
                return NotFound(); // Handle the case where user is not found
            }

            // Return user details as JSON
            return Json(new { Name = $"{user.FirstName} {user.LastName}", user.Email, user.PhoneNumber });
        }
*/
     
        public async Task<IActionResult> Details(int id)
        {
            var car = await _carRepository.GetCarByIdAsync(id);
            var user = await _userManager.FindByIdAsync(car.UserId);
            var Location = _context.Locations.FirstOrDefault(x => x.Id == car.LocationId);
            var Registration = _context.Registrations.FirstOrDefault(x => x.Id == car.RegistrationId);
            var Category = _context.Categories.FirstOrDefault(x => x.Id == car.CategoryId);
            ViewBag.Category = Category;
            ViewBag.Registration = Registration;
            ViewBag.Location = Location;
            ViewBag.User= user;
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // Action to display all car makes
        public async Task<IActionResult> CarMakes()
        {
            var cars = await _carRepository.GetAllCarsAsync();
            var groupedCars = cars.GroupBy(c => c.Make);
            return View(groupedCars);
        }



        [HttpPost]
        public async Task<IActionResult> FilterCars(string filterCriteria)
        {
            var allCars = await _carRepository.GetAllCarsAsync();

            if (!string.IsNullOrEmpty(filterCriteria))
            {
                var filteredCars = allCars.Where(car => car.Make.Equals(filterCriteria, StringComparison.OrdinalIgnoreCase)).ToList();
                return View("Index", filteredCars);
            }

            return View("Index", allCars);
        }

        public IActionResult Contact() => View();

        public IActionResult About() => View();

      

        public IActionResult Index()
        {
            // Get all cars from the database, ordered by DatePublished in descending order
            var cars = _context.Cars.OrderByDescending(car => car.DatePublished).ToList();

            // Initialize ViewBag.Location and ViewBag.Category as dictionaries
            ViewBag.Location = new Dictionary<int, Location>();
            ViewBag.Category = new Dictionary<int, Category>();

            // Iterate through each car to get its associated location, registration, and category
            foreach (var car in cars)
            {
                // Get the associated location for the current car and store it in ViewBag
                ViewBag.Location[car.Id] = _context.Locations.FirstOrDefault(x => x.Id == car.LocationId);

                // Get the associated category for the current car and store it in ViewBag
                ViewBag.Category[car.Id] = _context.Categories.FirstOrDefault(x => x.Id == car.CategoryId);
            }

            // Pass the sorted list of cars to the view
            return View(cars);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

using AfghanWheelzz.Data;
using AfghanWheelzz.Models;
using AfghanWheelzz.Models.UserModels;
using AfghanWheelzz.Repository;
using AfghanWheelzz.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Security.Claims;
using X.PagedList;

namespace AfghanWheelzz.Controllers
{
    public class CarsController : Controller
    {
        private readonly ICarRepository _carRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarsController(ICarRepository carRepository, UserManager<ApplicationUser> userManager, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _carRepository = carRepository;
            _userManager = userManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Listing()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin"); // Adjust this based on how you identify admins

            IEnumerable<CarViewModel> cars;

            if (isAdmin)
            {
                cars = await _carRepository.GetAllCarsAsync();
            }
            else
            {
                cars = await _carRepository.GetCarsByUserIdAsync(userId);
            }

            return View(cars);
        }
        public async Task<IActionResult> Index(string searchTerm, string make, string city, string category, string minPrice, string maxPrice, int pageNumber = 1, int pageSize = 9)
        {
            // Get total count of cars
            var totalCount = _context.Cars.Count();

            // Start with an IQueryable to allow dynamic filtering
            var cars = _context.Cars.AsQueryable();

            // Apply filtering based on search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                cars = cars.Where(car =>
                    car.Make.ToLower().Contains(searchTerm.ToLower()) ||
                    car.Model.ToLower().Contains(searchTerm.ToLower())
                );
            }

            // Apply filtering based on make if provided
            if (!string.IsNullOrEmpty(make))
            {
                cars = cars.Where(car => car.Make.ToLower() == make.ToLower());
            }

            // Apply filtering based on city if provided
            if (!string.IsNullOrEmpty(city))
            {
                cars = cars.Where(car => car.Location.City.ToLower() == city.ToLower());
            }

            // Apply filtering based on category if provided
            if (!string.IsNullOrEmpty(category))
            {
                // Filter based on the CategoryName property in the Category table
                cars = cars.Where(car => car.Category.CategoryName.ToLower() == category.ToLower());
            }

            // Apply filtering based on price range if provided
            if (!string.IsNullOrEmpty(minPrice) && !string.IsNullOrEmpty(maxPrice))
            {
                // Convert minPrice and maxPrice to numeric values
                var minPriceValue = Convert.ToDecimal(minPrice);
                var maxPriceValue = Convert.ToDecimal(maxPrice);

                // Filter cars with prices falling within the specified range
                cars = cars.Where(car => Convert.ToDecimal(car.Price) >= minPriceValue && Convert.ToDecimal(car.Price) <= maxPriceValue);
            }

            // Apply ordering after filtering
            cars = cars.OrderBy(car => car.DatePublished);

            // Paginate the filtered or unfiltered list of cars
            var pagedCars = await cars.ToPagedListAsync(pageNumber, pageSize);

            // Initialize ViewBag.Location and ViewBag.Category as dictionaries
            ViewBag.Location = new Dictionary<int, Location>();
            ViewBag.Category = new Dictionary<int, Category>();

            // Iterate through each car to get its associated location and category
            foreach (var car in pagedCars)
            {
                // Get the associated location for the current car and store it in ViewBag
                ViewBag.Location[car.Id] = _context.Locations.FirstOrDefault(x => x.Id == car.LocationId);

                // Get the associated category for the current car and store it in ViewBag
                ViewBag.Category[car.Id] = _context.Categories.FirstOrDefault(x => x.Id == car.CategoryId);
            }

            // Pass necessary pagination information to the view using ViewBag
            ViewBag.PageNumber = pagedCars.PageNumber;
            ViewBag.PageSize = pagedCars.PageSize;
            ViewBag.TotalCount = pagedCars.TotalItemCount;
            ViewBag.TotalPages = pagedCars.PageCount;

            // Pass the paginated list of cars to the view
            return View(pagedCars);
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarViewModel carViewModel)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (ModelState.IsValid)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string uploadFolder = Path.Combine(wwwRootPath, "Images", "cars");

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    string fileName = Path.GetFileName(carViewModel.File.FileName);
                    string fullPath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await carViewModel.File.CopyToAsync(stream);
                    }

                    carViewModel.ImagePath = Path.Combine("Images", "cars", fileName);
                    var user = await _userManager.FindByIdAsync(userId);
                    user.CarCount+=1;
                    await _carRepository.AddCarAsync(carViewModel, userId);

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }
            
            return View(carViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Get the car by its ID
                var car = await _carRepository.GetCarByIdAsync(id);
                if (car == null)
                {
                    return NotFound(); // Car not found
                }

                // Delete the car from the repository
                await _carRepository.DeleteCarAsync(id);

                // Return a redirect to the Index action
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Handle any errors and return to the Index view with an error message
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(nameof(Index), await _carRepository.GetAllCarsAsync());
            }
        }
        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            var carViewModel = new CarViewModel
            {
                Id = car.Id,
                Make = car.Make,
                Model = car.Model,
                Year = car.Year,
                Price = car.Price,
                Mileage = car.Mileage,
                Description = car.description
            };

            return View(carViewModel);
        }

        // POST: Cars/Edit/5
        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Make,Model,Year,Price,Mileage,Description,File")] CarViewModel carViewModel)
        {
            if (id != carViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var car = await _context.Cars.FindAsync(id);
                    car.Make = carViewModel.Make;
                    car.Model = carViewModel.Model;
                    car.Year = carViewModel.Year;
                    car.Price = carViewModel.Price;
                    car.Mileage = carViewModel.Mileage;
                    car.description = carViewModel.Description;

                    // Check if a new file is provided
                    if (carViewModel.File != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "cars");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(carViewModel.File.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        car.ImagePath = Path.Combine("Images", "cars", uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await carViewModel.File.CopyToAsync(stream);
                        }
                    }
                    else
                    {
                        // If no new file is provided, keep the existing file path unchanged
                        carViewModel.ImagePath = car.ImagePath;
                    }

                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(carViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(carViewModel);
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }

    }
}

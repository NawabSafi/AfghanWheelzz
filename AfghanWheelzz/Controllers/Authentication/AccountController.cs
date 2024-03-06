using AfghanWheelzz.Data;
using AfghanWheelzz.Models.UserModels;
using AfghanWheelzz.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AfghanWheelzz.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        [Authorize] // Restrict access to authenticated users
        public async Task<IActionResult> Index()
        {
            // Get the current user
            ApplicationUser user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound(); // User not found
            }

            // Pass the user details to the view
            return View(user);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the email already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email Already Exists");
                    return View(model);
                }

                var newUser = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address
                };

                // Handle profile picture
                if (model.ProfilePicture != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string uploadFolder = Path.Combine(wwwRootPath, "Images", "ProfilePictures");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }
                    string fileName = Path.GetFileName(model.ProfilePicture.FileName);
                    string fullPath = Path.Combine(uploadFolder, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await model.ProfilePicture.CopyToAsync(stream);
                    }
                    newUser.ProfilePicturePath = Path.Combine("Images", "ProfilePictures", fileName);
                }
                else
                {
                    // Set default profile picture
                    newUser.ProfilePicturePath = "Images\\ProfilePictures\\Default.jpeg";
                }

                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email Not Found");
                    return View(model);
                }

                if (await _userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Credentials");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Update LastLogin property
                    user.LastLogin = DateTime.Now;
                    await _userManager.UpdateAsync(user);

                    return RedirectToAction("Index", "Home"); // Change the redirect action and controller as needed
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            // If we reach here, something went wrong, redisplay the form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Restrict access to authenticated users
        public async Task<IActionResult> UpdateProfile(ApplicationUser model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model); // Return the updated model to the view with validation errors
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound(); // User not found
            }

            // Update user details
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address; // Add this line if you also want to update the address

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Redirect back to the profile details page with the updated user
                return RedirectToAction("Index", user);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                // Return the updated model to the view with any errors
                return View("Index", model);
            }
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();

                var usersWithCarCounts = new List<ApplicationUser>();

                foreach (var user in users)
                {
                    var carCount = await _context.Cars.CountAsync(c => c.UserId == user.Id);
                    user.CarCount = carCount; // Add a CarCount property to ApplicationUser model
                    usersWithCarCounts.Add(user);
                }

                var carsByMake = await _context.Cars
                    .GroupBy(c => c.Make)
                    .Select(g => new { Make = g.Key, Count = g.Count() })
                    .ToListAsync();

                var carsByLocation = await _context.Locations
                    .Select(l => new
                    {
                        City = l.City,
                        CarCount = l.Cars.Count() // Count cars directly from the navigation property
                    })
                    .ToListAsync();

                var carsByRegistration = await _context.Registrations
                    .GroupBy(r => r.RegistrationName)
                    .Select(g => new { RegistrationType = g.Key, CarCount = g.Count() })
                    .ToListAsync();

                var carsByCategory = await _context.Categories
                    .Select(c => new
                    {
                        CategoryName = c.CategoryName,
                        CarCount = c.Cars.Count() // Count cars directly from the navigation property
                    })
                    .ToListAsync();

                if (carsByMake != null && carsByMake.Any() &&
                    carsByLocation != null && carsByLocation.Any() &&
                    carsByRegistration != null && carsByRegistration.Any() &&
                    carsByCategory != null && carsByCategory.Any())
                {
                    var chartDataByMake = carsByMake.Select(c => new object[] { c.Make, c.Count }).ToList();
                    var chartDataByLocation = carsByLocation.Select(c => new object[] { c.City, c.CarCount }).ToList();
                    var chartDataByRegistration = carsByRegistration.Select(c => new object[] { c.RegistrationType, c.CarCount }).ToList();
                    var chartDataByCategory = carsByCategory.Select(c => new object[] { c.CategoryName, c.CarCount }).ToList();

                    // Insert column headers as the first row
                    chartDataByMake.Insert(0, new object[] { "Make", "Count" });
                    chartDataByLocation.Insert(0, new object[] { "City", "Car Count" });
                    chartDataByRegistration.Insert(0, new object[] { "Registration Type", "Car Count" });
                    chartDataByCategory.Insert(0, new object[] { "Category Name", "Car Count" });

                    ViewBag.ChartDataByMake = chartDataByMake;
                    ViewBag.ChartDataByLocation = chartDataByLocation;
                    ViewBag.ChartDataByRegistration = chartDataByRegistration;
                    ViewBag.ChartDataByCategory = chartDataByCategory;
                }
                else
                {
                    ViewBag.ChartDataByMake = null; // Set ViewBag.ChartDataByMake to null if there are no cars available by make
                    ViewBag.ChartDataByLocation = null; // Set ViewBag.ChartDataByLocation to null if there are no cars available by location
                    ViewBag.ChartDataByRegistration = null; // Set ViewBag.ChartDataByRegistration to null if there are no cars available by registration
                    ViewBag.ChartDataByCategory = null; // Set ViewBag.ChartDataByCategory to null if there are no cars available by category
                }

                return View(usersWithCarCounts);
            }
            catch (Exception ex)
            {
                ViewBag.ChartDataByMake = null; // Set ViewBag.ChartDataByMake to null in case of an exception
                ViewBag.ChartDataByLocation = null; // Set ViewBag.ChartDataByLocation to null in case of an exception
                ViewBag.ChartDataByRegistration = null; // Set ViewBag.ChartDataByRegistration to null in case of an exception
                ViewBag.ChartDataByCategory = null; // Set ViewBag.ChartDataByCategory to null in case of an exception

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                // Find the user by Id
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    ViewBag.ErrorMessage = "User not found.";
                    return View("Dashboard");
                }

                // Delete the user
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed to delete user.";
                    return View("Dashboard");
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                ViewBag.ErrorMessage = "An error occurred while deleting the user.";
                return View("Dashboard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); // Change the redirect action and controller as needed
        }
    }
}

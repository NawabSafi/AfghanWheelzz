using AfghanWheelzz.Data;
using AfghanWheelzz.Models;
using AfghanWheelzz.Models.UserModels;
using AfghanWheelzz.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AfghanWheelzz.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _logger = logger;
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
                    Address = model.Address,
                    DateCreated = DateTime.Now
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
                    // Redirect to login page after successful registration
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            return View(model);
        }

     
        // Example of populating ExternalLogins in the controller action
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {

            LoginViewModel model = new()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
           

            return View(model);
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            
            var redirectUrl = Url.Action(action: "ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });

            // Configure the redirect URL, provider and other properties
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            //This will redirect the user to the external provider's login page
            return new ChallengeResult(provider, properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", loginViewModel);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("Login", loginViewModel);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                ModelState.AddModelError(string.Empty, "Email claim not received from external provider.");
                return View("Login", loginViewModel);
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null )
            {
               
                
                    // If the user exists, sign them in and redirect to the appropriate page
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                
            }
            else
            {
                // Create a new user with the information provided by the external provider
                var newUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                    LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                    // Additional fields if needed
                };

                // Save user data
                var result = await _userManager.CreateAsync(newUser);
                if (result.Succeeded)
                {
                    // Add a login (i.e., insert a row for the user in AspNetUserLogins table)
                    await _userManager.AddLoginAsync(newUser, info);

                    // Sign in the user
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("Login", loginViewModel);
                }
            }
        }

      [HttpPost]
[ValidateAntiForgeryToken]
[AllowAnonymous]
public async Task<IActionResult> Login(LoginViewModel model)
{
    model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

    if (ModelState.IsValid)
    {
        // Find user by email
        ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null)
        {
            /* if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                return View(model);
            }*/

            // Check if the user is locked out
            if (await _userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Account locked out. Please try again later.");
                return View(model);
            }

            // Check if the provided password is correct
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
             user.LastLogin = DateTime.Now;
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl); // Redirect to the returnUrl if it's provided and is a local URL
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // Redirect to a default action if returnUrl is null or not a local URL
                }
                
            }
            else
            {
                        // If login failed, increment access failed count
                        // If login failed, increment access failed count
                        await _userManager.AccessFailedAsync(user);

                        // Check if the user is locked out after the failed attempt
                        var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);
                        if (failedAttempts >= _userManager.Options.Lockout.MaxFailedAccessAttempts)
                        {
                            // Lock the user out
                            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.Add(_userManager.Options.Lockout.DefaultLockoutTimeSpan));
                            // Additional logic if needed
                        }

                        // Check if the user is locked out after the failed attempt
                        if (await _userManager.IsLockedOutAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Account locked out. Please try again later.");
                    return View(model);
                }
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }
    }

    // If we reach here, something went wrong, redisplay the form
    return View(model);
}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Restrict access to authenticated users
        public async Task<IActionResult> UpdateProfile(ApplicationUser model, IFormFile profilePicture)
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
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address; // Add this line if you also want to update the address

            // Handle profile picture
            if (profilePicture != null)
            {
                // Delete existing profile picture if any
                if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string filePath = Path.Combine(wwwRootPath, user.ProfilePicturePath.TrimStart('~', '/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Save new profile picture
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "ProfilePictures");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + profilePicture.FileName;
                string filePathToSave = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePathToSave, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                user.ProfilePicturePath = Path.Combine("Images", "ProfilePictures", uniqueFileName);
            }

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


        public async Task<IActionResult> Dashboard(string? email)
        {
            try
            {
                var usersWithCarCounts = new List<ApplicationUser>();

                var query = _userManager.Users.AsQueryable();

                if (!string.IsNullOrEmpty(email))
                {
                    query = query.Where(u => u.Email.Contains(email));
                }

                var users = await query.ToListAsync();

                DateTime today = DateTime.Now;
                var usersVisitedTodayCount = await _context.Users
                    .Where(u => u.LastLogin.Date == today)
                    .CountAsync();

                // Pass the count to the view
                ViewBag.UsersVisitedTodayCount = usersVisitedTodayCount;

                // Find the user by Id
                foreach (var user in users)
                {
                    var userCarCount = await _context.Cars.CountAsync(c => c.UserId == user.Id);
                    user.CarCount = userCarCount; // Add a CarCount property to ApplicationUser model
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

                var carsByRegistration = await _context.Cars
                  .GroupBy(c => c.Registration.RegistrationName != null ? c.Registration.RegistrationName : "Unregistered")
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
                ViewBag.UsersVisitedTodayCount = 0; // Set count to 0 in case of an exception

                ViewBag.ChartDataByMake = null; // Set ViewBag.ChartDataByMake to null in case of an exception
                ViewBag.ChartDataByLocation = null; // Set ViewBag.ChartDataByLocation to null in case of an exception
                ViewBag.ChartDataByRegistration = null; // Set ViewBag.ChartDataByRegistration to null in case of an exception
                ViewBag.ChartDataByCategory = null; // Set ViewBag.ChartDataByCategory to null in case of an exception

                return View();
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

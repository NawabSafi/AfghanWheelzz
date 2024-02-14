using System.Threading.Tasks;
using AfghanWheelzz.Models.UserModels;
using AfghanWheelzz.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace AfghanWheelzz.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
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
                ApplicationUser checkEmail= await _userManager.FindByNameAsync(model.Email);
                if (checkEmail == null)
                {
                    ModelState.AddModelError(string.Empty, "Email Not Found");
                    return View(model);
                }
                if(await _userManager.CheckPasswordAsync(checkEmail, model.Password)==false) {
                    ModelState.AddModelError(string.Empty, "Invalid Credntials");
                    return View(model);
                }
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home"); // Change the redirect action and controller as needed
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            // If we reach here, something went wrong, redisplay the form
            return View(model);
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

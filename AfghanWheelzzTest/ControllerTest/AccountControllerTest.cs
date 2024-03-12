using FakeItEasy;
using Microsoft.AspNetCore.Identity;
namespace AfghanWheelzzTest.ControllerTest
{
    public class AccountControllerTest
    {
        private readonly object _userManager;

        public AccountControllerTest() {

            _userManager = A.Fake<userManager>;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
    }

}

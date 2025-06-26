using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PsyNet.Web.Models.Domain;
using PsyNet.Web.Models.ViewModels;

namespace PsyNet.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                var existingUserByUsername = await userManager.FindByNameAsync(registerViewModel.Username);
                if (existingUserByUsername != null)
                {
                    ModelState.AddModelError("Username", "Username is already taken. Please choose a different one.");
                    return View(registerViewModel);
                }

                // Check if email already exists
                var existingUserByEmail = await userManager.FindByEmailAsync(registerViewModel.Email);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Email", "Email is already registered. Please use a different email or try logging in.");
                    return View(registerViewModel);
                }

                var identityUser = new ApplicationUser
                {
                    UserName = registerViewModel.Username,
                    Email = registerViewModel.Email
                };

                var identityResult = await userManager.CreateAsync(identityUser, registerViewModel.Password);

                if (identityResult.Succeeded)
                {
                    // assign this user the "User" role
                    var roleIdentityResult = await userManager.AddToRoleAsync(identityUser, "User");

                    if (roleIdentityResult.Succeeded)
                    {
                        // Show success notification
                        TempData["NotificationMessage"] = "Registration successful! You can now log in.";
                        TempData["NotificationType"] = "success";
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    // Add all the identity errors to the ModelState
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // Show error notification
            return View(registerViewModel);
        }

        [HttpGet]
        public IActionResult Login(string ReturnUrl)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = ReturnUrl
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var signInResult = await signInManager.PasswordSignInAsync(loginViewModel.Username,
                loginViewModel.Password, false, false);

            if (signInResult != null && signInResult.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(loginViewModel.ReturnUrl))
                {
                    return Redirect(loginViewModel.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            // Show error message for failed login
            ModelState.AddModelError(string.Empty, "User does not exist or incorrect password.");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

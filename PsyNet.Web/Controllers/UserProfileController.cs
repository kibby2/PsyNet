using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PsyNet.Web.Data;
using PsyNet.Web.Models.Domain;
using PsyNet.Web.Models.ViewModels;
using PsyNet.Web.Services;

namespace PsyNet.Web.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly PsyNetDbContext psyNetDbContext;
        private readonly IProfilePictureService profilePictureService;

        public UserProfileController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            PsyNetDbContext psyNetDbContext,
            IProfilePictureService profilePictureService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.psyNetDbContext = psyNetDbContext;
            this.profilePictureService = profilePictureService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }            // Get user statistics
            var totalBlogPosts = await psyNetDbContext.BlogPosts
                .Where(bp => bp.Author == user.UserName)
                .CountAsync();

            var totalPatients = await psyNetDbContext.Patients
                .Where(p => p.UserId == user.Id)
                .CountAsync();

            var viewModel = new UserProfileViewModel
            {
                Id = user.Id,
                Username = user.UserName ?? "",
                Email = user.Email ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePictureUrl,
                JoinDate = user.CreatedDate,
                TotalBlogPosts = totalBlogPosts,
                TotalPatients = totalPatients
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new EditUserProfileViewModel
            {
                Id = user.Id,
                Username = user.UserName ?? "",
                Email = user.Email ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Bio = user.Bio,
                CurrentProfilePictureUrl = user.ProfilePictureUrl
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Check if username is already taken by another user
            var existingUserWithUsername = await userManager.FindByNameAsync(model.Username);
            if (existingUserWithUsername != null && existingUserWithUsername.Id != user.Id)
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return View(model);
            }

            // Check if email is already taken by another user
            var existingUserWithEmail = await userManager.FindByEmailAsync(model.Email);
            if (existingUserWithEmail != null && existingUserWithEmail.Id != user.Id)
            {
                ModelState.AddModelError("Email", "Email is already taken.");
                return View(model);
            }

            // Handle profile picture upload
            string? newProfilePictureUrl = user.ProfilePictureUrl;
            if (model.ProfilePicture != null)
            {
                // Delete old profile picture if exists
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    await profilePictureService.DeleteProfilePictureAsync(user.ProfilePictureUrl);
                }

                // Upload new profile picture
                newProfilePictureUrl = await profilePictureService.UploadProfilePictureAsync(model.ProfilePicture, user.Id);
                if (newProfilePictureUrl == null)
                {
                    ModelState.AddModelError("ProfilePicture", "Failed to upload profile picture. Please ensure it's a valid image file under 5MB.");
                    return View(model);
                }
            }

            // Update user properties
            user.UserName = model.Username;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Bio = model.Bio;
            user.ProfilePictureUrl = newProfilePictureUrl;

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProfilePicture()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                await profilePictureService.DeleteProfilePictureAsync(user.ProfilePictureUrl);
                user.ProfilePictureUrl = null;
                await userManager.UpdateAsync(user);
                TempData["SuccessMessage"] = "Profile picture removed successfully!";
            }

            return RedirectToAction(nameof(Edit));
        }
    }
}

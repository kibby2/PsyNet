namespace PsyNet.Web.Services
{
    public class ProfilePictureService : IProfilePictureService
    {
        private readonly IWebHostEnvironment environment;
        private readonly string uploadsPath;

        public ProfilePictureService(IWebHostEnvironment environment)
        {
            this.environment = environment;
            this.uploadsPath = Path.Combine(environment.WebRootPath, "uploads", "profile-pictures");

            // Ensure the directory exists
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
        }

        public async Task<string?> UploadProfilePictureAsync(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
                return null;

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return null;

            // Validate file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return null;

            try
            {
                // Generate unique filename
                var fileName = $"{userId}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative URL
                return $"/uploads/profile-pictures/{fileName}";
            }
            catch
            {
                return null;
            }
        }
        public Task<bool> DeleteProfilePictureAsync(string profilePictureUrl)
        {
            if (string.IsNullOrEmpty(profilePictureUrl))
                return Task.FromResult(false);

            try
            {
                // Extract filename from URL
                var fileName = Path.GetFileName(profilePictureUrl);
                var filePath = Path.Combine(uploadsPath, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return Task.FromResult(true);
                }
            }
            catch
            {
                // Log error if needed
            }

            return Task.FromResult(false);
        }
    }
}

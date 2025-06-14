namespace PsyNet.Web.Services
{
    public interface IProfilePictureService
    {
        Task<string?> UploadProfilePictureAsync(IFormFile file, string userId);
        Task<bool> DeleteProfilePictureAsync(string profilePictureUrl);
    }
}

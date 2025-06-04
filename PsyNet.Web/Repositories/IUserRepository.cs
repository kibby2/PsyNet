using Microsoft.AspNetCore.Identity;

namespace PsyNet.Web.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<IdentityUser>> GetAll();
    }
}

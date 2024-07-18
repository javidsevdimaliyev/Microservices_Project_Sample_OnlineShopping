using IdentityService.Domain.Entities;

namespace IdentityService.Application.Interfaces
{
    public interface IAuthRepository : IGenericRepository<UserEntity>
    {
    }
}

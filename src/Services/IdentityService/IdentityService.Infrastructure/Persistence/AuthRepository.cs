using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using RepositoryWork;

namespace IdentityService.Infrastructure.Persistence
{
    public class AuthRepository : GenericRepository<UserEntity>, IAuthRepository
    {
        public AuthRepository(AuthDbContext context) : base(context)
        {
        }
    }
}

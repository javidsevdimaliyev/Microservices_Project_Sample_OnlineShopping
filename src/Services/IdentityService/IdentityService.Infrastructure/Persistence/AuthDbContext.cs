using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence
{
    public class AuthDbContext : IdentityDbContext<UserEntity, RoleEntity, int>
    {
        public const string DEFAULT_SCHEMA = "auth";
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        { }
    }
}

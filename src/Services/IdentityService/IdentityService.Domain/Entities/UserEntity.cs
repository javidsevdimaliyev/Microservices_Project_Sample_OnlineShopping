using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Domain.Entities
{
    public class UserEntity : IdentityUser<int>
    {

        [MaxLength(50)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Surname { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }

        public ICollection<UserRoleEntity> UserRoles { get; set; }

    }
}

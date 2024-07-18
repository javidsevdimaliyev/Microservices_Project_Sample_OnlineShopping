using IdentityService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginRequest model, CancellationToken cancellationToken);
        Task VerifyPasswordAsync(
            string password,
            UserDto currentUser,
            CancellationToken cancellationToken = default);
    }
}

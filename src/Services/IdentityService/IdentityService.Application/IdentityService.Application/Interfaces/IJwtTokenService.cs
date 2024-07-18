using IdentityService.Application.DTOs;
using IdentityService.Domain.Entities;
using System.Security.Claims;

namespace IdentityService.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(UserDto user);
    }

}

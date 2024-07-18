using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces;
using IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Utilities;


namespace IdentityService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IAuthRepository _repository;
        private readonly IJwtTokenService _tokenService;
        public AuthService(UserManager<UserEntity> userManager, IAuthRepository repository, IJwtTokenService tokenService)
        {
            _userManager = userManager;
            _repository = repository;
            _tokenService = tokenService;
        }
        public async Task<string> LoginAsync(LoginRequest model, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Username == model.UsernameOrEmail);
         
            if (user == null)
                throw new Exception("Domain User not found.");
         
            var userDto= user.Map<UserDto>();
            await VerifyPasswordAsync(user.PasswordHash, userDto);
            var token = _tokenService.GenerateToken(userDto);
            return token;
        }

        public async Task VerifyPasswordAsync(string password, UserDto currentUser, CancellationToken cancellationToken = default)
        {
            var hasher = new PasswordHasher<UserEntity>();
            var user = await _repository.GetByIdAsync(currentUser.Id);

            if((user == null || string.IsNullOrEmpty(user.PasswordHash)))
                throw new Exception("Domain User not found.");
                     
            var res = hasher.VerifyHashedPassword(
                user!,
                currentUser.PasswordHash,
                password);

            if(res != PasswordVerificationResult.Success)
                throw new Exception("Invalid Password.");
            
        }
    }
}

using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using TestBridge.DTOs;

namespace Services
{
    public class UserService
    {
        #region Fields and Dependencies

        private readonly IUserRepository _userRepository;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public UserService(IUserRepository userRepository, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        #endregion

        #region RegisterUserAsync

        public async Task<(IdentityResult, AppUser)> RegisterUserAsync(RegisterDTO userDto)
        {
            if (await _userRepository.GetByUsernameAsync(userDto.UserName) != null)
            {
                return (IdentityResult.Failed(new IdentityError { Description = "Username is already taken." }), null);
            }

            if (await _userRepository.GetByEmailAsync(userDto.Email) != null)
            {
                return (IdentityResult.Failed(new IdentityError { Description = "Email is already taken." }), null);
            }

            if (userDto.Password != userDto.ConfirmPassword)
            {
                return (IdentityResult.Failed(new IdentityError { Description = "Passwords do not match." }), null);
            }

            var appUser = new AppUser
            {
                UserName = userDto.UserName,
                Email = userDto.Email
            };

            var result = await _userRepository.CreateUserAsync(appUser, userDto.Password);

            return (result, appUser);
        }

        #endregion

        #region LoginUserAsync

        public async Task<SignInResult> LoginUserAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.userName);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            return await _signInManager.PasswordSignInAsync(user.UserName, loginDto.password, false, false);
        }

        #endregion
    }
}

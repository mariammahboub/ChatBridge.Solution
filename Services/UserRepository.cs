using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserRepository : IUserRepository
    {
        #region Constructor and Dependencies

        private readonly UserManager<AppUser> _userManager;

        public UserRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        #endregion

        #region GetByUsernameAsync

        public async Task<AppUser> GetByUsernameAsync(string username)
        {
            return await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == username);
        }

        #endregion

        #region GetByEmailAsync

        public async Task<AppUser> GetByEmailAsync(string email)
        {
            return await _userManager.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        #endregion

        #region CreateUserAsync

        public async Task<IdentityResult> CreateUserAsync(AppUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        #endregion
    }
}

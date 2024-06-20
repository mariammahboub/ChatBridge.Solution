using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repo.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class ProfileRepository : IProfileRepository
    {
        #region Constructor and Dependencies

        private readonly AppIdentityDbContext _context;

        public ProfileRepository(AppIdentityDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Get Profile by User ID

        public async Task<Profile?> GetByUserIdAsync(string userId)
        {
            return await _context.Profiles.FirstOrDefaultAsync(p => p.AppUserId == userId);
        }

        #endregion

        #region Get Profile by ID

        public async Task<Profile?> GetByIdAsync(int id)
        {
            return await _context.Profiles.FindAsync(id);
        }

        #endregion

        #region Add Profile

        public async Task<bool> AddAsync(Profile profile)
        {
            _context.Profiles.Add(profile);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion

        #region Update Profile

        public async Task<bool> UpdateAsync(Profile profile)
        {
            _context.Profiles.Update(profile);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        #endregion

        #region Delete Profile

        public async Task<bool> DeleteAsync(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return false;
            }

            _context.Profiles.Remove(profile);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion
    }
}

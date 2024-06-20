using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using Profile = Core.Entities.Profile;

namespace TestBridge.Controllers
{
    [Authorize]
    public class ProfileController : ApiBaceController
    {

        #region Parameters
        private readonly ILogger<ProfileController> _logger;
        private readonly IFileService _fileService;
        private readonly IProfileRepository _profileRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper; 
        #endregion

        #region Constructor
        public ProfileController(ILogger<ProfileController> logger,
                             IFileService fileService,
                             IProfileRepository profileRepository,
                             UserManager<AppUser> userManager,
                             IMapper mapper)
        {
            _logger = logger;
            _fileService = fileService;
            _profileRepository = profileRepository;
            _userManager = userManager;
            _mapper = mapper;
        } 
        #endregion

        #region HelperMethods

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private IActionResult HandleUnauthorized()
        {
            return Unauthorized(new ResponseDTOs
            {
                Status = "0",
                Message = "User is not authenticated."
            });
        }

        private IActionResult HandleProfileNotFound(int profileId)
        {
            return NotFound(new ResponseDTOs
            {
                Status = "0",
                Message = $"Profile with ID {profileId} not found."
            });
        }

        private IActionResult HandleError(string message)
        {
            return BadRequest(new ResponseDTOs
            {
                Status = "0",
                Message = message
            });
        } 
        #endregion

        #region AddProfile
        [HttpPost("AddProfile")]
        public async Task<IActionResult> AddProfile([FromForm] ProfileFormDataDto profileDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return HandleUnauthorized();
            var existingProfile = await _profileRepository.GetByUserIdAsync(userId);
            if (existingProfile != null)
                return HandleError("User already has a profile.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return HandleError("User not found.");
            var profile = _mapper.Map<Profile>(profileDto);
            profile.AppUserId = userId;

            if (profileDto.ImageFile != null)
            {
                var (status, filePath) = _fileService.SaveImage(profileDto.ImageFile);
                if (status == 1)
                    profile.ProfilePicture = filePath;
            }
            var added = await _profileRepository.AddAsync(profile);
            if (!added)
                return HandleError("Error when adding profile.");
            var responseDto = _mapper.Map<ResponseProfileDto>(profile);
            return Ok(new ResponseDTOs
            {
                Status = "1",
                Message = "Profile added successfully.",
                Data = responseDto
            });
        } 
        #endregion

        #region UpdateProfile
        [HttpPut("UpdateProfile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromForm] ProfileFormDataDto profileDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return HandleUnauthorized();

            var existingProfile = await _profileRepository.GetByIdAsync(id);
            if (existingProfile == null)
                return HandleProfileNotFound(id);

            if (existingProfile.AppUserId != userId)
                return HandleUnauthorized();

            _mapper.Map(profileDto, existingProfile);

            if (profileDto.ImageFile != null)
            {
                var (status, filePath) = _fileService.SaveImage(profileDto.ImageFile);
                if (status == 1)
                    existingProfile.ProfilePicture = filePath;
            }

            var updated = await _profileRepository.UpdateAsync(existingProfile);
            if (!updated)
                return HandleError("Error when updating profile.");

            var responseDto = _mapper.Map<ResponseProfileDto>(existingProfile);
            return Ok(new ResponseDTOs
            {
                Status = "1",
                Message = "Profile updated successfully.",
                Data = responseDto
            });
        } 
        #endregion

        #region DeleteProfile
        [HttpDelete("DeleteProfile/{id}")]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return HandleUnauthorized();

            var existingProfile = await _profileRepository.GetByIdAsync(id);
            if (existingProfile == null)
                return HandleProfileNotFound(id);

            if (existingProfile.AppUserId != userId)
                return HandleUnauthorized();

            var deleted = await _profileRepository.DeleteAsync(id);
            if (!deleted)
                return HandleError("Error when deleting profile.");

            return Ok(new ResponseDTOs
            {
                Status = "1",
                Message = "Profile deleted successfully."
            });
        } 
        #endregion

        #region GetProfileById
        [HttpGet("GetProfileById/{id}")]
        public async Task<IActionResult> GetProfileById(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return HandleUnauthorized();
            var profile = await _profileRepository.GetByIdAsync(id);
            if (profile == null)
                return HandleProfileNotFound(id);
            var responseDto = _mapper.Map<ResponseProfileDto>(profile);
            return Ok(new ResponseDTOs
            {
                Status = "1",
                Message = "Profile retrieved successfully.",
                Data = responseDto
            });
        }

        #endregion

    }
}

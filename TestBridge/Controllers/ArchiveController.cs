using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TestBridge.Controllers
{
    [Authorize]
    public class ArchiveController : ApiBaceController
    {
        #region Parameters

        private readonly IArchiveService _archiveService;
        private readonly IHttpContextAccessor _httpContextAccessor; 
        #endregion

        #region Constructor
        public ArchiveController(IArchiveService archiveService, IHttpContextAccessor httpContextAccessor)
        {
            _archiveService = archiveService;
            _httpContextAccessor = httpContextAccessor;
        } 
        #endregion

        #region archiveChat
        [HttpPost("archiveChat")]
        public async Task<IActionResult> ArchiveChat([FromBody] ArchiveChatDto archiveChatDto)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _archiveService.ArchiveChatAsync(archiveChatDto.ChatId);
                if (result)
                {
                    return Ok(new { Message = "Chat archived successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "Failed to archive chat and Chat is not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
        #endregion

        #region archiveGroup
        [HttpPost("archiveGroup")]
        public async Task<IActionResult> ArchiveGroup([FromBody] ArchiveGroupDto archiveGroupDto)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _archiveService.ArchiveGroupAsync(archiveGroupDto.GroupId);
                if (result)
                {
                    return Ok(new { Message = "Group archived successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "Failed to archive Group and Group is not found" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        } 
        #endregion
    }
}
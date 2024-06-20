using Core.DTOs;
using Core.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace TestBridge.Controllers
{
    [Authorize]
    public class BackupSettingsController : ApiBaceController
    {

        private readonly IOptions<BackupSettings> _backupSettings;

        public BackupSettingsController(IOptions<BackupSettings> backupSettings)
        {
            _backupSettings = backupSettings;
        }

        // GET: api/backupsettings
        [HttpGet]
        public IActionResult GetBackupSettings()
        {
            return Ok(_backupSettings.Value); // Return current backup settings
        }

        // PUT: api/backupsettings
        [HttpPut]
        public IActionResult UpdateBackupSettings(BackupSettingsDto backupSettingsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            // Update the backup interval based on user input
            switch (backupSettingsDto.Interval.ToLower())
            {
                case "none":
                    _backupSettings.Value.Interval = BackupInterval.None;
                    break;
                case "daily":
                    _backupSettings.Value.Interval = BackupInterval.Daily;
                    break;
                case "weekly":
                    _backupSettings.Value.Interval = BackupInterval.Weekly;
                    break;
                case "monthly":
                    _backupSettings.Value.Interval = BackupInterval.Monthly;
                    break;
                default:
                    return BadRequest("Invalid backup interval provided.");
            }

            // TODO: Save the updated backup settings to persistent storage (e.g., database)

            return Ok(new { Message = "Backup settings updated successfully." });
        }
   
    }
}

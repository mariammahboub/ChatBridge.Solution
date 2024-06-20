using Core.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace TestBridge.Controllers
{
    [Authorize]
    public class BackupController : ApiBaceController
    {

        #region Constructor and Dependencies
        private readonly IBackupService _backupService;
        private readonly ILogger<BackupController> _logger;

        public BackupController(IBackupService backupService, ILogger<BackupController> logger)
        {
            _backupService = backupService;
            _logger = logger;
        }

        #endregion

        #region scheduleBackup
        [HttpPost("scheduleBackup")]
        public IActionResult ScheduleBackup()
        {
            try
            {
                BackgroundJob.Enqueue<IBackupService>(service => service.BackupAllAsync());
                _logger.LogInformation("Scheduled backup job successfully.");
                return Ok(new { Message = "Backup job scheduled successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error scheduling backup job: {ex.Message}");
                return StatusCode(500, new { Message = "Failed to schedule backup job." });
            }
        } 
        #endregion
    }
}

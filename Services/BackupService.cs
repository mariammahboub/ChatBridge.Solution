using Core.Interfaces;
using Hangfire;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Repo.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Services
{
    public class BackupService : IBackupService
    {
        #region Constructor and Dependencies
        private readonly HangfireDbContext _dbContext;
        private readonly ILogger<BackupService> _logger;
        private readonly IConfiguration _configuration;

        public BackupService(HangfireDbContext dbContext, ILogger<BackupService> logger, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _configuration = configuration;
        } 
        #endregion

        #region BackupDatabaseAsync
        public async Task BackupDatabaseAsync()
        {
            var backupFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
            Directory.CreateDirectory(backupFolder);
            var backupFileName = $"backup_{DateTime.Now:yyyyMMddHHmmss}.bak";
            var backupFilePath = Path.Combine(backupFolder, backupFileName);

            try
            {
                _logger.LogInformation($"Starting backup process to {backupFilePath}...");
                await BackupDatabaseSqlAsync(backupFilePath);
                _logger.LogInformation("Backup process completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Backup process failed: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region BackupAllAsync
        public async Task BackupAllAsync()
        {
            string databaseName = _dbContext.Database.GetDbConnection().Database;
            string backupFileName = $"backup_{DateTime.Now:yyyyMMddHHmmss}.bak";
            string backupFilePath = Path.Combine(_configuration["BackupSettings:FolderPath"], backupFileName);

            await _dbContext.Database.ExecuteSqlRawAsync($"BACKUP DATABASE [{databaseName}] TO DISK = '{backupFilePath}'");
            Console.WriteLine($"Database backup completed: {backupFilePath}");
        }
        #endregion

        #region BackupDatabaseSqlAsync
        private async Task BackupDatabaseSqlAsync(string backupFilePath)
        {
            using var connection = new SqlConnection(_dbContext.Database.GetConnectionString());
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = $"BACKUP DATABASE [{_dbContext.Database.GetDbConnection().Database}] TO DISK = '{backupFilePath}'";
            await command.ExecuteNonQueryAsync();
        } 
        #endregion
    }
}

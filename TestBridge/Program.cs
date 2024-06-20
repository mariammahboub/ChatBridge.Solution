using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repo.Data;
using Repo.Repositories;
using Services;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using TestBridge.Helpers;

namespace TestBridge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TestBridge API", Version = "v1" });
            });

            builder.Services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
            builder.Services.AddDbContext<HangfireDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("HangfireConnection"));
            });
            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<IProfileRepository, ProfileRepository>(); // Corrected typo here

            var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfigurationDto>();
            builder.Services.AddSingleton(emailConfig);
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IFriendRequestService, FriendRequestService>();
            builder.Services.AddScoped<IFriendshipService, FriendshipService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IChatRepository, ChatRepository>();
            builder.Services.AddScoped<IGroupService, GroupService>();
            builder.Services.AddScoped<IArchiveService, ArchiveService>();
            builder.Services.AddScoped<IBackupService, BackupService>();

            builder.Services.AddLogging();

            // Register AutoMapper and specify the assembly containing mapping profiles
            builder.Services.AddAutoMapper(typeof(MappingProfiles)); // Register AutoMapper and specify the profile(s)
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // Updated to use the correct property
                });

            builder.Services.Configure<IdentityOptions>(opts =>
            {
                opts.SignIn.RequireConfirmedEmail = true;
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
                };
            });

            builder.Services.Configure<DataProtectionTokenProviderOptions>(opts =>
            {
                opts.TokenLifespan = TimeSpan.FromHours(10);
            });

            // Add Hangfire services
            builder.Services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"));
            });
            builder.Services.AddHangfireServer();

            var app = builder.Build();

            // Migration and other configurations
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<StoreDbContext>();
            var identityDbContext = services.GetRequiredService<AppIdentityDbContext>();
            var hangfireDbContext = services.GetRequiredService<HangfireDbContext>();

            try
            {
                dbContext.Database.Migrate();
                identityDbContext.Database.Migrate();
                hangfireDbContext.Database.Migrate();

            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration.");
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestBridge API v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard(); // Add Hangfire dashboard middleware
            app.UseHangfireServer(); // Add Hangfire server middleware
            app.MapControllers();
            ConfigureHangfireJobs(app);

            app.Run();
        }

        private static void ConfigureHangfireJobs(WebApplication app)
        {
            var configuration = app.Configuration;
            var backupInterval = configuration.GetValue<string>("BackupSettings:Interval") ?? "None";

            Console.WriteLine($"Backup interval from configuration: {backupInterval}");

            if (backupInterval != "None")
            {
                string cronExpression = GetCronExpressionForBackupInterval(backupInterval);
                var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

                // Schedule backup job
                recurringJobManager.AddOrUpdate<IBackupService>(
                    "backup-all",
                    service => service.BackupAllAsync(),
                    cronExpression);

                // Example of another recurring job (display information)
                recurringJobManager.AddOrUpdate(
                    "display-information",
                    () => Console.WriteLine($"Current time: {DateTime.Now}"),
                    Cron.MinuteInterval(1)); // Runs every minute
            }
            else
            {
                Console.WriteLine("Backup is disabled.");
            }
        }

        private static string GetCronExpressionForBackupInterval(string interval)
        {
            if (interval == null)
            {
                throw new ArgumentNullException(nameof(interval), "Backup interval cannot be null");
            }

            return interval.ToLower() switch
            {
                "daily" => Cron.Daily(),
                "weekly" => Cron.Weekly(),
                "monthly" => Cron.Monthly(),
                _ => throw new ArgumentException($"Invalid backup interval: {interval}")
            };
        }
    }
}

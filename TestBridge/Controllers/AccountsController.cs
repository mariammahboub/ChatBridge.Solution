
        #region Using
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestBridge.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Org.BouncyCastle.Asn1.Ocsp;

#endregion

namespace TestBridge.Controllers
{
    public class AccountsController : ApiBaceController
    {
        #region Parameters
        private readonly UserService _userService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        #endregion

        #region Constructor
        public AccountsController(UserService userService, SignInManager<AppUser> signInManager, IUserRepository userRepository, UserManager<AppUser> userManager, IConfiguration configuration, IEmailService emailService)
        {
            _userService = userService;
            _signInManager = signInManager;
            _userRepository = userRepository;
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }
        #endregion

        #region Register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (result, user) = await _userService.RegisterUserAsync(userDto);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmUrl = Url.Action(nameof(ConfirmEmail), "Accounts", new { token, email = userDto.Email }, Request.Scheme);

                var message = new MessageDto(new string[] { userDto.Email }, "Confirm your email", confirmUrl);
                await _emailService.SendEmailAsync(message);

                return Ok(new ResponseDTOs
                {
                    Status = "Success",
                    Message = $"User Created & Email Sent to {userDto.Email} Successfully",
                    Data=$"username: {user.UserName}"
                });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }
        #endregion

        #region ConfirmEmail
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid email confirmation request.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTOs
                {
                    Status = "Error",
                    Message = "This User does not exist!"
                });
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok(new ResponseDTOs
                {
                    Status = "Success",
                    Message = "Email Verified Successfully"
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTOs
            {
                Status = "Error",
                Message = "Email confirmation failed."
            });
        }
        #endregion

        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                // Return if the model state is not valid
                return BadRequest(ModelState);
            }

            // Attempt to retrieve the user by username
            var user = await _userRepository.GetByUsernameAsync(loginDto.userName);
            if (user == null)
            {
                // If the user does not exist, return an invalid login attempt
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return BadRequest(ModelState);
            }

            // Validate the user's password
            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginDto.password, false, false);
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

                // Retrieve and add user roles to the claims
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                // Get the JWT configuration settings
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Create the JWT token
                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(Convert.ToDouble(_configuration["JWT:DurationInDays"])),
                    signingCredentials: creds
                );

                // Return the token and expiration date
                var _token = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                };

                return Ok(_token);

        }

        #endregion

        #region ForgotPassword
        [HttpPost("forgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action("ResetPassword", "Accounts", new { token, email = user.Email }, Request.Scheme);
                var message = new MessageDto(new string[] { user.Email }, "Password Reset Link", link);
                await _emailService.SendEmailAsync(message);

                return Ok(new ResponseDTOs
                {
                    Status = "Success",
                    Message = $"Password reset request sent to {user.Email}. Please verify your email."
                });
            }

            return BadRequest(new { Message = "User not found." });
        }
        #endregion

        #region ResetPassword
        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var model = new ResetPasswordDtos
            {
                Token = token,
                Email = email
            };
            return Ok(new { model });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDtos resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user != null)
            {
                var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
                if (!resetPassResult.Succeeded)
                {
                    foreach (var error in resetPassResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return Ok(ModelState);
                }
                return Ok(new ResponseDTOs
                {
                    Status = "Success",
                    Message = "Password has been changed successfully."
                });
            }
            return BadRequest(new { Message = "User not found." });
        }
        #endregion

    }
}

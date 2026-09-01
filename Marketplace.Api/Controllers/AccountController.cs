using Marketplace.Application.DTOs.Auth;
using Marketplace.Application.DTOs.Users;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Domain.Entities;
using Marketplace.Infrastructure.Data;
using Marketplace.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AccountController> _logger;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
       

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IJwtService jwtService,
            ILogger<AccountController> logger,
            AppDbContext context,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _logger = logger;
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid Register Data"
                    });

                }

                var existUser = await _userManager.FindByEmailAsync(registerDto.Email);

                if(existUser != null)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Account already exist "
                    });

                }

                var user = new ApplicationUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if(!result.Succeeded)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Registration failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                if (!await _roleManager.RoleExistsAsync("Customer"))
                    await _roleManager.CreateAsync(new IdentityRole("Customer"));

                await _userManager.AddToRoleAsync(user, "Customer");

                // Create Business User 

                var businessUser = new User
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber ?? string.Empty,
                    Bio = registerDto.Bio,
                    IdentityUserId =user.Id,
                    IsActive =true,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Users.AddAsync(businessUser);
                await _context.SaveChangesAsync();

                user.BusinessUserId = businessUser.Id;
                await _userManager.UpdateAsync(user);

                var roles = await _userManager.GetRolesAsync(user);

                var token = _jwtService.GenerateToken(user, roles);

                await _emailService.SendWelcomeEmailAsync(registerDto.Email, registerDto.FirstName);

                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                await _emailService.SendEmailVerificationAsync(registerDto.Email,emailToken);

                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "Successful Registration",
                    Token = token,
                    TokenExpiry = DateTime.UtcNow.AddHours(1),
                    User = new UserDto
                    {
                        Id = businessUser.Id,
                        FirstName = businessUser.FirstName,
                        LastName = businessUser.LastName,
                        Email = businessUser.Email,
                        PhoneNumber = businessUser.PhoneNumber,
                        Bio = businessUser.Bio,
                        CreatedAt = businessUser.CreatedAt,
                        IsActive = businessUser.IsActive
                    }
                });

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error during registration ");

                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "An error eccurred during registration"
                });
            }
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if(! ModelState.IsValid)
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid login data"
                    });
                }

                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user == null)
                {
                    return Unauthorized(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    });
                }

                if(! user.IsActive)
                {
                    return Unauthorized(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Account is deactivated"
                    });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (!result.Succeeded)
                {
                    return Unauthorized(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    });
                }

                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                var businessUser =await _context.Users.FirstOrDefaultAsync(u => u.IdentityUserId == user.Id);

                var roles = await _userManager.GetRolesAsync(user);

                var token = _jwtService.GenerateToken(user, roles);

                return Ok(new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    TokenExpiry = DateTime.UtcNow.AddMinutes(60),
                    User = businessUser != null ? new UserDto
                    {
                        Id = businessUser.Id,
                        FirstName = businessUser.FirstName,
                        LastName = businessUser.LastName,
                        Email = businessUser.Email,
                        PhoneNumber = businessUser.PhoneNumber,
                        Bio = businessUser.Bio,
                        CreatedAt = businessUser.CreatedAt,
                        IsActive = businessUser.IsActive
                    } : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred during login"
                });
            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    return NotFound();

                var businessUser = await _context.Users.FirstOrDefaultAsync(u => u.IdentityUserId == userId);

                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    fullName = user.FullName,
                    profilePictureUrl = user.ProfilePictureUrl,
                    createdAt = user.CreatedAt,
                    lastLoginAt = user.LastLoginAt,
                    isActive = user.IsActive,
                    roles = roles,
                    businessUser = businessUser != null ? new UserDto
                    {
                        Id = businessUser.Id,
                        FirstName = businessUser.FirstName,
                        LastName = businessUser.LastName,
                        Email = businessUser.Email,
                        PhoneNumber = businessUser.PhoneNumber,
                        Bio = businessUser.Bio,
                        Address = businessUser.Address,
                        City = businessUser.City,
                        Country = businessUser.Country,
                        CreatedAt = businessUser.CreatedAt,
                        IsActive = businessUser.IsActive
                    } : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPost("change-password")]
        [Authorize]

        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto passwordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                var result = await _userManager.ChangePasswordAsync(user, passwordDto.CurrentPassword,passwordDto.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        success = false,
                        errors = result.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new { success = true, message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, "An error occurred");
            }
        }

    }
}

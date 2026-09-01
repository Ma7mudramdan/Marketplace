using Marketplace.Application.DTOs.Profile;
using Marketplace.Application.DTOs.Users;
using Marketplace.Application.Interfaces.Services;
using Marketplace.Infrastructure.Data;
using Marketplace.Web.Controllers.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : BaseApiController
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IFileUploadService _fileUploadService;
        public ProfileController(
            IUserProfileService userProfileService,
            IFileUploadService fileUploadService,
            ILogger<ProfileController> logger,
            AppDbContext context)
            : base(context,logger)
        {
            _userProfileService = userProfileService;
            _fileUploadService = fileUploadService;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var id = GetIdentityUserId();
                if (string.IsNullOrEmpty(id))
                    return Unauthorized();

                var profile = await _userProfileService.GetUserProfileAsync(id);

                return Ok(profile);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto updateDto)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var identityUserId = GetIdentityUserId();
                if (string.IsNullOrEmpty(identityUserId))
                {
                    return Unauthorized();
                }

                var result = await _userProfileService.UpdateUserProfileAsync(identityUserId, updateDto);

                if (!result)
                {
                    return NotFound("User not found");
                }

                return Ok(new { success = true, message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error updating profile");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPost("upload-picture")]
        public async Task<IActionResult> UploadPicture([FromForm] UploadPictureDto pictureDto)
        {
            try
            {
                if (pictureDto.File== null || pictureDto.File.Length == 0)
                    return BadRequest("No file uploaded");

                var id = GetIdentityUserId();

                if (string.IsNullOrEmpty(id))
                {
                    return Unauthorized();
                }

                if (!_fileUploadService.ValidateFile(pictureDto.File))
                    return BadRequest("Invalid file. Only JPG, PNG, GIF allowed. Max 5MB.");

                var imageUrl = await _userProfileService.UploadProfilePictureAsync(id, pictureDto.File);

                return Ok(new
                {
                    success = true,
                    message = "Profile picture uploaded successfully",
                    Url = imageUrl
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpDelete("picture")]

        public async Task<IActionResult> DeleteProfilePicture()
        {
            try
            {
                var userId = GetIdentityUserId();

                if (string.IsNullOrEmpty(userId))
                    return BadRequest("user not found");

                var result = await _userProfileService.DeleteProfilePictureAsync(identityUserId: userId);

                if (!result)
                    return NotFound("profile picture not found");

                return Ok(new {success = true , message ="Profile picture deleted"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile picture");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPut("Email")]

        public async Task<IActionResult> UpdateEmail(UpdateEmailDto emailDto)
        {

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetIdentityUserId();

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var result = await _userProfileService.UpdateEmailAsync(userId, emailDto.Email);

                if (!result)
                    return NotFound("Uer not found");

                return Ok(new { Message = "Email Updated successfully", Current_Eamil = emailDto.Email });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating email");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPut("Phone")]
        public async Task<IActionResult> UpdatePhone(UpdatePhoneDto phoneDto)
        {
            try
            {
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetIdentityUserId();

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var result = await _userProfileService.UpdatePhoneAsync(userId, phoneDto.PhoneNumber);

                if (!result)
                    return NotFound("user not found");

                return Ok(new {success = true, Message = "phone updated successfully"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating phone");
                return StatusCode(500, "An error occurred");
            }
        }
        




    }

}

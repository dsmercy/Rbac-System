using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RbacSystem.Models.Common;
using RbacSystem.Models.DTOs;
using RbacSystem.Services;

namespace RbacSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users with pagination and filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetUsers([FromQuery] UserFilterParams filterParams)
    {
        try
        {
            var result = await _userService.GetUsersAsync(filterParams);
            return Ok(ApiResponse<List<UserDto>>.SuccessResponse(result.Items, result.Metadata));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, ApiResponse<List<UserDto>>.ErrorResponse("An error occurred while retrieving users"));
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(long id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));

            return Ok(ApiResponse<UserDto>.SuccessResponse(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while retrieving the user"));
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            var user = await _userService.CreateUserAsync(createUserDto);
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId },
                ApiResponse<UserDto>.SuccessResponse(user, "User created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while creating the user"));
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(long id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await _userService.UpdateUserAsync(id, updateUserDto);
            if (user == null)
                return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));

            return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while updating the user"));
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(long id)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "User deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting the user"));
        }
    }

    /// <summary>
    /// Get user's effective permissions (direct + inherited from groups)
    /// </summary>
    [HttpGet("{id}/permissions")]
    public async Task<ActionResult<ApiResponse<UserEffectivePermissionsDto>>> GetUserPermissions(long id)
    {
        try
        {
            var permissions = await _userService.GetUserEffectivePermissionsAsync(id);
            if (permissions == null)
                return NotFound(ApiResponse<UserEffectivePermissionsDto>.ErrorResponse("User not found"));

            return Ok(ApiResponse<UserEffectivePermissionsDto>.SuccessResponse(permissions));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for user {UserId}", id);
            return StatusCode(500, ApiResponse<UserEffectivePermissionsDto>.ErrorResponse("An error occurred while retrieving user permissions"));
        }
    }
}
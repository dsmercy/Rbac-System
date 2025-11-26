using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RbacSystem.Models.Common;
using RbacSystem.Models.DTOs;
using RbacSystem.Services;

namespace RbacSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly ILogger<GroupsController> _logger;

    public GroupsController(IGroupService groupService, ILogger<GroupsController> logger)
    {
        _groupService = groupService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<GroupDto>>>> GetGroups([FromQuery] GroupFilterParams filterParams)
    {
        try
        {
            var result = await _groupService.GetGroupsAsync(filterParams);
            return Ok(ApiResponse<List<GroupDto>>.SuccessResponse(result.Items, result.Metadata));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving groups");
            return StatusCode(500, ApiResponse<List<GroupDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GroupDto>>> GetGroup(long id)
    {
        try
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound(ApiResponse<GroupDto>.ErrorResponse("Group not found"));

            return Ok(ApiResponse<GroupDto>.SuccessResponse(group));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving group {GroupId}", id);
            return StatusCode(500, ApiResponse<GroupDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GroupDto>>> CreateGroup([FromBody] CreateGroupDto createGroupDto)
    {
        try
        {
            var group = await _groupService.CreateGroupAsync(createGroupDto);
            return CreatedAtAction(nameof(GetGroup), new { id = group.GroupId },
                ApiResponse<GroupDto>.SuccessResponse(group, "Group created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating group");
            return StatusCode(500, ApiResponse<GroupDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<GroupDto>>> UpdateGroup(long id, [FromBody] UpdateGroupDto updateGroupDto)
    {
        try
        {
            var group = await _groupService.UpdateGroupAsync(id, updateGroupDto);
            if (group == null)
                return NotFound(ApiResponse<GroupDto>.ErrorResponse("Group not found"));

            return Ok(ApiResponse<GroupDto>.SuccessResponse(group, "Group updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating group {GroupId}", id);
            return StatusCode(500, ApiResponse<GroupDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteGroup(long id)
    {
        try
        {
            var result = await _groupService.DeleteGroupAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Group not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Group deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting group {GroupId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(IRoleService roleService, ILogger<RolesController> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<RoleDto>>>> GetRoles([FromQuery] RoleFilterParams filterParams)
    {
        try
        {
            var result = await _roleService.GetRolesAsync(filterParams);
            return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(result.Items, result.Metadata));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles");
            return StatusCode(500, ApiResponse<List<RoleDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> GetRole(long id)
    {
        try
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound(ApiResponse<RoleDto>.ErrorResponse("Role not found"));

            return Ok(ApiResponse<RoleDto>.SuccessResponse(role));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role {RoleId}", id);
            return StatusCode(500, ApiResponse<RoleDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole([FromBody] CreateRoleDto createRoleDto)
    {
        try
        {
            var role = await _roleService.CreateRoleAsync(createRoleDto);
            return CreatedAtAction(nameof(GetRole), new { id = role.RoleId },
                ApiResponse<RoleDto>.SuccessResponse(role, "Role created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            return StatusCode(500, ApiResponse<RoleDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> UpdateRole(long id, [FromBody] UpdateRoleDto updateRoleDto)
    {
        try
        {
            var role = await _roleService.UpdateRoleAsync(id, updateRoleDto);
            if (role == null)
                return NotFound(ApiResponse<RoleDto>.ErrorResponse("Role not found"));

            return Ok(ApiResponse<RoleDto>.SuccessResponse(role, "Role updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", id);
            return StatusCode(500, ApiResponse<RoleDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteRole(long id)
    {
        try
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Role not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Role deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(IPermissionService permissionService, ILogger<PermissionsController> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PermissionDto>>>> GetPermissions([FromQuery] PermissionFilterParams filterParams)
    {
        try
        {
            var result = await _permissionService.GetPermissionsAsync(filterParams);
            return Ok(ApiResponse<List<PermissionDto>>.SuccessResponse(result.Items, result.Metadata));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions");
            return StatusCode(500, ApiResponse<List<PermissionDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PermissionDto>>> GetPermission(long id)
    {
        try
        {
            var permission = await _permissionService.GetPermissionByIdAsync(id);
            if (permission == null)
                return NotFound(ApiResponse<PermissionDto>.ErrorResponse("Permission not found"));

            return Ok(ApiResponse<PermissionDto>.SuccessResponse(permission));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permission {PermissionId}", id);
            return StatusCode(500, ApiResponse<PermissionDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PermissionDto>>> CreatePermission([FromBody] CreatePermissionDto createPermissionDto)
    {
        try
        {
            var permission = await _permissionService.CreatePermissionAsync(createPermissionDto);
            return CreatedAtAction(nameof(GetPermission), new { id = permission.PermissionId },
                ApiResponse<PermissionDto>.SuccessResponse(permission, "Permission created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating permission");
            return StatusCode(500, ApiResponse<PermissionDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<PermissionDto>>> UpdatePermission(long id, [FromBody] UpdatePermissionDto updatePermissionDto)
    {
        try
        {
            var permission = await _permissionService.UpdatePermissionAsync(id, updatePermissionDto);
            if (permission == null)
                return NotFound(ApiResponse<PermissionDto>.ErrorResponse("Permission not found"));

            return Ok(ApiResponse<PermissionDto>.SuccessResponse(permission, "Permission updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permission {PermissionId}", id);
            return StatusCode(500, ApiResponse<PermissionDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeletePermission(long id)
    {
        try
        {
            var result = await _permissionService.DeletePermissionAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Permission not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Permission deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting permission {PermissionId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;
    private readonly ILogger<AssignmentsController> _logger;

    public AssignmentsController(IAssignmentService assignmentService, ILogger<AssignmentsController> logger)
    {
        _assignmentService = assignmentService;
        _logger = logger;
    }

    [HttpPost("user-role")]
    public async Task<ActionResult<ApiResponse<bool>>> AssignRoleToUser([FromBody] AssignRoleToUserDto dto)
    {
        try
        {
            var result = await _assignmentService.AssignRoleToUserAsync(dto);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Assignment already exists"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Role assigned to user successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role to user");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("user-role/{userId}/{roleId}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveRoleFromUser(long userId, long roleId)
    {
        try
        {
            var result = await _assignmentService.RemoveRoleFromUserAsync(userId, roleId);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Assignment not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Role removed from user successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role from user");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost("group-role")]
    public async Task<ActionResult<ApiResponse<bool>>> AssignRoleToGroup([FromBody] AssignRoleToGroupDto dto)
    {
        try
        {
            var result = await _assignmentService.AssignRoleToGroupAsync(dto);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Assignment already exists"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Role assigned to group successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role to group");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("group-role/{groupId}/{roleId}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveRoleFromGroup(long groupId, long roleId)
    {
        try
        {
            var result = await _assignmentService.RemoveRoleFromGroupAsync(groupId, roleId);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Assignment not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Role removed from group successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role from group");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost("user-group")]
    public async Task<ActionResult<ApiResponse<bool>>> AssignUserToGroup([FromBody] AssignUserToGroupDto dto)
    {
        try
        {
            var result = await _assignmentService.AssignUserToGroupAsync(dto);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Assignment already exists"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "User assigned to group successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning user to group");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("user-group/{userId}/{groupId}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveUserFromGroup(long userId, long groupId)
    {
        try
        {
            var result = await _assignmentService.RemoveUserFromGroupAsync(userId, groupId);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Assignment not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "User removed from group successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user from group");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost("role-permission")]
    public async Task<ActionResult<ApiResponse<bool>>> AssignPermissionToRole([FromBody] AssignPermissionToRoleDto dto)
    {
        try
        {
            var result = await _assignmentService.AssignPermissionToRoleAsync(dto);
            if (!result)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Assignment already exists"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Permission assigned to role successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permission to role");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("role-permission/{roleId}/{permissionId}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemovePermissionFromRole(long roleId, long permissionId)
    {
        try
        {
            var result = await _assignmentService.RemovePermissionFromRoleAsync(roleId, permissionId);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("Assignment not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Permission removed from role successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing permission from role");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    // Bulk operations
    [HttpPost("bulk/user-roles")]
    public async Task<ActionResult<ApiResponse<int>>> BulkAssignRolesToUser([FromBody] BulkAssignRolesToUserDto dto)
    {
        try
        {
            var count = await _assignmentService.BulkAssignRolesToUserAsync(dto);
            return Ok(ApiResponse<int>.SuccessResponse(count, $"{count} roles assigned successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk role assignment");
            return StatusCode(500, ApiResponse<int>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost("bulk/group-users")]
    public async Task<ActionResult<ApiResponse<int>>> BulkAssignUsersToGroup([FromBody] BulkAssignUsersToGroupDto dto)
    {
        try
        {
            var count = await _assignmentService.BulkAssignUsersToGroupAsync(dto);
            return Ok(ApiResponse<int>.SuccessResponse(count, $"{count} users assigned successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk user assignment");
            return StatusCode(500, ApiResponse<int>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost("bulk/role-permissions")]
    public async Task<ActionResult<ApiResponse<int>>> BulkAssignPermissionsToRole([FromBody] BulkAssignPermissionsToRoleDto dto)
    {
        try
        {
            var count = await _assignmentService.BulkAssignPermissionsToRoleAsync(dto);
            return Ok(ApiResponse<int>.SuccessResponse(count, $"{count} permissions assigned successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in bulk permission assignment");
            return StatusCode(500, ApiResponse<int>.ErrorResponse("An error occurred"));
        }
    }
}
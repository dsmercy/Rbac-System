namespace RbacSystem.Models.DTOs;

// User DTOs
public class UserDto
{
    public long UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUserDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}

public class UpdateUserDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public bool? IsActive { get; set; }
}

// Group DTOs
public class GroupDto
{
    public long GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateGroupDto
{
    public string GroupName { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdateGroupDto
{
    public string? GroupName { get; set; }
    public string? Description { get; set; }
}

// Role DTOs
public class RoleDto
{
    public long RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PermissionDto>? Permissions { get; set; }
}

public class CreateRoleDto
{
    public string RoleName { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdateRoleDto
{
    public string? RoleName { get; set; }
    public string? Description { get; set; }
}

// Permission DTOs
public class PermissionDto
{
    public long PermissionId { get; set; }
    public string PermissionName { get; set; } = null!;
    public string? Description { get; set; }
}

public class CreatePermissionDto
{
    public string PermissionName { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdatePermissionDto
{
    public string? PermissionName { get; set; }
    public string? Description { get; set; }
}

// Assignment DTOs
public class AssignRoleToUserDto
{
    public long UserId { get; set; }
    public long RoleId { get; set; }
}

public class AssignRoleToGroupDto
{
    public long GroupId { get; set; }
    public long RoleId { get; set; }
}

public class AssignUserToGroupDto
{
    public long UserId { get; set; }
    public long GroupId { get; set; }
}

public class AssignPermissionToRoleDto
{
    public long RoleId { get; set; }
    public long PermissionId { get; set; }
}

public class BulkAssignRolesToUserDto
{
    public long UserId { get; set; }
    public List<long> RoleIds { get; set; } = new();
}

public class BulkAssignUsersToGroupDto
{
    public long GroupId { get; set; }
    public List<long> UserIds { get; set; } = new();
}

public class BulkAssignPermissionsToRoleDto
{
    public long RoleId { get; set; }
    public List<long> PermissionIds { get; set; } = new();
}

// User effective permissions
public class UserEffectivePermissionsDto
{
    public long UserId { get; set; }
    public string Username { get; set; } = null!;
    public List<string> DirectPermissions { get; set; } = new();
    public List<string> GroupInheritedPermissions { get; set; } = new();
    public List<string> AllPermissions { get; set; } = new();
}
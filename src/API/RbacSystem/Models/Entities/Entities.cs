namespace RbacSystem.Models.Entities;

public class User
{
    public long UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

public class Group
{
    public long GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public ICollection<GroupRole> GroupRoles { get; set; } = new List<GroupRole>();
}

public class UserGroup
{
    public long UserGroupId { get; set; }
    public long UserId { get; set; }
    public long GroupId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public Group Group { get; set; } = null!;
}

public class Role
{
    public long RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<GroupRole> GroupRoles { get; set; } = new List<GroupRole>();
}

public class Permission
{
    public long PermissionId { get; set; }
    public string PermissionName { get; set; } = null!;
    public string? Description { get; set; }

    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class RolePermission
{
    public long RolePermissionId { get; set; }
    public long RoleId { get; set; }
    public long PermissionId { get; set; }

    // Navigation properties
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}

public class UserRole
{
    public long UserRoleId { get; set; }
    public long UserId { get; set; }
    public long RoleId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}

public class GroupRole
{
    public long GroupRoleId { get; set; }
    public long GroupId { get; set; }
    public long RoleId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Group Group { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
using Microsoft.EntityFrameworkCore;
using RbacSystem.Data;
using RbacSystem.Models.Entities;

namespace RbacSystem.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(RbacDbContext context)
    {
        // Check if data already exists
        if (await context.Users.AnyAsync())
        {
            return; // Database already seeded
        }

        // Seed Users
        var users = new List<User>
        {
            new User { Username = "admin", Email = "admin@example.com", IsActive = true },
            new User { Username = "john.doe", Email = "john.doe@example.com", IsActive = true },
            new User { Username = "jane.smith", Email = "jane.smith@example.com", IsActive = true },
            new User { Username = "bob.wilson", Email = "bob.wilson@example.com", IsActive = true },
            new User { Username = "alice.johnson", Email = "alice.johnson@example.com", IsActive = false }
        };
        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        // Seed Groups
        var groups = new List<Group>
        {
            new Group { GroupName = "Engineering", Description = "Engineering Department" },
            new Group { GroupName = "Sales", Description = "Sales Department" },
            new Group { GroupName = "HR", Description = "Human Resources Department" },
            new Group { GroupName = "Management", Description = "Management Team" }
        };
        context.Groups.AddRange(groups);
        await context.SaveChangesAsync();

        // Seed Permissions
        var permissions = new List<Permission>
        {
            new Permission { PermissionName = "user.read", Description = "Read user data" },
            new Permission { PermissionName = "user.write", Description = "Create and update users" },
            new Permission { PermissionName = "user.delete", Description = "Delete users" },
            new Permission { PermissionName = "group.read", Description = "Read group data" },
            new Permission { PermissionName = "group.write", Description = "Create and update groups" },
            new Permission { PermissionName = "group.delete", Description = "Delete groups" },
            new Permission { PermissionName = "role.read", Description = "Read role data" },
            new Permission { PermissionName = "role.write", Description = "Create and update roles" },
            new Permission { PermissionName = "role.delete", Description = "Delete roles" },
            new Permission { PermissionName = "permission.read", Description = "Read permission data" },
            new Permission { PermissionName = "permission.write", Description = "Create and update permissions" },
            new Permission { PermissionName = "permission.delete", Description = "Delete permissions" },
            new Permission { PermissionName = "report.view", Description = "View reports" },
            new Permission { PermissionName = "report.export", Description = "Export reports" },
            new Permission { PermissionName = "settings.manage", Description = "Manage system settings" }
        };
        context.Permissions.AddRange(permissions);
        await context.SaveChangesAsync();

        // Seed Roles
        var roles = new List<Role>
        {
            new Role { RoleName = "Super Admin", Description = "Full system access" },
            new Role { RoleName = "Admin", Description = "Administrative access" },
            new Role { RoleName = "Manager", Description = "Manager access" },
            new Role { RoleName = "User", Description = "Basic user access" },
            new Role { RoleName = "Viewer", Description = "Read-only access" }
        };
        context.Roles.AddRange(roles);
        await context.SaveChangesAsync();

        // Seed Role Permissions
        var rolePermissions = new List<RolePermission>
        {
            // Super Admin - all permissions
            new RolePermission { RoleId = 1, PermissionId = 1 },
            new RolePermission { RoleId = 1, PermissionId = 2 },
            new RolePermission { RoleId = 1, PermissionId = 3 },
            new RolePermission { RoleId = 1, PermissionId = 4 },
            new RolePermission { RoleId = 1, PermissionId = 5 },
            new RolePermission { RoleId = 1, PermissionId = 6 },
            new RolePermission { RoleId = 1, PermissionId = 7 },
            new RolePermission { RoleId = 1, PermissionId = 8 },
            new RolePermission { RoleId = 1, PermissionId = 9 },
            new RolePermission { RoleId = 1, PermissionId = 10 },
            new RolePermission { RoleId = 1, PermissionId = 11 },
            new RolePermission { RoleId = 1, PermissionId = 12 },
            new RolePermission { RoleId = 1, PermissionId = 13 },
            new RolePermission { RoleId = 1, PermissionId = 14 },
            new RolePermission { RoleId = 1, PermissionId = 15 },
            
            // Admin - most permissions
            new RolePermission { RoleId = 2, PermissionId = 1 },
            new RolePermission { RoleId = 2, PermissionId = 2 },
            new RolePermission { RoleId = 2, PermissionId = 4 },
            new RolePermission { RoleId = 2, PermissionId = 5 },
            new RolePermission { RoleId = 2, PermissionId = 7 },
            new RolePermission { RoleId = 2, PermissionId = 8 },
            new RolePermission { RoleId = 2, PermissionId = 13 },
            new RolePermission { RoleId = 2, PermissionId = 14 },
            
            // Manager
            new RolePermission { RoleId = 3, PermissionId = 1 },
            new RolePermission { RoleId = 3, PermissionId = 2 },
            new RolePermission { RoleId = 3, PermissionId = 4 },
            new RolePermission { RoleId = 3, PermissionId = 13 },
            new RolePermission { RoleId = 3, PermissionId = 14 },
            
            // User
            new RolePermission { RoleId = 4, PermissionId = 1 },
            new RolePermission { RoleId = 4, PermissionId = 4 },
            new RolePermission { RoleId = 4, PermissionId = 13 },
            
            // Viewer
            new RolePermission { RoleId = 5, PermissionId = 1 },
            new RolePermission { RoleId = 5, PermissionId = 4 },
            new RolePermission { RoleId = 5, PermissionId = 7 },
            new RolePermission { RoleId = 5, PermissionId = 10 }
        };
        context.RolePermissions.AddRange(rolePermissions);
        await context.SaveChangesAsync();

        // Seed User Roles
        var userRoles = new List<UserRole>
        {
            new UserRole { UserId = 1, RoleId = 1 }, // admin - Super Admin
            new UserRole { UserId = 2, RoleId = 2 }, // john.doe - Admin
            new UserRole { UserId = 3, RoleId = 3 }, // jane.smith - Manager
            new UserRole { UserId = 4, RoleId = 4 }, // bob.wilson - User
            new UserRole { UserId = 5, RoleId = 5 }  // alice.johnson - Viewer
        };
        context.UserRoles.AddRange(userRoles);
        await context.SaveChangesAsync();

        // Seed User Groups
        var userGroups = new List<UserGroup>
        {
            new UserGroup { UserId = 1, GroupId = 4 }, // admin - Management
            new UserGroup { UserId = 2, GroupId = 1 }, // john.doe - Engineering
            new UserGroup { UserId = 3, GroupId = 2 }, // jane.smith - Sales
            new UserGroup { UserId = 3, GroupId = 4 }, // jane.smith - Management
            new UserGroup { UserId = 4, GroupId = 1 }, // bob.wilson - Engineering
            new UserGroup { UserId = 5, GroupId = 3 }  // alice.johnson - HR
        };
        context.UserGroups.AddRange(userGroups);
        await context.SaveChangesAsync();

        // Seed Group Roles
        var groupRoles = new List<GroupRole>
        {
            new GroupRole { GroupId = 1, RoleId = 4 }, // Engineering - User
            new GroupRole { GroupId = 2, RoleId = 4 }, // Sales - User
            new GroupRole { GroupId = 3, RoleId = 5 }, // HR - Viewer
            new GroupRole { GroupId = 4, RoleId = 3 }  // Management - Manager
        };
        context.GroupRoles.AddRange(groupRoles);
        await context.SaveChangesAsync();

        Console.WriteLine("Sample data seeded successfully!");
    }
}
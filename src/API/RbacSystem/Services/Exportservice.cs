using Microsoft.EntityFrameworkCore;
using RbacSystem.Data;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace RbacSystem.Services;

public interface IExportService
{
    Task<byte[]> ExportToJsonAsync();
    Task<byte[]> ExportToCsvAsync();
    Task<string> ExportToHtmlAsync();
}

public class ExportService : IExportService
{
    private readonly RbacDbContext _context;
    private readonly ILogger<ExportService> _logger;

    public ExportService(RbacDbContext context, ILogger<ExportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<byte[]> ExportToJsonAsync()
    {
        _logger.LogInformation("Exporting all data to JSON");

        var exportData = new
        {
            ExportDate = DateTime.UtcNow,
            Version = "1.0",
            Data = new
            {
                Users = await _context.Users
                    .Select(u => new
                    {
                        u.UserId,
                        u.Username,
                        u.Email,
                        u.IsActive,
                        u.CreatedAt
                    })
                    .ToListAsync(),

                Groups = await _context.Groups
                    .Select(g => new
                    {
                        g.GroupId,
                        g.GroupName,
                        g.Description,
                        g.CreatedAt
                    })
                    .ToListAsync(),

                Roles = await _context.Roles
                    .Select(r => new
                    {
                        r.RoleId,
                        r.RoleName,
                        r.Description,
                        r.CreatedAt
                    })
                    .ToListAsync(),

                Permissions = await _context.Permissions
                    .Select(p => new
                    {
                        p.PermissionId,
                        p.PermissionName,
                        p.Description
                    })
                    .ToListAsync(),

                UserGroups = await _context.UserGroups
                    .Select(ug => new
                    {
                        ug.UserGroupId,
                        ug.UserId,
                        ug.GroupId,
                        ug.AssignedAt
                    })
                    .ToListAsync(),

                UserRoles = await _context.UserRoles
                    .Select(ur => new
                    {
                        ur.UserRoleId,
                        ur.UserId,
                        ur.RoleId,
                        ur.AssignedAt
                    })
                    .ToListAsync(),

                GroupRoles = await _context.GroupRoles
                    .Select(gr => new
                    {
                        gr.GroupRoleId,
                        gr.GroupId,
                        gr.RoleId,
                        gr.AssignedAt
                    })
                    .ToListAsync(),

                RolePermissions = await _context.RolePermissions
                    .Select(rp => new
                    {
                        rp.RolePermissionId,
                        rp.RoleId,
                        rp.PermissionId
                    })
                    .ToListAsync()
            }
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(exportData, options);
        return Encoding.UTF8.GetBytes(json);
    }

    public async Task<byte[]> ExportToCsvAsync()
    {
        _logger.LogInformation("Exporting all data to Excel with aggregated relationships");

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();

        // Sheet 1: Users
        var usersSheet = package.Workbook.Worksheets.Add("Users");
        usersSheet.Cells[1, 1].Value = "UserId";
        usersSheet.Cells[1, 2].Value = "Username";
        usersSheet.Cells[1, 3].Value = "Email";
        usersSheet.Cells[1, 4].Value = "IsActive";
        usersSheet.Cells[1, 5].Value = "CreatedAt";

        var users = await _context.Users.OrderBy(u => u.UserId).ToListAsync();
        for (int i = 0; i < users.Count; i++)
        {
            var user = users[i];
            usersSheet.Cells[i + 2, 1].Value = user.UserId;
            usersSheet.Cells[i + 2, 2].Value = user.Username;
            usersSheet.Cells[i + 2, 3].Value = user.Email;
            usersSheet.Cells[i + 2, 4].Value = user.IsActive ? "Yes" : "No";
            usersSheet.Cells[i + 2, 5].Value = user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        usersSheet.Cells[1, 1, 1, 5].Style.Font.Bold = true;
        usersSheet.Cells.AutoFitColumns();

        // Sheet 2: Groups
        var groupsSheet = package.Workbook.Worksheets.Add("Groups");
        groupsSheet.Cells[1, 1].Value = "GroupId";
        groupsSheet.Cells[1, 2].Value = "GroupName";
        groupsSheet.Cells[1, 3].Value = "Description";
        groupsSheet.Cells[1, 4].Value = "CreatedAt";

        var groups = await _context.Groups.OrderBy(g => g.GroupId).ToListAsync();
        for (int i = 0; i < groups.Count; i++)
        {
            var group = groups[i];
            groupsSheet.Cells[i + 2, 1].Value = group.GroupId;
            groupsSheet.Cells[i + 2, 2].Value = group.GroupName;
            groupsSheet.Cells[i + 2, 3].Value = group.Description ?? "";
            groupsSheet.Cells[i + 2, 4].Value = group.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        groupsSheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;
        groupsSheet.Cells.AutoFitColumns();

        // Sheet 3: Roles
        var rolesSheet = package.Workbook.Worksheets.Add("Roles");
        rolesSheet.Cells[1, 1].Value = "RoleId";
        rolesSheet.Cells[1, 2].Value = "RoleName";
        rolesSheet.Cells[1, 3].Value = "Description";
        rolesSheet.Cells[1, 4].Value = "CreatedAt";

        var roles = await _context.Roles.OrderBy(r => r.RoleId).ToListAsync();
        for (int i = 0; i < roles.Count; i++)
        {
            var role = roles[i];
            rolesSheet.Cells[i + 2, 1].Value = role.RoleId;
            rolesSheet.Cells[i + 2, 2].Value = role.RoleName;
            rolesSheet.Cells[i + 2, 3].Value = role.Description ?? "";
            rolesSheet.Cells[i + 2, 4].Value = role.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        rolesSheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;
        rolesSheet.Cells.AutoFitColumns();

        // Sheet 4: Permissions
        var permissionsSheet = package.Workbook.Worksheets.Add("Permissions");
        permissionsSheet.Cells[1, 1].Value = "PermissionId";
        permissionsSheet.Cells[1, 2].Value = "PermissionName";
        permissionsSheet.Cells[1, 3].Value = "Description";

        var permissions = await _context.Permissions.OrderBy(p => p.PermissionId).ToListAsync();
        for (int i = 0; i < permissions.Count; i++)
        {
            var permission = permissions[i];
            permissionsSheet.Cells[i + 2, 1].Value = permission.PermissionId;
            permissionsSheet.Cells[i + 2, 2].Value = permission.PermissionName;
            permissionsSheet.Cells[i + 2, 3].Value = permission.Description ?? "";
        }
        permissionsSheet.Cells[1, 1, 1, 3].Style.Font.Bold = true;
        permissionsSheet.Cells.AutoFitColumns();

        // Sheet 5: User-Groups (AGGREGATED)
        var userGroupsSheet = package.Workbook.Worksheets.Add("User-Groups");
        userGroupsSheet.Cells[1, 1].Value = "UserId";
        userGroupsSheet.Cells[1, 2].Value = "Username";
        userGroupsSheet.Cells[1, 3].Value = "GroupIds";
        userGroupsSheet.Cells[1, 4].Value = "GroupNames";

        var userGroupsData = await _context.UserGroups
            .Include(ug => ug.User)
            .Include(ug => ug.Group)
            .OrderBy(ug => ug.UserId)
            .ThenBy(ug => ug.GroupId)
            .ToListAsync();

        var userGroupsAggregated = userGroupsData
            .GroupBy(ug => new { ug.UserId, ug.User.Username })
            .Select(g => new
            {
                g.Key.UserId,
                g.Key.Username,
                GroupIds = string.Join(", ", g.Select(x => x.GroupId).OrderBy(id => id)),
                GroupNames = string.Join(", ", g.Select(x => x.Group.GroupName).OrderBy(name => name))
            })
            .OrderBy(x => x.UserId)
            .ToList();

        for (int i = 0; i < userGroupsAggregated.Count; i++)
        {
            var ug = userGroupsAggregated[i];
            userGroupsSheet.Cells[i + 2, 1].Value = ug.UserId;
            userGroupsSheet.Cells[i + 2, 2].Value = ug.Username;
            userGroupsSheet.Cells[i + 2, 3].Value = ug.GroupIds;
            userGroupsSheet.Cells[i + 2, 4].Value = ug.GroupNames;
        }
        userGroupsSheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;
        userGroupsSheet.Cells.AutoFitColumns();

        // Sheet 6: User-Roles (AGGREGATED)
        var userRolesSheet = package.Workbook.Worksheets.Add("User-Roles");
        userRolesSheet.Cells[1, 1].Value = "UserId";
        userRolesSheet.Cells[1, 2].Value = "Username";
        userRolesSheet.Cells[1, 3].Value = "RoleIds";
        userRolesSheet.Cells[1, 4].Value = "RoleNames";

        var userRolesData = await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .OrderBy(ur => ur.UserId)
            .ThenBy(ur => ur.RoleId)
            .ToListAsync();

        var userRolesAggregated = userRolesData
            .GroupBy(ur => new { ur.UserId, ur.User.Username })
            .Select(g => new
            {
                g.Key.UserId,
                g.Key.Username,
                RoleIds = string.Join(", ", g.Select(x => x.RoleId).OrderBy(id => id)),
                RoleNames = string.Join(", ", g.Select(x => x.Role.RoleName).OrderBy(name => name))
            })
            .OrderBy(x => x.UserId)
            .ToList();

        for (int i = 0; i < userRolesAggregated.Count; i++)
        {
            var ur = userRolesAggregated[i];
            userRolesSheet.Cells[i + 2, 1].Value = ur.UserId;
            userRolesSheet.Cells[i + 2, 2].Value = ur.Username;
            userRolesSheet.Cells[i + 2, 3].Value = ur.RoleIds;
            userRolesSheet.Cells[i + 2, 4].Value = ur.RoleNames;
        }
        userRolesSheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;
        userRolesSheet.Cells.AutoFitColumns();

        // Sheet 7: Group-Roles (AGGREGATED)
        var groupRolesSheet = package.Workbook.Worksheets.Add("Group-Roles");
        groupRolesSheet.Cells[1, 1].Value = "GroupId";
        groupRolesSheet.Cells[1, 2].Value = "GroupName";
        groupRolesSheet.Cells[1, 3].Value = "RoleIds";
        groupRolesSheet.Cells[1, 4].Value = "RoleNames";

        var groupRolesData = await _context.GroupRoles
            .Include(gr => gr.Group)
            .Include(gr => gr.Role)
            .OrderBy(gr => gr.GroupId)
            .ThenBy(gr => gr.RoleId)
            .ToListAsync();

        var groupRolesAggregated = groupRolesData
            .GroupBy(gr => new { gr.GroupId, gr.Group.GroupName })
            .Select(g => new
            {
                g.Key.GroupId,
                g.Key.GroupName,
                RoleIds = string.Join(", ", g.Select(x => x.RoleId).OrderBy(id => id)),
                RoleNames = string.Join(", ", g.Select(x => x.Role.RoleName).OrderBy(name => name))
            })
            .OrderBy(x => x.GroupId)
            .ToList();

        for (int i = 0; i < groupRolesAggregated.Count; i++)
        {
            var gr = groupRolesAggregated[i];
            groupRolesSheet.Cells[i + 2, 1].Value = gr.GroupId;
            groupRolesSheet.Cells[i + 2, 2].Value = gr.GroupName;
            groupRolesSheet.Cells[i + 2, 3].Value = gr.RoleIds;
            groupRolesSheet.Cells[i + 2, 4].Value = gr.RoleNames;
        }
        groupRolesSheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;
        groupRolesSheet.Cells.AutoFitColumns();

        // Sheet 8: Role-Permissions (AGGREGATED)
        var rolePermissionsSheet = package.Workbook.Worksheets.Add("Role-Permissions");
        rolePermissionsSheet.Cells[1, 1].Value = "RoleId";
        rolePermissionsSheet.Cells[1, 2].Value = "RoleName";
        rolePermissionsSheet.Cells[1, 3].Value = "PermissionIds";
        rolePermissionsSheet.Cells[1, 4].Value = "PermissionNames";

        var rolePermissionsData = await _context.RolePermissions
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .OrderBy(rp => rp.RoleId)
            .ThenBy(rp => rp.PermissionId)
            .ToListAsync();

        var rolePermissionsAggregated = rolePermissionsData
            .GroupBy(rp => new { rp.RoleId, rp.Role.RoleName })
            .Select(g => new
            {
                g.Key.RoleId,
                g.Key.RoleName,
                PermissionIds = string.Join(", ", g.Select(x => x.PermissionId).OrderBy(id => id)),
                PermissionNames = string.Join(", ", g.Select(x => x.Permission.PermissionName).OrderBy(name => name))
            })
            .OrderBy(x => x.RoleId)
            .ToList();

        for (int i = 0; i < rolePermissionsAggregated.Count; i++)
        {
            var rp = rolePermissionsAggregated[i];
            rolePermissionsSheet.Cells[i + 2, 1].Value = rp.RoleId;
            rolePermissionsSheet.Cells[i + 2, 2].Value = rp.RoleName;
            rolePermissionsSheet.Cells[i + 2, 3].Value = rp.PermissionIds;
            rolePermissionsSheet.Cells[i + 2, 4].Value = rp.PermissionNames;
        }
        rolePermissionsSheet.Cells[1, 1, 1, 4].Style.Font.Bold = true;
        rolePermissionsSheet.Cells.AutoFitColumns();

        return await package.GetAsByteArrayAsync();
    }



    public async Task<string> ExportToHtmlAsync()
    {
        _logger.LogInformation("Exporting all data to HTML");

        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang='en'>");
        html.AppendLine("<head>");
        html.AppendLine("    <meta charset='UTF-8'>");
        html.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        html.AppendLine("    <title>RBAC System Data Export</title>");
        html.AppendLine("    <style>");
        html.AppendLine("        body { font-family: Arial, sans-serif; margin: 20px; background: #f5f5f5; }");
        html.AppendLine("        .container { max-width: 1400px; margin: 0 auto; background: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
        html.AppendLine("        h1 { color: #333; border-bottom: 3px solid #007bff; padding-bottom: 10px; }");
        html.AppendLine("        h2 { color: #007bff; margin-top: 30px; border-bottom: 2px solid #dee2e6; padding-bottom: 8px; }");
        html.AppendLine("        table { width: 100%; border-collapse: collapse; margin-bottom: 30px; background: white; }");
        html.AppendLine("        th { background: #007bff; color: white; padding: 12px; text-align: left; font-weight: 600; }");
        html.AppendLine("        td { padding: 10px; border-bottom: 1px solid #dee2e6; }");
        html.AppendLine("        tr:hover { background: #f8f9fa; }");
        html.AppendLine("        .badge { padding: 4px 8px; border-radius: 4px; font-size: 12px; font-weight: 600; }");
        html.AppendLine("        .badge-success { background: #28a745; color: white; }");
        html.AppendLine("        .badge-danger { background: #dc3545; color: white; }");
        html.AppendLine("        .export-info { background: #e7f3ff; padding: 15px; border-radius: 5px; margin-bottom: 20px; border-left: 4px solid #007bff; }");
        html.AppendLine("        .stats { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px; margin-bottom: 30px; }");
        html.AppendLine("        .stat-card { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 8px; text-align: center; }");
        html.AppendLine("        .stat-card h3 { margin: 0; font-size: 36px; }");
        html.AppendLine("        .stat-card p { margin: 5px 0 0 0; opacity: 0.9; }");
        html.AppendLine("    </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        html.AppendLine("    <div class='container'>");
        html.AppendLine($"        <h1>RBAC System Data Export</h1>");
        html.AppendLine($"        <div class='export-info'>");
        html.AppendLine($"            <strong>Export Date:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC<br>");
        html.AppendLine($"            <strong>Export Type:</strong> Complete Database Snapshot");
        html.AppendLine($"        </div>");

        // Statistics
        var userCount = await _context.Users.CountAsync();
        var groupCount = await _context.Groups.CountAsync();
        var roleCount = await _context.Roles.CountAsync();
        var permissionCount = await _context.Permissions.CountAsync();

        html.AppendLine("        <div class='stats'>");
        html.AppendLine($"            <div class='stat-card'><h3>{userCount}</h3><p>Users</p></div>");
        html.AppendLine($"            <div class='stat-card'><h3>{groupCount}</h3><p>Groups</p></div>");
        html.AppendLine($"            <div class='stat-card'><h3>{roleCount}</h3><p>Roles</p></div>");
        html.AppendLine($"            <div class='stat-card'><h3>{permissionCount}</h3><p>Permissions</p></div>");
        html.AppendLine("        </div>");

        // Users Table
        html.AppendLine("        <h2>👥 Users</h2>");
        html.AppendLine("        <table>");
        html.AppendLine("            <thead><tr><th>ID</th><th>Username</th><th>Email</th><th>Status</th><th>Created At</th></tr></thead>");
        html.AppendLine("            <tbody>");
        var users = await _context.Users.OrderBy(u => u.UserId).ToListAsync();
        foreach (var user in users)
        {
            var badge = user.IsActive ? "badge-success" : "badge-danger";
            var status = user.IsActive ? "Active" : "Inactive";
            html.AppendLine($"                <tr><td>{user.UserId}</td><td><strong>{user.Username}</strong></td><td>{user.Email}</td><td><span class='badge {badge}'>{status}</span></td><td>{user.CreatedAt:yyyy-MM-dd HH:mm:ss}</td></tr>");
        }
        html.AppendLine("            </tbody>");
        html.AppendLine("        </table>");

        // Groups Table
        html.AppendLine("        <h2>👔 Groups</h2>");
        html.AppendLine("        <table>");
        html.AppendLine("            <thead><tr><th>ID</th><th>Group Name</th><th>Description</th><th>Created At</th></tr></thead>");
        html.AppendLine("            <tbody>");
        var groups = await _context.Groups.OrderBy(g => g.GroupId).ToListAsync();
        foreach (var group in groups)
        {
            html.AppendLine($"                <tr><td>{group.GroupId}</td><td><strong>{group.GroupName}</strong></td><td>{group.Description ?? "-"}</td><td>{group.CreatedAt:yyyy-MM-dd HH:mm:ss}</td></tr>");
        }
        html.AppendLine("            </tbody>");
        html.AppendLine("        </table>");

        // Roles Table
        html.AppendLine("        <h2>🎭 Roles</h2>");
        html.AppendLine("        <table>");
        html.AppendLine("            <thead><tr><th>ID</th><th>Role Name</th><th>Description</th><th>Created At</th></tr></thead>");
        html.AppendLine("            <tbody>");
        var roles = await _context.Roles.OrderBy(r => r.RoleId).ToListAsync();
        foreach (var role in roles)
        {
            html.AppendLine($"                <tr><td>{role.RoleId}</td><td><strong>{role.RoleName}</strong></td><td>{role.Description ?? "-"}</td><td>{role.CreatedAt:yyyy-MM-dd HH:mm:ss}</td></tr>");
        }
        html.AppendLine("            </tbody>");
        html.AppendLine("        </table>");

        // Permissions Table
        html.AppendLine("        <h2>🔑 Permissions</h2>");
        html.AppendLine("        <table>");
        html.AppendLine("            <thead><tr><th>ID</th><th>Permission Name</th><th>Description</th></tr></thead>");
        html.AppendLine("            <tbody>");
        var permissions = await _context.Permissions.OrderBy(p => p.PermissionId).ToListAsync();
        foreach (var permission in permissions)
        {
            html.AppendLine($"                <tr><td>{permission.PermissionId}</td><td><strong>{permission.PermissionName}</strong></td><td>{permission.Description ?? "-"}</td></tr>");
        }
        html.AppendLine("            </tbody>");
        html.AppendLine("        </table>");

        // User-Group Assignments
        html.AppendLine("        <h2>🔗 User-Group Assignments</h2>");
        html.AppendLine("        <table>");
        html.AppendLine("            <thead><tr><th>ID</th><th>User</th><th>Group</th><th>Assigned At</th></tr></thead>");
        html.AppendLine("            <tbody>");
        var userGroups = await _context.UserGroups
            .Include(ug => ug.User)
            .Include(ug => ug.Group)
            .OrderBy(ug => ug.UserGroupId)
            .ToListAsync();
        foreach (var ug in userGroups)
        {
            html.AppendLine($"                <tr><td>{ug.UserGroupId}</td><td>{ug.User.Username}</td><td>{ug.Group.GroupName}</td><td>{ug.AssignedAt:yyyy-MM-dd HH:mm:ss}</td></tr>");
        }
        html.AppendLine("            </tbody>");
        html.AppendLine("        </table>");

        // User-Role Assignments
        html.AppendLine("        <h2>🔗 User-Role Assignments</h2>");
        html.AppendLine("        <table>");
        html.AppendLine("            <thead><tr><th>ID</th><th>User</th><th>Role</th><th>Assigned At</th></tr></thead>");
        html.AppendLine("            <tbody>");
        var userRoles = await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .OrderBy(ur => ur.UserRoleId)
            .ToListAsync();
        foreach (var ur in userRoles)
        {
            html.AppendLine($"                <tr><td>{ur.UserRoleId}</td><td>{ur.User.Username}</td><td>{ur.Role.RoleName}</td><td>{ur.AssignedAt:yyyy-MM-dd HH:mm:ss}</td></tr>");
        }
        html.AppendLine("            </tbody>");
        html.AppendLine("        </table>");

        // Group-Role Assignments
        html.AppendLine("        <h2>🔗 Group-Role Assignments</h2>");
        html.AppendLine("        <table>");
        html.AppendLine("            <thead><tr><th>ID</th><th>Group</th><th>Role</th><th>Assigned At</th></tr></thead>");
        html.AppendLine("            <tbody>");
        var groupRoles = await _context.GroupRoles
            .Include(gr => gr.Group)
            .Include(gr => gr.Role)
            .OrderBy(gr => gr.GroupRoleId)
            .ToListAsync();
        foreach (var gr in groupRoles)
        {
            html.AppendLine($"                <tr><td>{gr.GroupRoleId}</td><td>{gr.Group.GroupName}</td><td>{gr.Role.RoleName}</td><td>{gr.AssignedAt:yyyy-MM-dd HH:mm:ss}</td></tr>");
        }
        html.AppendLine("            </tbody>");
        html.AppendLine("        </table>");

        // Role-Permission Assignments
        html.AppendLine("        <h2>🔗 Role-Permission Assignments</h2>");
        html.AppendLine("        <table>");
        html.AppendLine("            <thead><tr><th>ID</th><th>Role</th><th>Permission</th></tr></thead>");
        html.AppendLine("            <tbody>");
        var rolePermissions = await _context.RolePermissions
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .OrderBy(rp => rp.RolePermissionId)
            .ToListAsync();
        foreach (var rp in rolePermissions)
        {
            html.AppendLine($"                <tr><td>{rp.RolePermissionId}</td><td>{rp.Role.RoleName}</td><td>{rp.Permission.PermissionName}</td></tr>");
        }
        html.AppendLine("            </tbody>");
        html.AppendLine("        </table>");

        html.AppendLine("    </div>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }

    private string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}
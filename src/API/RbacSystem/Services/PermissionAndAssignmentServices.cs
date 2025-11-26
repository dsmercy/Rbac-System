using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RbacSystem.Data;
using RbacSystem.Models.Common;
using RbacSystem.Models.DTOs;
using RbacSystem.Models.Entities;

namespace RbacSystem.Services;

public interface IPermissionService
{
    Task<PagedResult<PermissionDto>> GetPermissionsAsync(PermissionFilterParams filterParams);
    Task<PermissionDto?> GetPermissionByIdAsync(long permissionId);
    Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto createPermissionDto);
    Task<PermissionDto?> UpdatePermissionAsync(long permissionId, UpdatePermissionDto updatePermissionDto);
    Task<bool> DeletePermissionAsync(long permissionId);
}

public class PermissionService : IPermissionService
{
    private readonly RbacDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly ILogger<PermissionService> _logger;
    private const string PermissionCacheKeyPrefix = "permission:";

    public PermissionService(
        RbacDbContext context,
        IMapper mapper,
        ICacheService cacheService,
        ILogger<PermissionService> logger)
    {
        _context = context;
        _mapper = mapper;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<PagedResult<PermissionDto>> GetPermissionsAsync(PermissionFilterParams filterParams)
    {
        var query = _context.Permissions.AsQueryable();

        if (!string.IsNullOrEmpty(filterParams.PermissionName))
        {
            query = query.Where(p => p.PermissionName.Contains(filterParams.PermissionName));
        }

        var totalCount = await query.CountAsync();

        var permissions = await query
            .OrderBy(p => p.PermissionId)
            .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
            .Take(filterParams.PageSize)
            .ToListAsync();

        var permissionDtos = _mapper.Map<List<PermissionDto>>(permissions);

        return new PagedResult<PermissionDto>
        {
            Items = permissionDtos,
            Metadata = new PaginationMetadata
            {
                CurrentPage = filterParams.PageNumber,
                PageSize = filterParams.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filterParams.PageSize),
                HasPrevious = filterParams.PageNumber > 1,
                HasNext = filterParams.PageNumber * filterParams.PageSize < totalCount
            }
        };
    }

    public async Task<PermissionDto?> GetPermissionByIdAsync(long permissionId)
    {
        var cacheKey = $"{PermissionCacheKeyPrefix}{permissionId}";
        var cachedPermission = await _cacheService.GetAsync<PermissionDto>(cacheKey);

        if (cachedPermission != null)
        {
            _logger.LogInformation("Permission {PermissionId} retrieved from cache", permissionId);
            return cachedPermission;
        }

        var permission = await _context.Permissions.FindAsync(permissionId);
        if (permission == null)
            return null;

        var permissionDto = _mapper.Map<PermissionDto>(permission);
        await _cacheService.SetAsync(cacheKey, permissionDto, TimeSpan.FromMinutes(10));

        return permissionDto;
    }

    public async Task<PermissionDto> CreatePermissionAsync(CreatePermissionDto createPermissionDto)
    {
        var permission = _mapper.Map<Permission>(createPermissionDto);
        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();

        var permissionDto = _mapper.Map<PermissionDto>(permission);
        await _cacheService.SetAsync($"{PermissionCacheKeyPrefix}{permission.PermissionId}", permissionDto, TimeSpan.FromMinutes(10));

        return permissionDto;
    }

    public async Task<PermissionDto?> UpdatePermissionAsync(long permissionId, UpdatePermissionDto updatePermissionDto)
    {
        var permission = await _context.Permissions.FindAsync(permissionId);
        if (permission == null)
            return null;

        _mapper.Map(updatePermissionDto, permission);
        await _context.SaveChangesAsync();

        var permissionDto = _mapper.Map<PermissionDto>(permission);
        await _cacheService.RemoveAsync($"{PermissionCacheKeyPrefix}{permissionId}");
        await _cacheService.RemoveByPatternAsync("user:permissions:*");

        return permissionDto;
    }

    public async Task<bool> DeletePermissionAsync(long permissionId)
    {
        var permission = await _context.Permissions.FindAsync(permissionId);
        if (permission == null)
            return false;

        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveAsync($"{PermissionCacheKeyPrefix}{permissionId}");
        await _cacheService.RemoveByPatternAsync("user:permissions:*");

        return true;
    }
}

// Assignment Service for managing relationships
public interface IAssignmentService
{
    Task<bool> AssignRoleToUserAsync(AssignRoleToUserDto dto);
    Task<bool> RemoveRoleFromUserAsync(long userId, long roleId);
    Task<bool> AssignRoleToGroupAsync(AssignRoleToGroupDto dto);
    Task<bool> RemoveRoleFromGroupAsync(long groupId, long roleId);
    Task<bool> AssignUserToGroupAsync(AssignUserToGroupDto dto);
    Task<bool> RemoveUserFromGroupAsync(long userId, long groupId);
    Task<bool> AssignPermissionToRoleAsync(AssignPermissionToRoleDto dto);
    Task<bool> RemovePermissionFromRoleAsync(long roleId, long permissionId);

    // Bulk operations
    Task<int> BulkAssignRolesToUserAsync(BulkAssignRolesToUserDto dto);
    Task<int> BulkAssignUsersToGroupAsync(BulkAssignUsersToGroupDto dto);
    Task<int> BulkAssignPermissionsToRoleAsync(BulkAssignPermissionsToRoleDto dto);
}

public class AssignmentService : IAssignmentService
{
    private readonly RbacDbContext _context;
    private readonly ICacheService _cacheService;
    private readonly ILogger<AssignmentService> _logger;

    public AssignmentService(
        RbacDbContext context,
        ICacheService cacheService,
        ILogger<AssignmentService> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<bool> AssignRoleToUserAsync(AssignRoleToUserDto dto)
    {
        var exists = await _context.UserRoles
            .AnyAsync(ur => ur.UserId == dto.UserId && ur.RoleId == dto.RoleId);

        if (exists)
            return false;

        var userRole = new UserRole
        {
            UserId = dto.UserId,
            RoleId = dto.RoleId
        };

        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveAsync($"user:permissions:{dto.UserId}");

        return true;
    }

    public async Task<bool> RemoveRoleFromUserAsync(long userId, long roleId)
    {
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (userRole == null)
            return false;

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveAsync($"user:permissions:{userId}");

        return true;
    }

    public async Task<bool> AssignRoleToGroupAsync(AssignRoleToGroupDto dto)
    {
        var exists = await _context.GroupRoles
            .AnyAsync(gr => gr.GroupId == dto.GroupId && gr.RoleId == dto.RoleId);

        if (exists)
            return false;

        var groupRole = new GroupRole
        {
            GroupId = dto.GroupId,
            RoleId = dto.RoleId
        };

        _context.GroupRoles.Add(groupRole);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveByPatternAsync("user:permissions:*");

        return true;
    }

    public async Task<bool> RemoveRoleFromGroupAsync(long groupId, long roleId)
    {
        var groupRole = await _context.GroupRoles
            .FirstOrDefaultAsync(gr => gr.GroupId == groupId && gr.RoleId == roleId);

        if (groupRole == null)
            return false;

        _context.GroupRoles.Remove(groupRole);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveByPatternAsync("user:permissions:*");

        return true;
    }

    public async Task<bool> AssignUserToGroupAsync(AssignUserToGroupDto dto)
    {
        var exists = await _context.UserGroups
            .AnyAsync(ug => ug.UserId == dto.UserId && ug.GroupId == dto.GroupId);

        if (exists)
            return false;

        var userGroup = new UserGroup
        {
            UserId = dto.UserId,
            GroupId = dto.GroupId
        };

        _context.UserGroups.Add(userGroup);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveAsync($"user:permissions:{dto.UserId}");

        return true;
    }

    public async Task<bool> RemoveUserFromGroupAsync(long userId, long groupId)
    {
        var userGroup = await _context.UserGroups
            .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId);

        if (userGroup == null)
            return false;

        _context.UserGroups.Remove(userGroup);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveAsync($"user:permissions:{userId}");

        return true;
    }

    public async Task<bool> AssignPermissionToRoleAsync(AssignPermissionToRoleDto dto)
    {
        var exists = await _context.RolePermissions
            .AnyAsync(rp => rp.RoleId == dto.RoleId && rp.PermissionId == dto.PermissionId);

        if (exists)
            return false;

        var rolePermission = new RolePermission
        {
            RoleId = dto.RoleId,
            PermissionId = dto.PermissionId
        };

        _context.RolePermissions.Add(rolePermission);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveByPatternAsync("user:permissions:*");
        await _cacheService.RemoveAsync($"role:{dto.RoleId}");

        return true;
    }

    public async Task<bool> RemovePermissionFromRoleAsync(long roleId, long permissionId)
    {
        var rolePermission = await _context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

        if (rolePermission == null)
            return false;

        _context.RolePermissions.Remove(rolePermission);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveByPatternAsync("user:permissions:*");
        await _cacheService.RemoveAsync($"role:{roleId}");

        return true;
    }

    public async Task<int> BulkAssignRolesToUserAsync(BulkAssignRolesToUserDto dto)
    {
        var existingAssignments = await _context.UserRoles
            .Where(ur => ur.UserId == dto.UserId && dto.RoleIds.Contains(ur.RoleId))
            .Select(ur => ur.RoleId)
            .ToListAsync();

        var newRoleIds = dto.RoleIds.Except(existingAssignments).ToList();

        if (!newRoleIds.Any())
            return 0;

        var userRoles = newRoleIds.Select(roleId => new UserRole
        {
            UserId = dto.UserId,
            RoleId = roleId
        }).ToList();

        _context.UserRoles.AddRange(userRoles);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveAsync($"user:permissions:{dto.UserId}");

        return userRoles.Count;
    }

    public async Task<int> BulkAssignUsersToGroupAsync(BulkAssignUsersToGroupDto dto)
    {
        var existingAssignments = await _context.UserGroups
            .Where(ug => ug.GroupId == dto.GroupId && dto.UserIds.Contains(ug.UserId))
            .Select(ug => ug.UserId)
            .ToListAsync();

        var newUserIds = dto.UserIds.Except(existingAssignments).ToList();

        if (!newUserIds.Any())
            return 0;

        var userGroups = newUserIds.Select(userId => new UserGroup
        {
            UserId = userId,
            GroupId = dto.GroupId
        }).ToList();

        _context.UserGroups.AddRange(userGroups);
        await _context.SaveChangesAsync();

        foreach (var userId in newUserIds)
        {
            await _cacheService.RemoveAsync($"user:permissions:{userId}");
        }

        return userGroups.Count;
    }

    public async Task<int> BulkAssignPermissionsToRoleAsync(BulkAssignPermissionsToRoleDto dto)
    {
        var existingAssignments = await _context.RolePermissions
            .Where(rp => rp.RoleId == dto.RoleId && dto.PermissionIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        var newPermissionIds = dto.PermissionIds.Except(existingAssignments).ToList();

        if (!newPermissionIds.Any())
            return 0;

        var rolePermissions = newPermissionIds.Select(permissionId => new RolePermission
        {
            RoleId = dto.RoleId,
            PermissionId = permissionId
        }).ToList();

        _context.RolePermissions.AddRange(rolePermissions);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveByPatternAsync("user:permissions:*");
        await _cacheService.RemoveAsync($"role:{dto.RoleId}");

        return rolePermissions.Count;
    }
}
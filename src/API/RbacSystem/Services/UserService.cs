using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RbacSystem.Data;
using RbacSystem.Models.Common;
using RbacSystem.Models.DTOs;
using RbacSystem.Models.Entities;

namespace RbacSystem.Services;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetUsersAsync(UserFilterParams filterParams);
    Task<UserDto?> GetUserByIdAsync(long userId);
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserDto?> UpdateUserAsync(long userId, UpdateUserDto updateUserDto);
    Task<bool> DeleteUserAsync(long userId);
    Task<UserEffectivePermissionsDto?> GetUserEffectivePermissionsAsync(long userId);
}

public class UserService : IUserService
{
    private readonly RbacDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly ILogger<UserService> _logger;
    private const string UserCacheKeyPrefix = "user:";
    private const string UserPermissionsCacheKeyPrefix = "user:permissions:";

    public UserService(
        RbacDbContext context,
        IMapper mapper,
        ICacheService cacheService,
        ILogger<UserService> logger)
    {
        _context = context;
        _mapper = mapper;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<PagedResult<UserDto>> GetUsersAsync(UserFilterParams filterParams)
    {
        var query = _context.Users.AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(filterParams.Username))
        {
            query = query.Where(u => u.Username.Contains(filterParams.Username));
        }

        if (!string.IsNullOrEmpty(filterParams.Email))
        {
            query = query.Where(u => u.Email.Contains(filterParams.Email));
        }

        if (filterParams.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == filterParams.IsActive.Value);
        }

        var totalCount = await query.CountAsync();

        var users = await query
            .OrderBy(u => u.UserId)
            .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
            .Take(filterParams.PageSize)
            .ToListAsync();

        var userDtos = _mapper.Map<List<UserDto>>(users);

        return new PagedResult<UserDto>
        {
            Items = userDtos,
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

    public async Task<UserDto?> GetUserByIdAsync(long userId)
    {
        var cacheKey = $"{UserCacheKeyPrefix}{userId}";
        var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey);

        if (cachedUser != null)
        {
            _logger.LogInformation("User {UserId} retrieved from cache", userId);
            return cachedUser;
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return null;

        var userDto = _mapper.Map<UserDto>(user);
        await _cacheService.SetAsync(cacheKey, userDto, TimeSpan.FromMinutes(10));

        return userDto;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = _mapper.Map<User>(createUserDto);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userDto = _mapper.Map<UserDto>(user);
        await _cacheService.SetAsync($"{UserCacheKeyPrefix}{user.UserId}", userDto, TimeSpan.FromMinutes(10));

        return userDto;
    }

    public async Task<UserDto?> UpdateUserAsync(long userId, UpdateUserDto updateUserDto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return null;

        _mapper.Map(updateUserDto, user);
        await _context.SaveChangesAsync();

        var userDto = _mapper.Map<UserDto>(user);

        // Invalidate cache
        await _cacheService.RemoveAsync($"{UserCacheKeyPrefix}{userId}");
        await _cacheService.RemoveAsync($"{UserPermissionsCacheKeyPrefix}{userId}");

        return userDto;
    }

    public async Task<bool> DeleteUserAsync(long userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        // Invalidate cache
        await _cacheService.RemoveAsync($"{UserCacheKeyPrefix}{userId}");
        await _cacheService.RemoveAsync($"{UserPermissionsCacheKeyPrefix}{userId}");

        return true;
    }

    public async Task<UserEffectivePermissionsDto?> GetUserEffectivePermissionsAsync(long userId)
    {
        var cacheKey = $"{UserPermissionsCacheKeyPrefix}{userId}";
        var cachedPermissions = await _cacheService.GetAsync<UserEffectivePermissionsDto>(cacheKey);

        if (cachedPermissions != null)
        {
            _logger.LogInformation("User permissions for {UserId} retrieved from cache", userId);
            return cachedPermissions;
        }

        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserGroups)
                .ThenInclude(ug => ug.Group)
                    .ThenInclude(g => g.GroupRoles)
                        .ThenInclude(gr => gr.Role)
                            .ThenInclude(r => r.RolePermissions)
                                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
            return null;

        // Get direct permissions from user roles
        var directPermissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.PermissionName)
            .Distinct()
            .ToList();

        // Get inherited permissions from group roles
        var groupPermissions = user.UserGroups
            .SelectMany(ug => ug.Group.GroupRoles)
            .SelectMany(gr => gr.Role.RolePermissions)
            .Select(rp => rp.Permission.PermissionName)
            .Distinct()
            .ToList();

        var allPermissions = directPermissions
            .Union(groupPermissions)
            .Distinct()
            .OrderBy(p => p)
            .ToList();

        var result = new UserEffectivePermissionsDto
        {
            UserId = user.UserId,
            Username = user.Username,
            DirectPermissions = directPermissions,
            GroupInheritedPermissions = groupPermissions,
            AllPermissions = allPermissions
        };

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

        return result;
    }
}
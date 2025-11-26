using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RbacSystem.Data;
using RbacSystem.Models.Common;
using RbacSystem.Models.DTOs;
using RbacSystem.Models.Entities;

namespace RbacSystem.Services;

public interface IRoleService
{
    Task<PagedResult<RoleDto>> GetRolesAsync(RoleFilterParams filterParams);
    Task<RoleDto?> GetRoleByIdAsync(long roleId);
    Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto);
    Task<RoleDto?> UpdateRoleAsync(long roleId, UpdateRoleDto updateRoleDto);
    Task<bool> DeleteRoleAsync(long roleId);
}

public class RoleService : IRoleService
{
    private readonly RbacDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly ILogger<RoleService> _logger;
    private const string RoleCacheKeyPrefix = "role:";

    public RoleService(
        RbacDbContext context,
        IMapper mapper,
        ICacheService cacheService,
        ILogger<RoleService> logger)
    {
        _context = context;
        _mapper = mapper;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<PagedResult<RoleDto>> GetRolesAsync(RoleFilterParams filterParams)
    {
        var query = _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filterParams.RoleName))
        {
            query = query.Where(r => r.RoleName.Contains(filterParams.RoleName));
        }

        var totalCount = await query.CountAsync();

        var roles = await query
            .OrderBy(r => r.RoleId)
            .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
            .Take(filterParams.PageSize)
            .ToListAsync();

        var roleDtos = _mapper.Map<List<RoleDto>>(roles);

        return new PagedResult<RoleDto>
        {
            Items = roleDtos,
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

    public async Task<RoleDto?> GetRoleByIdAsync(long roleId)
    {
        var cacheKey = $"{RoleCacheKeyPrefix}{roleId}";
        var cachedRole = await _cacheService.GetAsync<RoleDto>(cacheKey);

        if (cachedRole != null)
        {
            _logger.LogInformation("Role {RoleId} retrieved from cache", roleId);
            return cachedRole;
        }

        var role = await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.RoleId == roleId);

        if (role == null)
            return null;

        var roleDto = _mapper.Map<RoleDto>(role);
        await _cacheService.SetAsync(cacheKey, roleDto, TimeSpan.FromMinutes(10));

        return roleDto;
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
    {
        var role = _mapper.Map<Role>(createRoleDto);
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        var roleDto = _mapper.Map<RoleDto>(role);
        await _cacheService.SetAsync($"{RoleCacheKeyPrefix}{role.RoleId}", roleDto, TimeSpan.FromMinutes(10));

        return roleDto;
    }

    public async Task<RoleDto?> UpdateRoleAsync(long roleId, UpdateRoleDto updateRoleDto)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role == null)
            return null;

        _mapper.Map(updateRoleDto, role);
        await _context.SaveChangesAsync();

        var roleDto = _mapper.Map<RoleDto>(role);
        await _cacheService.RemoveAsync($"{RoleCacheKeyPrefix}{roleId}");
        await _cacheService.RemoveByPatternAsync("user:permissions:*");

        return roleDto;
    }

    public async Task<bool> DeleteRoleAsync(long roleId)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role == null)
            return false;

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveAsync($"{RoleCacheKeyPrefix}{roleId}");
        await _cacheService.RemoveByPatternAsync("user:permissions:*");

        return true;
    }
}
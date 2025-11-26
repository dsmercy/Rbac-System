using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RbacSystem.Data;
using RbacSystem.Models.Common;
using RbacSystem.Models.DTOs;
using RbacSystem.Models.Entities;

namespace RbacSystem.Services;

public interface IGroupService
{
    Task<PagedResult<GroupDto>> GetGroupsAsync(GroupFilterParams filterParams);
    Task<GroupDto?> GetGroupByIdAsync(long groupId);
    Task<GroupDto> CreateGroupAsync(CreateGroupDto createGroupDto);
    Task<GroupDto?> UpdateGroupAsync(long groupId, UpdateGroupDto updateGroupDto);
    Task<bool> DeleteGroupAsync(long groupId);
}

public class GroupService : IGroupService
{
    private readonly RbacDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GroupService> _logger;
    private const string GroupCacheKeyPrefix = "group:";

    public GroupService(
        RbacDbContext context,
        IMapper mapper,
        ICacheService cacheService,
        ILogger<GroupService> logger)
    {
        _context = context;
        _mapper = mapper;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<PagedResult<GroupDto>> GetGroupsAsync(GroupFilterParams filterParams)
    {
        var query = _context.Groups.AsQueryable();

        if (!string.IsNullOrEmpty(filterParams.GroupName))
        {
            query = query.Where(g => g.GroupName.Contains(filterParams.GroupName));
        }

        var totalCount = await query.CountAsync();

        var groups = await query
            .OrderBy(g => g.GroupId)
            .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
            .Take(filterParams.PageSize)
            .ToListAsync();

        var groupDtos = _mapper.Map<List<GroupDto>>(groups);

        return new PagedResult<GroupDto>
        {
            Items = groupDtos,
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

    public async Task<GroupDto?> GetGroupByIdAsync(long groupId)
    {
        var cacheKey = $"{GroupCacheKeyPrefix}{groupId}";
        var cachedGroup = await _cacheService.GetAsync<GroupDto>(cacheKey);

        if (cachedGroup != null)
        {
            _logger.LogInformation("Group {GroupId} retrieved from cache", groupId);
            return cachedGroup;
        }

        var group = await _context.Groups.FindAsync(groupId);
        if (group == null)
            return null;

        var groupDto = _mapper.Map<GroupDto>(group);
        await _cacheService.SetAsync(cacheKey, groupDto, TimeSpan.FromMinutes(10));

        return groupDto;
    }

    public async Task<GroupDto> CreateGroupAsync(CreateGroupDto createGroupDto)
    {
        var group = _mapper.Map<Group>(createGroupDto);
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        var groupDto = _mapper.Map<GroupDto>(group);
        await _cacheService.SetAsync($"{GroupCacheKeyPrefix}{group.GroupId}", groupDto, TimeSpan.FromMinutes(10));

        return groupDto;
    }

    public async Task<GroupDto?> UpdateGroupAsync(long groupId, UpdateGroupDto updateGroupDto)
    {
        var group = await _context.Groups.FindAsync(groupId);
        if (group == null)
            return null;

        _mapper.Map(updateGroupDto, group);
        await _context.SaveChangesAsync();

        var groupDto = _mapper.Map<GroupDto>(group);
        await _cacheService.RemoveAsync($"{GroupCacheKeyPrefix}{groupId}");

        return groupDto;
    }

    public async Task<bool> DeleteGroupAsync(long groupId)
    {
        var group = await _context.Groups.FindAsync(groupId);
        if (group == null)
            return false;

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();

        await _cacheService.RemoveAsync($"{GroupCacheKeyPrefix}{groupId}");

        return true;
    }
}
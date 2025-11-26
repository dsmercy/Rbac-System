using AutoMapper;
using RbacSystem.Models.DTOs;
using RbacSystem.Models.Entities;

namespace RbacSystem.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Group mappings
        CreateMap<Group, GroupDto>();
        CreateMap<CreateGroupDto, Group>();
        CreateMap<UpdateGroupDto, Group>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Role mappings
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.Permissions,
                opt => opt.MapFrom(src => src.RolePermissions.Select(rp => rp.Permission)));
        CreateMap<CreateRoleDto, Role>();
        CreateMap<UpdateRoleDto, Role>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Permission mappings
        CreateMap<Permission, PermissionDto>();
        CreateMap<CreatePermissionDto, Permission>();
        CreateMap<UpdatePermissionDto, Permission>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
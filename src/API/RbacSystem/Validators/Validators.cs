using FluentValidation;
using RbacSystem.Models.DTOs;

namespace RbacSystem.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MaximumLength(100).WithMessage("Username cannot exceed 100 characters")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Username can only contain alphanumeric characters, hyphens, and underscores");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(200).WithMessage("Email cannot exceed 200 characters");
    }
}

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .MaximumLength(100).WithMessage("Username cannot exceed 100 characters")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Username can only contain alphanumeric characters, hyphens, and underscores")
            .When(x => !string.IsNullOrEmpty(x.Username));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(200).WithMessage("Email cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}

public class CreateGroupDtoValidator : AbstractValidator<CreateGroupDto>
{
    public CreateGroupDtoValidator()
    {
        RuleFor(x => x.GroupName)
            .NotEmpty().WithMessage("Group name is required")
            .MaximumLength(150).WithMessage("Group name cannot exceed 150 characters");

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Description cannot exceed 300 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class UpdateGroupDtoValidator : AbstractValidator<UpdateGroupDto>
{
    public UpdateGroupDtoValidator()
    {
        RuleFor(x => x.GroupName)
            .MaximumLength(150).WithMessage("Group name cannot exceed 150 characters")
            .When(x => !string.IsNullOrEmpty(x.GroupName));

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Description cannot exceed 300 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
{
    public CreateRoleDtoValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty().WithMessage("Role name is required")
            .MaximumLength(100).WithMessage("Role name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Description cannot exceed 300 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class UpdateRoleDtoValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleDtoValidator()
    {
        RuleFor(x => x.RoleName)
            .MaximumLength(100).WithMessage("Role name cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.RoleName));

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Description cannot exceed 300 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class CreatePermissionDtoValidator : AbstractValidator<CreatePermissionDto>
{
    public CreatePermissionDtoValidator()
    {
        RuleFor(x => x.PermissionName)
            .NotEmpty().WithMessage("Permission name is required")
            .MaximumLength(150).WithMessage("Permission name cannot exceed 150 characters");

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Description cannot exceed 300 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class UpdatePermissionDtoValidator : AbstractValidator<UpdatePermissionDto>
{
    public UpdatePermissionDtoValidator()
    {
        RuleFor(x => x.PermissionName)
            .MaximumLength(150).WithMessage("Permission name cannot exceed 150 characters")
            .When(x => !string.IsNullOrEmpty(x.PermissionName));

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Description cannot exceed 300 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class AssignRoleToUserDtoValidator : AbstractValidator<AssignRoleToUserDto>
{
    public AssignRoleToUserDtoValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("Valid UserId is required");
        RuleFor(x => x.RoleId).GreaterThan(0).WithMessage("Valid RoleId is required");
    }
}

public class AssignRoleToGroupDtoValidator : AbstractValidator<AssignRoleToGroupDto>
{
    public AssignRoleToGroupDtoValidator()
    {
        RuleFor(x => x.GroupId).GreaterThan(0).WithMessage("Valid GroupId is required");
        RuleFor(x => x.RoleId).GreaterThan(0).WithMessage("Valid RoleId is required");
    }
}

public class AssignUserToGroupDtoValidator : AbstractValidator<AssignUserToGroupDto>
{
    public AssignUserToGroupDtoValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("Valid UserId is required");
        RuleFor(x => x.GroupId).GreaterThan(0).WithMessage("Valid GroupId is required");
    }
}

public class AssignPermissionToRoleDtoValidator : AbstractValidator<AssignPermissionToRoleDto>
{
    public AssignPermissionToRoleDtoValidator()
    {
        RuleFor(x => x.RoleId).GreaterThan(0).WithMessage("Valid RoleId is required");
        RuleFor(x => x.PermissionId).GreaterThan(0).WithMessage("Valid PermissionId is required");
    }
}

public class BulkAssignRolesToUserDtoValidator : AbstractValidator<BulkAssignRolesToUserDto>
{
    public BulkAssignRolesToUserDtoValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("Valid UserId is required");
        RuleFor(x => x.RoleIds).NotEmpty().WithMessage("At least one RoleId is required");
        RuleForEach(x => x.RoleIds).GreaterThan(0).WithMessage("All RoleIds must be valid");
    }
}

public class BulkAssignUsersToGroupDtoValidator : AbstractValidator<BulkAssignUsersToGroupDto>
{
    public BulkAssignUsersToGroupDtoValidator()
    {
        RuleFor(x => x.GroupId).GreaterThan(0).WithMessage("Valid GroupId is required");
        RuleFor(x => x.UserIds).NotEmpty().WithMessage("At least one UserId is required");
        RuleForEach(x => x.UserIds).GreaterThan(0).WithMessage("All UserIds must be valid");
    }
}

public class BulkAssignPermissionsToRoleDtoValidator : AbstractValidator<BulkAssignPermissionsToRoleDto>
{
    public BulkAssignPermissionsToRoleDtoValidator()
    {
        RuleFor(x => x.RoleId).GreaterThan(0).WithMessage("Valid RoleId is required");
        RuleFor(x => x.PermissionIds).NotEmpty().WithMessage("At least one PermissionId is required");
        RuleForEach(x => x.PermissionIds).GreaterThan(0).WithMessage("All PermissionIds must be valid");
    }
}
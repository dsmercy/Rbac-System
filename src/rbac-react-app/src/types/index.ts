// API Response Types
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: string[];
  pagination?: PaginationMetadata;
}

export interface PaginationMetadata {
  currentPage: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

// Entity Types
export interface User {
  userId: number;
  username: string;
  email: string;
  isActive: boolean;
  createdAt: string;
}

export interface Group {
  groupId: number;
  groupName: string;
  description?: string;
  createdAt: string;
}

export interface Role {
  roleId: number;
  roleName: string;
  description?: string;
  createdAt: string;
  permissions?: Permission[];
}

export interface Permission {
  permissionId: number;
  permissionName: string;
  description?: string;
}

export interface UserEffectivePermissions {
  userId: number;
  username: string;
  directPermissions: string[];
  groupInheritedPermissions: string[];
  allPermissions: string[];
}

// DTO Types
export interface CreateUserDto {
  username: string;
  email: string;
  isActive?: boolean;
}

export interface UpdateUserDto {
  username?: string;
  email?: string;
  isActive?: boolean;
}

export interface CreateGroupDto {
  groupName: string;
  description?: string;
}

export interface UpdateGroupDto {
  groupName?: string;
  description?: string;
}

export interface CreateRoleDto {
  roleName: string;
  description?: string;
}

export interface UpdateRoleDto {
  roleName?: string;
  description?: string;
}

export interface CreatePermissionDto {
  permissionName: string;
  description?: string;
}

export interface UpdatePermissionDto {
  permissionName?: string;
  description?: string;
}

export interface AssignRoleToUserDto {
  userId: number;
  roleId: number;
}

export interface AssignRoleToGroupDto {
  groupId: number;
  roleId: number;
}

export interface AssignUserToGroupDto {
  userId: number;
  groupId: number;
}

export interface AssignPermissionToRoleDto {
  roleId: number;
  permissionId: number;
}

export interface BulkAssignRolesToUserDto {
  userId: number;
  roleIds: number[];
}

export interface BulkAssignUsersToGroupDto {
  groupId: number;
  userIds: number[];
}

export interface BulkAssignPermissionsToRoleDto {
  roleId: number;
  permissionIds: number[];
}

// Filter Params
export interface PaginationParams {
  pageNumber?: number;
  pageSize?: number;
}

export interface UserFilterParams extends PaginationParams {
  username?: string;
  email?: string;
  isActive?: boolean;
}

export interface GroupFilterParams extends PaginationParams {
  groupName?: string;
}

export interface RoleFilterParams extends PaginationParams {
  roleName?: string;
}

export interface PermissionFilterParams extends PaginationParams {
  permissionName?: string;
}

// Auth Types
export interface LoginCredentials {
  username: string;
  password: string;
}

export interface AuthUser {
  userId: number;
  username: string;
  email: string;
  permissions: string[];
  token: string;
}

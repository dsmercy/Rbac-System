import apiService from './apiService';
import {
  AssignRoleToUserDto,
  AssignRoleToGroupDto,
  AssignUserToGroupDto,
  AssignPermissionToRoleDto,
  BulkAssignRolesToUserDto,
  BulkAssignUsersToGroupDto,
  BulkAssignPermissionsToRoleDto,
  ApiResponse,
} from '@/types';

export const assignmentService = {
  // User-Role assignments
  async assignRoleToUser(data: AssignRoleToUserDto): Promise<ApiResponse<boolean>> {
    return apiService.post<boolean>('/assignments/user-role', data);
  },

  async removeRoleFromUser(userId: number, roleId: number): Promise<ApiResponse<boolean>> {
    return apiService.delete<boolean>(`/assignments/user-role/${userId}/${roleId}`);
  },

  // Group-Role assignments
  async assignRoleToGroup(data: AssignRoleToGroupDto): Promise<ApiResponse<boolean>> {
    return apiService.post<boolean>('/assignments/group-role', data);
  },

  async removeRoleFromGroup(groupId: number, roleId: number): Promise<ApiResponse<boolean>> {
    return apiService.delete<boolean>(`/assignments/group-role/${groupId}/${roleId}`);
  },

  // User-Group assignments
  async assignUserToGroup(data: AssignUserToGroupDto): Promise<ApiResponse<boolean>> {
    return apiService.post<boolean>('/assignments/user-group', data);
  },

  async removeUserFromGroup(userId: number, groupId: number): Promise<ApiResponse<boolean>> {
    return apiService.delete<boolean>(`/assignments/user-group/${userId}/${groupId}`);
  },

  // Role-Permission assignments
  async assignPermissionToRole(data: AssignPermissionToRoleDto): Promise<ApiResponse<boolean>> {
    return apiService.post<boolean>('/assignments/role-permission', data);
  },

  async removePermissionFromRole(roleId: number, permissionId: number): Promise<ApiResponse<boolean>> {
    return apiService.delete<boolean>(`/assignments/role-permission/${roleId}/${permissionId}`);
  },

  // Bulk operations
  async bulkAssignRolesToUser(data: BulkAssignRolesToUserDto): Promise<ApiResponse<number>> {
    return apiService.post<number>('/assignments/bulk/user-roles', data);
  },

  async bulkAssignUsersToGroup(data: BulkAssignUsersToGroupDto): Promise<ApiResponse<number>> {
    return apiService.post<number>('/assignments/bulk/group-users', data);
  },

  async bulkAssignPermissionsToRole(data: BulkAssignPermissionsToRoleDto): Promise<ApiResponse<number>> {
    return apiService.post<number>('/assignments/bulk/role-permissions', data);
  },
};

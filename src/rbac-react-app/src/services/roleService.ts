import apiService from './apiService';
import {
  Role,
  CreateRoleDto,
  UpdateRoleDto,
  RoleFilterParams,
  ApiResponse,
} from '@/types';

export const roleService = {
  // Get all roles with filters
  async getRoles(filters?: RoleFilterParams): Promise<ApiResponse<Role[]>> {
    const params: Record<string, any> = {};
    
    if (filters?.pageNumber) params.pageNumber = filters.pageNumber;
    if (filters?.pageSize) params.pageSize = filters.pageSize;
    if (filters?.roleName) params.roleName = filters.roleName;
    
    return apiService.get<Role[]>('/roles', params);
  },

  // Get role by ID
  async getRoleById(roleId: number): Promise<ApiResponse<Role>> {
    return apiService.get<Role>(`/roles/${roleId}`);
  },

  // Create new role
  async createRole(data: CreateRoleDto): Promise<ApiResponse<Role>> {
    return apiService.post<Role>('/roles', data);
  },

  // Update role
  async updateRole(roleId: number, data: UpdateRoleDto): Promise<ApiResponse<Role>> {
    return apiService.put<Role>(`/roles/${roleId}`, data);
  },

  // Delete role
  async deleteRole(roleId: number): Promise<ApiResponse<boolean>> {
    return apiService.delete<boolean>(`/roles/${roleId}`);
  },
};

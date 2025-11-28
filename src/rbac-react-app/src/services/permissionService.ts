import apiService from './apiService';
import {
  Permission,
  CreatePermissionDto,
  UpdatePermissionDto,
  PermissionFilterParams,
  ApiResponse,
} from '@/types';

export const permissionService = {
  // Get all permissions with filters
  async getPermissions(filters?: PermissionFilterParams): Promise<ApiResponse<Permission[]>> {
    const params: Record<string, any> = {};
    
    if (filters?.pageNumber) params.pageNumber = filters.pageNumber;
    if (filters?.pageSize) params.pageSize = filters.pageSize;
    if (filters?.permissionName) params.permissionName = filters.permissionName;
    
    return apiService.get<Permission[]>('/permissions', params);
  },

  // Get permission by ID
  async getPermissionById(permissionId: number): Promise<ApiResponse<Permission>> {
    return apiService.get<Permission>(`/permissions/${permissionId}`);
  },

  // Create new permission
  async createPermission(data: CreatePermissionDto): Promise<ApiResponse<Permission>> {
    return apiService.post<Permission>('/permissions', data);
  },

  // Update permission
  async updatePermission(permissionId: number, data: UpdatePermissionDto): Promise<ApiResponse<Permission>> {
    return apiService.put<Permission>(`/permissions/${permissionId}`, data);
  },

  // Delete permission
  async deletePermission(permissionId: number): Promise<ApiResponse<boolean>> {
    return apiService.delete<boolean>(`/permissions/${permissionId}`);
  },
};

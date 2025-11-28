import apiService from './apiService';
import {
  User,
  CreateUserDto,
  UpdateUserDto,
  UserFilterParams,
  UserEffectivePermissions,
  ApiResponse,
} from '@/types';

export const userService = {
  // Get all users with filters
  async getUsers(filters?: UserFilterParams): Promise<ApiResponse<User[]>> {
    const params: Record<string, any> = {};
    
    if (filters?.pageNumber) params.pageNumber = filters.pageNumber;
    if (filters?.pageSize) params.pageSize = filters.pageSize;
    if (filters?.username) params.username = filters.username;
    if (filters?.email) params.email = filters.email;
    if (filters?.isActive !== undefined) params.isActive = filters.isActive;
    
    return apiService.get<User[]>('/users', params);
  },

  // Get user by ID
  async getUserById(userId: number): Promise<ApiResponse<User>> {
    return apiService.get<User>(`/users/${userId}`);
  },

  // Create new user
  async createUser(data: CreateUserDto): Promise<ApiResponse<User>> {
    return apiService.post<User>('/users', data);
  },

  // Update user
  async updateUser(userId: number, data: UpdateUserDto): Promise<ApiResponse<User>> {
    return apiService.put<User>(`/users/${userId}`, data);
  },

  // Delete user
  async deleteUser(userId: number): Promise<ApiResponse<boolean>> {
    return apiService.delete<boolean>(`/users/${userId}`);
  },

  // Get user's effective permissions
  async getUserPermissions(userId: number): Promise<ApiResponse<UserEffectivePermissions>> {
    return apiService.get<UserEffectivePermissions>(`/users/${userId}/permissions`);
  },
};

import apiService from './apiService';
import {
  Group,
  CreateGroupDto,
  UpdateGroupDto,
  GroupFilterParams,
  ApiResponse,
} from '@/types';

export const groupService = {
  // Get all groups with filters
  async getGroups(filters?: GroupFilterParams): Promise<ApiResponse<Group[]>> {
    const params: Record<string, any> = {};
    
    if (filters?.pageNumber) params.pageNumber = filters.pageNumber;
    if (filters?.pageSize) params.pageSize = filters.pageSize;
    if (filters?.groupName) params.groupName = filters.groupName;
    
    return apiService.get<Group[]>('/groups', params);
  },

  // Get group by ID
  async getGroupById(groupId: number): Promise<ApiResponse<Group>> {
    return apiService.get<Group>(`/groups/${groupId}`);
  },

  // Create new group
  async createGroup(data: CreateGroupDto): Promise<ApiResponse<Group>> {
    return apiService.post<Group>('/groups', data);
  },

  // Update group
  async updateGroup(groupId: number, data: UpdateGroupDto): Promise<ApiResponse<Group>> {
    return apiService.put<Group>(`/groups/${groupId}`, data);
  },

  // Delete group
  async deleteGroup(groupId: number): Promise<ApiResponse<boolean>> {
    return apiService.delete<boolean>(`/groups/${groupId}`);
  },
};

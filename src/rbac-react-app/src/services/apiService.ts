import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios';
import { ApiResponse, PaginationParams } from '@/types';

class ApiService {
  private api: AxiosInstance;

  constructor(baseURL: string = import.meta.env.VITE_API_URL || 'http://localhost:5225/api') {
    this.api = axios.create({
      baseURL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Request interceptor
    // this.api.interceptors.request.use(
    //   (config) => {
    //     const token = localStorage.getItem('token');
    //     if (token) {
    //       config.headers.Authorization = `Bearer ${token}`;
    //     }
    //     return config;
    //   },
    //   (error) => {
    //     return Promise.reject(error);
    //   }
    // );

    // Response interceptor
    this.api.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          localStorage.removeItem('token');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }

  // Generic GET request
  async get<T>(
    url: string,
    params?: Record<string, any>,
    config?: AxiosRequestConfig
  ): Promise<ApiResponse<T>> {
    try {
      const response: AxiosResponse<ApiResponse<T>> = await this.api.get(url, {
        params,
        ...config,
      });
      return response.data;
    } catch (error: any) {
      return this.handleError(error);
    }
  }

  // Generic POST request
  async post<T>(
    url: string,
    data?: any,
    config?: AxiosRequestConfig
  ): Promise<ApiResponse<T>> {
    try {
      const response: AxiosResponse<ApiResponse<T>> = await this.api.post(
        url,
        data,
        config
      );
      return response.data;
    } catch (error: any) {
      return this.handleError(error);
    }
  }

  // Generic PUT request
  async put<T>(
    url: string,
    data?: any,
    config?: AxiosRequestConfig
  ): Promise<ApiResponse<T>> {
    try {
      const response: AxiosResponse<ApiResponse<T>> = await this.api.put(
        url,
        data,
        config
      );
      return response.data;
    } catch (error: any) {
      return this.handleError(error);
    }
  }

  // Generic DELETE request
  async delete<T>(
    url: string,
    config?: AxiosRequestConfig
  ): Promise<ApiResponse<T>> {
    try {
      const response: AxiosResponse<ApiResponse<T>> = await this.api.delete(
        url,
        config
      );
      return response.data;
    } catch (error: any) {
      return this.handleError(error);
    }
  }

  // Generic PATCH request
  async patch<T>(
    url: string,
    data?: any,
    config?: AxiosRequestConfig
  ): Promise<ApiResponse<T>> {
    try {
      const response: AxiosResponse<ApiResponse<T>> = await this.api.patch(
        url,
        data,
        config
      );
      return response.data;
    } catch (error: any) {
      return this.handleError(error);
    }
  }

  // Download file
  async download(url: string, filename: string): Promise<void> {
    try {
      const response = await this.api.get(url, {
        responseType: 'blob',
      });
      
      const blob = new Blob([response.data]);
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      link.download = filename;
      link.click();
      window.URL.revokeObjectURL(link.href);
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Download failed');
    }
  }

  // Error handler
  private handleError(error: any): ApiResponse<any> {
    const message = error.response?.data?.message || error.message || 'An error occurred';
    const errors = error.response?.data?.errors || [message];
    
    return {
      success: false,
      message,
      errors,
    };
  }

  // Build query string from pagination params
  buildQueryParams(params: PaginationParams): string {
    const query = new URLSearchParams();
    
    if (params.pageNumber) query.append('pageNumber', params.pageNumber.toString());
    if (params.pageSize) query.append('pageSize', params.pageSize.toString());
    
    return query.toString();
  }
}

// Export singleton instance
export const apiService = new ApiService();

export default apiService;
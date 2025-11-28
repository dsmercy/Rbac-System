import apiService from './apiService';

export const exportService = {
  // Export to JSON
  async exportToJson(): Promise<void> {
    const filename = `rbac-export-${new Date().toISOString().split('T')[0]}.json`;
    return apiService.download('/export/json', filename);
  },

  // Export to Excel
  async exportToExcel(): Promise<void> {
    const filename = `rbac-export-${new Date().toISOString().split('T')[0]}.xlsx`;
    return apiService.download('/export/csv', filename);
  },

  // Export to HTML
  async exportToHtml(): Promise<void> {
    const filename = `rbac-export-${new Date().toISOString().split('T')[0]}.html`;
    return apiService.download('/export/html', filename);
  },

  // Get export info
  async getExportInfo(): Promise<any> {
    return apiService.get('/export/info');
  },
};

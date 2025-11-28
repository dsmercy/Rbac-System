// Export.tsx
import React from 'react';
import { Card, Button, Row, Col } from 'react-bootstrap';
import { Download, FileJson, FileSpreadsheet, FileText } from 'lucide-react';
import { exportService } from '@/services/exportService';
import toast from 'react-hot-toast';

const Export: React.FC = () => {
  const handleExport = async (format: 'json' | 'excel' | 'html') => {
    try {
      switch (format) {
        case 'json':
          await exportService.exportToJson();
          break;
        case 'excel':
          await exportService.exportToExcel();
          break;
        case 'html':
          await exportService.exportToHtml();
          break;
      }
      toast.success(`Exporting ${format.toUpperCase()} file...`);
    } catch (error) {
      toast.error('Export failed');
    }
  };

  const exportOptions = [
    {
      title: 'JSON Export',
      description: 'Download complete data as JSON for API integration',
      icon: FileJson,
      format: 'json' as const,
      color: 'primary',
    },
    {
      title: 'Excel Export',
      description: 'Download formatted Excel spreadsheet for analysis',
      icon: FileSpreadsheet,
      format: 'excel' as const,
      color: 'success',
    },
    {
      title: 'HTML Report',
      description: 'Generate formatted HTML report for sharing',
      icon: FileText,
      format: 'html' as const,
      color: 'info',
    },
  ];

  return (
    <div>
      <h2 className="mb-4">Export Data</h2>
      
      <Row className="g-4">
        {exportOptions.map((option) => {
          const Icon = option.icon;
          return (
            <Col key={option.format} md={4}>
              <Card className="border-0 shadow-sm h-100">
                <Card.Body className="d-flex flex-column">
                  <div className="mb-3">
                    <Icon size={48} className={`text-${option.color}`} />
                  </div>
                  <Card.Title>{option.title}</Card.Title>
                  <Card.Text className="flex-grow-1">{option.description}</Card.Text>
                  <Button
                    variant={option.color}
                    onClick={() => handleExport(option.format)}
                  >
                    <Download size={20} className="me-2" />
                    Export {option.format.toUpperCase()}
                  </Button>
                </Card.Body>
              </Card>
            </Col>
          );
        })}
      </Row>

      <Card className="border-0 shadow-sm mt-4">
        <Card.Body>
          <Card.Title>Export Information</Card.Title>
          <ul>
            <li>All exports include complete RBAC data</li>
            <li>Exports are timestamped automatically</li>
            <li>No sensitive data (passwords) included</li>
            <li>Data includes all relationships and assignments</li>
          </ul>
        </Card.Body>
      </Card>
    </div>
  );
};

export default Export;

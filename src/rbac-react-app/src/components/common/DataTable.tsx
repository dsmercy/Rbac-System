import React from 'react';
import { Table, Button, Badge } from 'react-bootstrap';
import { Edit, Trash2 } from 'lucide-react';

export interface Column<T> {
  header: string;
  accessor: keyof T | ((row: T) => any);
  render?: (value: any, row: T) => React.ReactNode;
}

interface DataTableProps<T> {
  data: T[];
  columns: Column<T>[];
  onEdit?: (row: T) => void;
  onDelete?: (row: T) => void;
  loading?: boolean;
  keyExtractor: (row: T) => string | number;
}

function DataTable<T>({
  data,
  columns,
  onEdit,
  onDelete,
  loading,
  keyExtractor,
}: DataTableProps<T>) {
  const getValue = (row: T, accessor: keyof T | ((row: T) => any)) => {
    if (typeof accessor === 'function') {
      return accessor(row);
    }
    return row[accessor];
  };

  if (loading) {
    return (
      <div className="text-center p-4">
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  if (data.length === 0) {
    return (
      <div className="text-center p-4 text-muted">
        No data available
      </div>
    );
  }

  return (
    <div className="table-responsive">
      <Table hover striped>
        <thead className="table-light">
          <tr>
            {columns.map((column, index) => (
              <th key={index}>{column.header}</th>
            ))}
            {(onEdit || onDelete) && <th className="text-end">Actions</th>}
          </tr>
        </thead>
        <tbody>
          {data.map((row) => (
            <tr key={keyExtractor(row)}>
              {columns.map((column, colIndex) => {
                const value = getValue(row, column.accessor);
                return (
                  <td key={colIndex}>
                    {column.render ? column.render(value, row) : value}
                  </td>
                );
              })}
              {(onEdit || onDelete) && (
                <td className="text-end">
                  {onEdit && (
                    <Button
                      variant="link"
                      size="sm"
                      className="text-primary"
                      onClick={() => onEdit(row)}
                    >
                      <Edit size={16} />
                    </Button>
                  )}
                  {onDelete && (
                    <Button
                      variant="link"
                      size="sm"
                      className="text-danger"
                      onClick={() => onDelete(row)}
                    >
                      <Trash2 size={16} />
                    </Button>
                  )}
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
  );
}

export default DataTable;

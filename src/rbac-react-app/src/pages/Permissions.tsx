import React, { useEffect, useState } from 'react';
import { Card, Button, Modal, Form, InputGroup } from 'react-bootstrap';
import { Plus, Search } from 'lucide-react';
import DataTable, { Column } from '@/components/common/DataTable';
import Pagination from '@/components/common/Pagination';
import { Permission, CreatePermissionDto, PermissionFilterParams } from '@/types';
import { permissionService } from '@/services/permissionService';
import toast from 'react-hot-toast';

const Permissions: React.FC = () => {
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [loading, setLoading] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [editingPermission, setEditingPermission] = useState<Permission | null>(null);
  const [filters, setFilters] = useState<PermissionFilterParams>({ pageNumber: 1, pageSize: 10 });
  const [pagination, setPagination] = useState<any>(null);
  const [formData, setFormData] = useState<CreatePermissionDto>({ permissionName: '', description: '' });

  useEffect(() => {
    loadPermissions();
  }, [filters]);

  const loadPermissions = async () => {
    setLoading(true);
    try {
      const response = await permissionService.getPermissions(filters);
      if (response.success && response.data) {
        setPermissions(response.data);
        setPagination(response.pagination);
      }
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async () => {
    try {
      const response = await permissionService.createPermission(formData);
      if (response.success) {
        toast.success('Permission created');
        setShowModal(false);
        loadPermissions();
      }
    } catch (error) {
      toast.error('Failed to create permission');
    }
  };

  const handleUpdate = async () => {
    if (!editingPermission) return;
    try {
      const response = await permissionService.updatePermission(editingPermission.permissionId, formData);
      if (response.success) {
        toast.success('Permission updated');
        setShowModal(false);
        loadPermissions();
      }
    } catch (error) {
      toast.error('Failed to update permission');
    }
  };

  const handleDelete = async (permission: Permission) => {
    if (!window.confirm(`Delete ${permission.permissionName}?`)) return;
    try {
      const response = await permissionService.deletePermission(permission.permissionId);
      if (response.success) {
        toast.success('Permission deleted');
        loadPermissions();
      }
    } catch (error) {
      toast.error('Failed to delete permission');
    }
  };

  const columns: Column<Permission>[] = [
    { header: 'ID', accessor: 'permissionId' },
    { header: 'Permission Name', accessor: 'permissionName' },
    { header: 'Description', accessor: 'description' },
  ];

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>Permissions Management</h2>
        <Button variant="primary" onClick={() => setShowModal(true)}>
          <Plus size={20} className="me-2" />
          Add Permission
        </Button>
      </div>

      <Card className="border-0 shadow-sm">
        <Card.Body>
          <div className="mb-3">
            <InputGroup>
              <InputGroup.Text><Search size={20} /></InputGroup.Text>
              <Form.Control
                placeholder="Search permissions..."
                onChange={(e) => setFilters({ ...filters, permissionName: e.target.value || undefined })}
              />
            </InputGroup>
          </div>

          <DataTable
            data={permissions}
            columns={columns}
            loading={loading}
            onEdit={(p) => { setEditingPermission(p); setFormData({ permissionName: p.permissionName, description: p.description }); setShowModal(true); }}
            onDelete={handleDelete}
            keyExtractor={(p) => p.permissionId}
          />

          {pagination && <Pagination metadata={pagination} onPageChange={(page) => setFilters({ ...filters, pageNumber: page })} />}
        </Card.Body>
      </Card>

      <Modal show={showModal} onHide={() => setShowModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>{editingPermission ? 'Edit' : 'Create'} Permission</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group className="mb-3">
              <Form.Label>Permission Name</Form.Label>
              <Form.Control value={formData.permissionName} onChange={(e) => setFormData({ ...formData, permissionName: e.target.value })} />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Description</Form.Label>
              <Form.Control as="textarea" rows={3} value={formData.description} onChange={(e) => setFormData({ ...formData, description: e.target.value })} />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowModal(false)}>Cancel</Button>
          <Button variant="primary" onClick={editingPermission ? handleUpdate : handleCreate}>
            {editingPermission ? 'Update' : 'Create'}
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};

export default Permissions;
import React, { useEffect, useState } from 'react';
import { Card, Button, Modal, Form, InputGroup, Badge } from 'react-bootstrap';
import { Plus, Search } from 'lucide-react';
import DataTable, { Column } from '@/components/common/DataTable';
import Pagination from '@/components/common/Pagination';
import { Role, CreateRoleDto, UpdateRoleDto, RoleFilterParams } from '@/types';
import { roleService } from '@/services/roleService';
import toast from 'react-hot-toast';

const Roles: React.FC = () => {
  const [roles, setRoles] = useState<Role[]>([]);
  const [loading, setLoading] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [editingRole, setEditingRole] = useState<Role | null>(null);
  const [filters, setFilters] = useState<RoleFilterParams>({ pageNumber: 1, pageSize: 10 });
  const [pagination, setPagination] = useState<any>(null);
  const [formData, setFormData] = useState<CreateRoleDto>({ roleName: '', description: '' });

  useEffect(() => {
    loadRoles();
  }, [filters]);

  const loadRoles = async () => {
    setLoading(true);
    try {
      const response = await roleService.getRoles(filters);
      if (response.success && response.data) {
        setRoles(response.data);
        setPagination(response.pagination);
      }
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async () => {
    try {
      const response = await roleService.createRole(formData);
      if (response.success) {
        toast.success('Role created successfully');
        setShowModal(false);
        resetForm();
        loadRoles();
      }
    } catch (error) {
      toast.error('Failed to create role');
    }
  };

  const handleUpdate = async () => {
    if (!editingRole) return;
    try {
      const response = await roleService.updateRole(editingRole.roleId, formData);
      if (response.success) {
        toast.success('Role updated successfully');
        setShowModal(false);
        resetForm();
        loadRoles();
      }
    } catch (error) {
      toast.error('Failed to update role');
    }
  };

  const handleDelete = async (role: Role) => {
    if (!window.confirm(`Delete ${role.roleName}?`)) return;
    try {
      const response = await roleService.deleteRole(role.roleId);
      if (response.success) {
        toast.success('Role deleted successfully');
        loadRoles();
      }
    } catch (error) {
      toast.error('Failed to delete role');
    }
  };

  const resetForm = () => {
    setFormData({ roleName: '', description: '' });
    setEditingRole(null);
  };

  const columns: Column<Role>[] = [
    { header: 'ID', accessor: 'roleId' },
    { header: 'Role Name', accessor: 'roleName' },
    { header: 'Description', accessor: 'description' },
    {
      header: 'Permissions',
      accessor: (row) => row.permissions?.length || 0,
      render: (value) => <Badge bg="info">{value} permissions</Badge>,
    },
    {
      header: 'Created At',
      accessor: 'createdAt',
      render: (value) => new Date(value).toLocaleDateString(),
    },
  ];

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>Roles Management</h2>
        <Button variant="primary" onClick={() => setShowModal(true)}>
          <Plus size={20} className="me-2" />
          Add Role
        </Button>
      </div>

      <Card className="border-0 shadow-sm">
        <Card.Body>
          <div className="mb-3">
            <InputGroup>
              <InputGroup.Text><Search size={20} /></InputGroup.Text>
              <Form.Control
                placeholder="Search by role name..."
                onChange={(e) => setFilters({ ...filters, roleName: e.target.value || undefined })}
              />
            </InputGroup>
          </div>

          <DataTable
            data={roles}
            columns={columns}
            loading={loading}
            onEdit={(r) => { setEditingRole(r); setFormData({ roleName: r.roleName, description: r.description }); setShowModal(true); }}
            onDelete={handleDelete}
            keyExtractor={(r) => r.roleId}
          />

          {pagination && (
            <Pagination metadata={pagination} onPageChange={(page) => setFilters({ ...filters, pageNumber: page })} />
          )}
        </Card.Body>
      </Card>

      <Modal show={showModal} onHide={() => { setShowModal(false); resetForm(); }}>
        <Modal.Header closeButton>
          <Modal.Title>{editingRole ? 'Edit Role' : 'Create Role'}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group className="mb-3">
              <Form.Label>Role Name</Form.Label>
              <Form.Control
                type="text"
                value={formData.roleName}
                onChange={(e) => setFormData({ ...formData, roleName: e.target.value })}
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Description</Form.Label>
              <Form.Control
                as="textarea"
                rows={3}
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => { setShowModal(false); resetForm(); }}>Cancel</Button>
          <Button variant="primary" onClick={editingRole ? handleUpdate : handleCreate}>
            {editingRole ? 'Update' : 'Create'}
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};

export default Roles;

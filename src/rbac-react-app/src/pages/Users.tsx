import React, { useEffect, useState } from 'react';
import { Card, Button, Modal, Form, Badge, InputGroup } from 'react-bootstrap';
import { Plus, Search } from 'lucide-react';
import DataTable, { Column } from '@/components/common/DataTable';
import Pagination from '@/components/common/Pagination';
import { User, CreateUserDto, UpdateUserDto, UserFilterParams } from '@/types';
import { userService } from '@/services/userService';
import toast from 'react-hot-toast';

const Users: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [editingUser, setEditingUser] = useState<User | null>(null);
  const [filters, setFilters] = useState<UserFilterParams>({
    pageNumber: 1,
    pageSize: 10,
  });
  const [pagination, setPagination] = useState<any>(null);
  const [formData, setFormData] = useState<CreateUserDto>({
    username: '',
    email: '',
    isActive: true,
  });

  useEffect(() => {
    loadUsers();
  }, [filters]);

  const loadUsers = async () => {
    setLoading(true);
    try {
      const response = await userService.getUsers(filters);
      if (response.success && response.data) {
        setUsers(response.data);
        setPagination(response.pagination);
      } else {
        toast.error(response.message);
      }
    } catch (error) {
      toast.error('Failed to load users');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async () => {
    try {
      const response = await userService.createUser(formData);
      if (response.success) {
        toast.success('User created successfully');
        setShowModal(false);
        resetForm();
        loadUsers();
      } else {
        toast.error(response.message);
      }
    } catch (error) {
      toast.error('Failed to create user');
    }
  };

  const handleUpdate = async () => {
    if (!editingUser) return;
    
    try {
      const updateData: UpdateUserDto = {
        username: formData.username,
        email: formData.email,
        isActive: formData.isActive,
      };
      
      const response = await userService.updateUser(editingUser.userId, updateData);
      if (response.success) {
        toast.success('User updated successfully');
        setShowModal(false);
        resetForm();
        loadUsers();
      } else {
        toast.error(response.message);
      }
    } catch (error) {
      toast.error('Failed to update user');
    }
  };

  const handleDelete = async (user: User) => {
    if (!window.confirm(`Are you sure you want to delete ${user.username}?`)) {
      return;
    }

    try {
      const response = await userService.deleteUser(user.userId);
      if (response.success) {
        toast.success('User deleted successfully');
        loadUsers();
      } else {
        toast.error(response.message);
      }
    } catch (error) {
      toast.error('Failed to delete user');
    }
  };

  const openCreateModal = () => {
    resetForm();
    setEditingUser(null);
    setShowModal(true);
  };

  const openEditModal = (user: User) => {
    setEditingUser(user);
    setFormData({
      username: user.username,
      email: user.email,
      isActive: user.isActive,
    });
    setShowModal(true);
  };

  const resetForm = () => {
    setFormData({
      username: '',
      email: '',
      isActive: true,
    });
    setEditingUser(null);
  };

  const handleSearch = (searchTerm: string) => {
    setFilters({
      ...filters,
      username: searchTerm || undefined,
      pageNumber: 1,
    });
  };

  const columns: Column<User>[] = [
    { header: 'ID', accessor: 'userId' },
    { header: 'Username', accessor: 'username' },
    { header: 'Email', accessor: 'email' },
    {
      header: 'Status',
      accessor: 'isActive',
      render: (value) => (
        <Badge bg={value ? 'success' : 'danger'}>
          {value ? 'Active' : 'Inactive'}
        </Badge>
      ),
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
        <h2>Users Management</h2>
        <Button variant="primary" onClick={openCreateModal}>
          <Plus size={20} className="me-2" />
          Add User
        </Button>
      </div>

      <Card className="border-0 shadow-sm">
        <Card.Body>
          <div className="mb-3">
            <InputGroup>
              <InputGroup.Text>
                <Search size={20} />
              </InputGroup.Text>
              <Form.Control
                placeholder="Search by username..."
                onChange={(e) => handleSearch(e.target.value)}
              />
            </InputGroup>
          </div>

          <DataTable
            data={users}
            columns={columns}
            loading={loading}
            onEdit={openEditModal}
            onDelete={handleDelete}
            keyExtractor={(user) => user.userId}
          />

          {pagination && (
            <Pagination
              metadata={pagination}
              onPageChange={(page) => setFilters({ ...filters, pageNumber: page })}
            />
          )}
        </Card.Body>
      </Card>

      <Modal show={showModal} onHide={() => setShowModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>{editingUser ? 'Edit User' : 'Create User'}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group className="mb-3">
              <Form.Label>Username</Form.Label>
              <Form.Control
                type="text"
                value={formData.username}
                onChange={(e) => setFormData({ ...formData, username: e.target.value })}
                placeholder="Enter username"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Email</Form.Label>
              <Form.Control
                type="email"
                value={formData.email}
                onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                placeholder="Enter email"
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Check
                type="checkbox"
                label="Active"
                checked={formData.isActive}
                onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
              />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowModal(false)}>
            Cancel
          </Button>
          <Button variant="primary" onClick={editingUser ? handleUpdate : handleCreate}>
            {editingUser ? 'Update' : 'Create'}
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};

export default Users;

// Groups.tsx
import React, { useEffect, useState } from 'react';
import { Card, Button, Modal, Form, InputGroup } from 'react-bootstrap';
import { Plus, Search } from 'lucide-react';
import DataTable, { Column } from '@/components/common/DataTable';
import Pagination from '@/components/common/Pagination';
import { Group, CreateGroupDto, UpdateGroupDto, GroupFilterParams } from '@/types';
import { groupService } from '@/services/groupService';
import toast from 'react-hot-toast';

const Groups: React.FC = () => {
  const [groups, setGroups] = useState<Group[]>([]);
  const [loading, setLoading] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [editingGroup, setEditingGroup] = useState<Group | null>(null);
  const [filters, setFilters] = useState<GroupFilterParams>({
    pageNumber: 1,
    pageSize: 10,
  });
  const [pagination, setPagination] = useState<any>(null);
  const [formData, setFormData] = useState<CreateGroupDto>({
    groupName: '',
    description: '',
  });

  useEffect(() => {
    loadGroups();
  }, [filters]);

  const loadGroups = async () => {
    setLoading(true);
    try {
      const response = await groupService.getGroups(filters);
      if (response.success && response.data) {
        setGroups(response.data);
        setPagination(response.pagination);
      }
    } catch (error) {
      toast.error('Failed to load groups');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async () => {
    try {
      const response = await groupService.createGroup(formData);
      if (response.success) {
        toast.success('Group created successfully');
        setShowModal(false);
        resetForm();
        loadGroups();
      } else {
        toast.error(response.message);
      }
    } catch (error) {
      toast.error('Failed to create group');
    }
  };

  const handleUpdate = async () => {
    if (!editingGroup) return;
    try {
      const response = await groupService.updateGroup(editingGroup.groupId, formData);
      if (response.success) {
        toast.success('Group updated successfully');
        setShowModal(false);
        resetForm();
        loadGroups();
      }
    } catch (error) {
      toast.error('Failed to update group');
    }
  };

  const handleDelete = async (group: Group) => {
    if (!window.confirm(`Delete ${group.groupName}?`)) return;
    try {
      const response = await groupService.deleteGroup(group.groupId);
      if (response.success) {
        toast.success('Group deleted successfully');
        loadGroups();
      }
    } catch (error) {
      toast.error('Failed to delete group');
    }
  };

  const resetForm = () => {
    setFormData({ groupName: '', description: '' });
    setEditingGroup(null);
  };

  const columns: Column<Group>[] = [
    { header: 'ID', accessor: 'groupId' },
    { header: 'Group Name', accessor: 'groupName' },
    { header: 'Description', accessor: 'description' },
    {
      header: 'Created At',
      accessor: 'createdAt',
      render: (value) => new Date(value).toLocaleDateString(),
    },
  ];

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>Groups Management</h2>
        <Button variant="primary" onClick={() => setShowModal(true)}>
          <Plus size={20} className="me-2" />
          Add Group
        </Button>
      </div>

      <Card className="border-0 shadow-sm">
        <Card.Body>
          <div className="mb-3">
            <InputGroup>
              <InputGroup.Text><Search size={20} /></InputGroup.Text>
              <Form.Control
                placeholder="Search by group name..."
                onChange={(e) => setFilters({ ...filters, groupName: e.target.value || undefined })}
              />
            </InputGroup>
          </div>

          <DataTable
            data={groups}
            columns={columns}
            loading={loading}
            onEdit={(g) => { setEditingGroup(g); setFormData({ groupName: g.groupName, description: g.description }); setShowModal(true); }}
            onDelete={handleDelete}
            keyExtractor={(g) => g.groupId}
          />

          {pagination && (
            <Pagination
              metadata={pagination}
              onPageChange={(page) => setFilters({ ...filters, pageNumber: page })}
            />
          )}
        </Card.Body>
      </Card>

      <Modal show={showModal} onHide={() => { setShowModal(false); resetForm(); }}>
        <Modal.Header closeButton>
          <Modal.Title>{editingGroup ? 'Edit Group' : 'Create Group'}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group className="mb-3">
              <Form.Label>Group Name</Form.Label>
              <Form.Control
                type="text"
                value={formData.groupName}
                onChange={(e) => setFormData({ ...formData, groupName: e.target.value })}
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
          <Button variant="primary" onClick={editingGroup ? handleUpdate : handleCreate}>
            {editingGroup ? 'Update' : 'Create'}
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};

export default Groups;

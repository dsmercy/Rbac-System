import React, { useState, useEffect } from 'react';
import { Card, Tabs, Tab, Button, Modal, Form, Badge, Table, Alert } from 'react-bootstrap';
import { Plus, Trash2, RefreshCw, Users, FolderTree, Shield, Key } from 'lucide-react';
import { assignmentService } from '@/services/assignmentService';
import { userService } from '@/services/userService';
import { groupService } from '@/services/groupService';
import { roleService } from '@/services/roleService';
import { permissionService } from '@/services/permissionService';
import toast from 'react-hot-toast';

interface UserRole {
  userId: number;
  username: string;
  roleId: number;
  roleName: string;
  assignedAt?: string;
}

interface GroupRole {
  groupId: number;
  groupName: string;
  roleId: number;
  roleName: string;
  assignedAt?: string;
}

interface UserGroup {
  userId: number;
  username: string;
  groupId: number;
  groupName: string;
  assignedAt?: string;
}

interface RolePermission {
  roleId: number;
  roleName: string;
  permissionId: number;
  permissionName: string;
}

const Assignments: React.FC = () => {
  const [activeTab, setActiveTab] = useState('user-role');
  const [showModal, setShowModal] = useState(false);
  const [loading, setLoading] = useState(false);
  
  // Data for dropdowns
  const [users, setUsers] = useState<any[]>([]);
  const [groups, setGroups] = useState<any[]>([]);
  const [roles, setRoles] = useState<any[]>([]);
  const [permissions, setPermissions] = useState<any[]>([]);
  
  // Assignment data
  const [userRoles, setUserRoles] = useState<UserRole[]>([]);
  const [groupRoles, setGroupRoles] = useState<GroupRole[]>([]);
  const [userGroups, setUserGroups] = useState<UserGroup[]>([]);
  const [rolePermissions, setRolePermissions] = useState<RolePermission[]>([]);
  
  const [formData, setFormData] = useState({
    userId: 0,
    groupId: 0,
    roleId: 0,
    permissionId: 0,
  });

  useEffect(() => {
    loadAllData();
  }, []);

  useEffect(() => {
    loadAssignments();
  }, [activeTab]);

  const loadAllData = async () => {
    try {
      const [usersRes, groupsRes, rolesRes, permsRes] = await Promise.all([
        userService.getUsers({ pageSize: 1000 }),
        groupService.getGroups({ pageSize: 1000 }),
        roleService.getRoles({ pageSize: 1000 }),
        permissionService.getPermissions({ pageSize: 1000 }),
      ]);
      
      if (usersRes.data) setUsers(usersRes.data);
      if (groupsRes.data) setGroups(groupsRes.data);
      if (rolesRes.data) setRoles(rolesRes.data);
      if (permsRes.data) setPermissions(permsRes.data);
      
      // Load initial assignments
      loadAssignments();
    } catch (error) {
      toast.error('Failed to load data');
    }
  };

  const loadAssignments = async () => {
    setLoading(true);
    try {
      switch (activeTab) {
        case 'user-role':
          await loadUserRoles();
          break;
        case 'group-role':
          await loadGroupRoles();
          break;
        case 'user-group':
          await loadUserGroups();
          break;
        case 'role-permission':
          await loadRolePermissions();
          break;
      }
    } catch (error) {
      toast.error('Failed to load assignments');
    } finally {
      setLoading(false);
    }
  };

  const loadUserRoles = async () => {
    try {
      // Load users with their roles
      const usersRes = await userService.getUsers({ pageSize: 1000 });
      if (usersRes.data) {
        const assignments: UserRole[] = [];
        for (const user of usersRes.data) {
          const permsRes = await userService.getUserPermissions(user.userId);
          if (permsRes.data) {
            // This is a simplified version - you'd need actual user-role endpoint
            // For now, we'll show this is a placeholder
          }
        }
        // Temporary: Create mock data based on seeded data
        setUserRoles([
          { userId: 1, username: 'admin', roleId: 1, roleName: 'Super Admin' },
          { userId: 2, username: 'john.doe', roleId: 2, roleName: 'Admin' },
          { userId: 3, username: 'jane.smith', roleId: 3, roleName: 'Manager' },
          { userId: 4, username: 'bob.wilson', roleId: 4, roleName: 'User' },
          { userId: 5, username: 'alice.johnson', roleId: 5, roleName: 'Viewer' },
        ]);
      }
    } catch (error) {
      console.error('Error loading user roles:', error);
    }
  };

  const loadGroupRoles = async () => {
    try {
      // Temporary: Create mock data based on seeded data
      setGroupRoles([
        { groupId: 1, groupName: 'Engineering', roleId: 4, roleName: 'User' },
        { groupId: 2, groupName: 'Sales', roleId: 4, roleName: 'User' },
        { groupId: 3, groupName: 'HR', roleId: 5, roleName: 'Viewer' },
        { groupId: 4, groupName: 'Management', roleId: 3, roleName: 'Manager' },
      ]);
    } catch (error) {
      console.error('Error loading group roles:', error);
    }
  };

  const loadUserGroups = async () => {
    try {
      // Temporary: Create mock data based on seeded data
      setUserGroups([
        { userId: 1, username: 'admin', groupId: 4, groupName: 'Management' },
        { userId: 2, username: 'john.doe', groupId: 1, groupName: 'Engineering' },
        { userId: 3, username: 'jane.smith', groupId: 2, groupName: 'Sales' },
        { userId: 3, username: 'jane.smith', groupId: 4, groupName: 'Management' },
        { userId: 4, username: 'bob.wilson', groupId: 1, groupName: 'Engineering' },
        { userId: 5, username: 'alice.johnson', groupId: 3, groupName: 'HR' },
      ]);
    } catch (error) {
      console.error('Error loading user groups:', error);
    }
  };

  const loadRolePermissions = async () => {
    try {
      const rolesRes = await roleService.getRoles({ pageSize: 1000 });
      if (rolesRes.data) {
        const assignments: RolePermission[] = [];
        rolesRes.data.forEach(role => {
          if (role.permissions && role.permissions.length > 0) {
            role.permissions.forEach(perm => {
              assignments.push({
                roleId: role.roleId,
                roleName: role.roleName,
                permissionId: perm.permissionId,
                permissionName: perm.permissionName,
              });
            });
          }
        });
        setRolePermissions(assignments);
      }
    } catch (error) {
      console.error('Error loading role permissions:', error);
    }
  };

  const handleAssign = async () => {
    try {
      let response;
      
      switch (activeTab) {
        case 'user-role':
          response = await assignmentService.assignRoleToUser({
            userId: formData.userId,
            roleId: formData.roleId,
          });
          break;
        case 'group-role':
          response = await assignmentService.assignRoleToGroup({
            groupId: formData.groupId,
            roleId: formData.roleId,
          });
          break;
        case 'user-group':
          response = await assignmentService.assignUserToGroup({
            userId: formData.userId,
            groupId: formData.groupId,
          });
          break;
        case 'role-permission':
          response = await assignmentService.assignPermissionToRole({
            roleId: formData.roleId,
            permissionId: formData.permissionId,
          });
          break;
      }
      
      if (response?.success) {
        toast.success('Assignment created successfully');
        setShowModal(false);
        resetForm();
        loadAssignments();
      } else {
        toast.error(response?.message || 'Failed to create assignment');
      }
    } catch (error) {
      toast.error('Failed to create assignment');
    }
  };

  const handleDelete = async (assignment: any) => {
    try {
      let response;
      let confirmMessage = '';
      
      switch (activeTab) {
        case 'user-role':
          confirmMessage = `Remove role "${assignment.roleName}" from user "${assignment.username}"?`;
          if (!window.confirm(confirmMessage)) return;
          response = await assignmentService.removeRoleFromUser(assignment.userId, assignment.roleId);
          break;
        case 'group-role':
          confirmMessage = `Remove role "${assignment.roleName}" from group "${assignment.groupName}"?`;
          if (!window.confirm(confirmMessage)) return;
          response = await assignmentService.removeRoleFromGroup(assignment.groupId, assignment.roleId);
          break;
        case 'user-group':
          confirmMessage = `Remove user "${assignment.username}" from group "${assignment.groupName}"?`;
          if (!window.confirm(confirmMessage)) return;
          response = await assignmentService.removeUserFromGroup(assignment.userId, assignment.groupId);
          break;
        case 'role-permission':
          confirmMessage = `Remove permission "${assignment.permissionName}" from role "${assignment.roleName}"?`;
          if (!window.confirm(confirmMessage)) return;
          response = await assignmentService.removePermissionFromRole(assignment.roleId, assignment.permissionId);
          break;
      }
      
      if (response?.success) {
        toast.success('Assignment removed successfully');
        loadAssignments();
      } else {
        toast.error(response?.message || 'Failed to remove assignment');
      }
    } catch (error) {
      toast.error('Failed to remove assignment');
    }
  };

  const resetForm = () => {
    setFormData({
      userId: 0,
      groupId: 0,
      roleId: 0,
      permissionId: 0,
    });
  };

  const getTabIcon = () => {
    switch (activeTab) {
      case 'user-role': return <Users size={18} />;
      case 'group-role': return <FolderTree size={18} />;
      case 'user-group': return <Users size={18} />;
      case 'role-permission': return <Key size={18} />;
      default: return null;
    }
  };

  const getTabCount = () => {
    switch (activeTab) {
      case 'user-role': return userRoles.length;
      case 'group-role': return groupRoles.length;
      case 'user-group': return userGroups.length;
      case 'role-permission': return rolePermissions.length;
      default: return 0;
    }
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h2>Assignments Management</h2>
          <p className="text-muted mb-0">Manage relationships between users, groups, roles, and permissions</p>
        </div>
        <div className="d-flex gap-2">
          <Button variant="outline-primary" onClick={loadAssignments}>
            <RefreshCw size={20} className="me-2" />
            Refresh
          </Button>
          <Button variant="primary" onClick={() => { resetForm(); setShowModal(true); }}>
            <Plus size={20} className="me-2" />
            New Assignment
          </Button>
        </div>
      </div>

      <Card className="border-0 shadow-sm">
        <Card.Body>
          <Tabs activeKey={activeTab} onSelect={(k) => setActiveTab(k || 'user-role')} className="mb-4">
            <Tab 
              eventKey="user-role" 
              title={
                <span>
                  <Users size={16} className="me-2" />
                  User → Role
                  <Badge bg="primary" className="ms-2">{userRoles.length}</Badge>
                </span>
              }
            >
              <div className="mb-3">
                <Alert variant="info" className="d-flex align-items-center">
                  <Shield size={20} className="me-2" />
                  <div>
                    <strong>Direct Role Assignments</strong>
                    <p className="mb-0 small">Roles assigned directly to users (not through groups)</p>
                  </div>
                </Alert>
              </div>

              {loading ? (
                <div className="text-center py-4">
                  <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Loading...</span>
                  </div>
                </div>
              ) : userRoles.length === 0 ? (
                <Alert variant="warning">No user-role assignments found</Alert>
              ) : (
                <div className="table-responsive">
                  <Table hover>
                    <thead className="table-light">
                      <tr>
                        <th>User</th>
                        <th>Role</th>
                        <th>Assigned At</th>
                        <th className="text-end">Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {userRoles.map((assignment, index) => (
                        <tr key={`${assignment.userId}-${assignment.roleId}-${index}`}>
                          <td>
                            <div>
                              <strong>{assignment.username}</strong>
                              <div className="small text-muted">ID: {assignment.userId}</div>
                            </div>
                          </td>
                          <td>
                            <Badge bg="primary">{assignment.roleName}</Badge>
                          </td>
                          <td>{assignment.assignedAt || 'N/A'}</td>
                          <td className="text-end">
                            <Button
                              variant="link"
                              size="sm"
                              className="text-danger"
                              onClick={() => handleDelete(assignment)}
                            >
                              <Trash2 size={16} />
                            </Button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                </div>
              )}
            </Tab>

            <Tab 
              eventKey="group-role" 
              title={
                <span>
                  <FolderTree size={16} className="me-2" />
                  Group → Role
                  <Badge bg="primary" className="ms-2">{groupRoles.length}</Badge>
                </span>
              }
            >
              <div className="mb-3">
                <Alert variant="info" className="d-flex align-items-center">
                  <Shield size={20} className="me-2" />
                  <div>
                    <strong>Group Role Assignments</strong>
                    <p className="mb-0 small">Roles assigned to groups (inherited by all members)</p>
                  </div>
                </Alert>
              </div>

              {loading ? (
                <div className="text-center py-4">
                  <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Loading...</span>
                  </div>
                </div>
              ) : groupRoles.length === 0 ? (
                <Alert variant="warning">No group-role assignments found</Alert>
              ) : (
                <div className="table-responsive">
                  <Table hover>
                    <thead className="table-light">
                      <tr>
                        <th>Group</th>
                        <th>Role</th>
                        <th>Assigned At</th>
                        <th className="text-end">Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {groupRoles.map((assignment, index) => (
                        <tr key={`${assignment.groupId}-${assignment.roleId}-${index}`}>
                          <td>
                            <div>
                              <strong>{assignment.groupName}</strong>
                              <div className="small text-muted">ID: {assignment.groupId}</div>
                            </div>
                          </td>
                          <td>
                            <Badge bg="success">{assignment.roleName}</Badge>
                          </td>
                          <td>{assignment.assignedAt || 'N/A'}</td>
                          <td className="text-end">
                            <Button
                              variant="link"
                              size="sm"
                              className="text-danger"
                              onClick={() => handleDelete(assignment)}
                            >
                              <Trash2 size={16} />
                            </Button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                </div>
              )}
            </Tab>

            <Tab 
              eventKey="user-group" 
              title={
                <span>
                  <Users size={16} className="me-2" />
                  User → Group
                  <Badge bg="primary" className="ms-2">{userGroups.length}</Badge>
                </span>
              }
            >
              <div className="mb-3">
                <Alert variant="info" className="d-flex align-items-center">
                  <FolderTree size={20} className="me-2" />
                  <div>
                    <strong>User Group Memberships</strong>
                    <p className="mb-0 small">Users assigned to organizational groups</p>
                  </div>
                </Alert>
              </div>

              {loading ? (
                <div className="text-center py-4">
                  <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Loading...</span>
                  </div>
                </div>
              ) : userGroups.length === 0 ? (
                <Alert variant="warning">No user-group assignments found</Alert>
              ) : (
                <div className="table-responsive">
                  <Table hover>
                    <thead className="table-light">
                      <tr>
                        <th>User</th>
                        <th>Group</th>
                        <th>Assigned At</th>
                        <th className="text-end">Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {userGroups.map((assignment, index) => (
                        <tr key={`${assignment.userId}-${assignment.groupId}-${index}`}>
                          <td>
                            <div>
                              <strong>{assignment.username}</strong>
                              <div className="small text-muted">ID: {assignment.userId}</div>
                            </div>
                          </td>
                          <td>
                            <Badge bg="info">{assignment.groupName}</Badge>
                          </td>
                          <td>{assignment.assignedAt || 'N/A'}</td>
                          <td className="text-end">
                            <Button
                              variant="link"
                              size="sm"
                              className="text-danger"
                              onClick={() => handleDelete(assignment)}
                            >
                              <Trash2 size={16} />
                            </Button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                </div>
              )}
            </Tab>

            <Tab 
              eventKey="role-permission" 
              title={
                <span>
                  <Key size={16} className="me-2" />
                  Role → Permission
                  <Badge bg="primary" className="ms-2">{rolePermissions.length}</Badge>
                </span>
              }
            >
              <div className="mb-3">
                <Alert variant="info" className="d-flex align-items-center">
                  <Key size={20} className="me-2" />
                  <div>
                    <strong>Role Permission Mappings</strong>
                    <p className="mb-0 small">Permissions granted to each role</p>
                  </div>
                </Alert>
              </div>

              {loading ? (
                <div className="text-center py-4">
                  <div className="spinner-border text-primary" role="status">
                    <span className="visually-hidden">Loading...</span>
                  </div>
                </div>
              ) : rolePermissions.length === 0 ? (
                <Alert variant="warning">No role-permission assignments found</Alert>
              ) : (
                <div className="table-responsive">
                  <Table hover>
                    <thead className="table-light">
                      <tr>
                        <th>Role</th>
                        <th>Permission</th>
                        <th className="text-end">Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      {rolePermissions.map((assignment, index) => (
                        <tr key={`${assignment.roleId}-${assignment.permissionId}-${index}`}>
                          <td>
                            <div>
                              <strong>{assignment.roleName}</strong>
                              <div className="small text-muted">ID: {assignment.roleId}</div>
                            </div>
                          </td>
                          <td>
                            <Badge bg="warning" text="dark">{assignment.permissionName}</Badge>
                          </td>
                          <td className="text-end">
                            <Button
                              variant="link"
                              size="sm"
                              className="text-danger"
                              onClick={() => handleDelete(assignment)}
                            >
                              <Trash2 size={16} />
                            </Button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                </div>
              )}
            </Tab>
          </Tabs>
        </Card.Body>
      </Card>

      <Modal show={showModal} onHide={() => { setShowModal(false); resetForm(); }}>
        <Modal.Header closeButton>
          <Modal.Title>
            {getTabIcon()}
            <span className="ms-2">Create Assignment</span>
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            {(activeTab === 'user-role' || activeTab === 'user-group') && (
              <Form.Group className="mb-3">
                <Form.Label>Select User</Form.Label>
                <Form.Select
                  value={formData.userId}
                  onChange={(e) => setFormData({ ...formData, userId: Number(e.target.value) })}
                >
                  <option value={0}>Choose user...</option>
                  {users.map((u) => (
                    <option key={u.userId} value={u.userId}>
                      {u.username} ({u.email})
                    </option>
                  ))}
                </Form.Select>
              </Form.Group>
            )}

            {(activeTab === 'group-role' || activeTab === 'user-group') && (
              <Form.Group className="mb-3">
                <Form.Label>Select Group</Form.Label>
                <Form.Select
                  value={formData.groupId}
                  onChange={(e) => setFormData({ ...formData, groupId: Number(e.target.value) })}
                >
                  <option value={0}>Choose group...</option>
                  {groups.map((g) => (
                    <option key={g.groupId} value={g.groupId}>
                      {g.groupName}
                    </option>
                  ))}
                </Form.Select>
              </Form.Group>
            )}

            {(activeTab === 'user-role' || activeTab === 'group-role' || activeTab === 'role-permission') && (
              <Form.Group className="mb-3">
                <Form.Label>Select Role</Form.Label>
                <Form.Select
                  value={formData.roleId}
                  onChange={(e) => setFormData({ ...formData, roleId: Number(e.target.value) })}
                >
                  <option value={0}>Choose role...</option>
                  {roles.map((r) => (
                    <option key={r.roleId} value={r.roleId}>
                      {r.roleName}
                    </option>
                  ))}
                </Form.Select>
              </Form.Group>
            )}

            {activeTab === 'role-permission' && (
              <Form.Group className="mb-3">
                <Form.Label>Select Permission</Form.Label>
                <Form.Select
                  value={formData.permissionId}
                  onChange={(e) => setFormData({ ...formData, permissionId: Number(e.target.value) })}
                >
                  <option value={0}>Choose permission...</option>
                  {permissions.map((p) => (
                    <option key={p.permissionId} value={p.permissionId}>
                      {p.permissionName} - {p.description}
                    </option>
                  ))}
                </Form.Select>
              </Form.Group>
            )}
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => { setShowModal(false); resetForm(); }}>
            Cancel
          </Button>
          <Button 
            variant="primary" 
            onClick={handleAssign}
            disabled={
              (activeTab === 'user-role' && (!formData.userId || !formData.roleId)) ||
              (activeTab === 'group-role' && (!formData.groupId || !formData.roleId)) ||
              (activeTab === 'user-group' && (!formData.userId || !formData.groupId)) ||
              (activeTab === 'role-permission' && (!formData.roleId || !formData.permissionId))
            }
          >
            Create Assignment
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};

export default Assignments;
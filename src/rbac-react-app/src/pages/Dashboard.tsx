import React, { useEffect, useState } from 'react';
import { Row, Col, Card } from 'react-bootstrap';
import { Users, FolderTree, Shield, Key } from 'lucide-react';
import { userService } from '@/services/userService';
import { groupService } from '@/services/groupService';
import { roleService } from '@/services/roleService';
import { permissionService } from '@/services/permissionService';
import toast from 'react-hot-toast';

const Dashboard: React.FC = () => {
  const [stats, setStats] = useState({
    users: 0,
    groups: 0,
    roles: 0,
    permissions: 0,
  });

  useEffect(() => {
    loadStats();
  }, []);

  const loadStats = async () => {
    try {
      const [usersRes, groupsRes, rolesRes, permissionsRes] = await Promise.all([
        userService.getUsers({ pageSize: 1 }),
        groupService.getGroups({ pageSize: 1 }),
        roleService.getRoles({ pageSize: 1 }),
        permissionService.getPermissions({ pageSize: 1 }),
      ]);

      setStats({
        users: usersRes.pagination?.totalCount || 0,
        groups: groupsRes.pagination?.totalCount || 0,
        roles: rolesRes.pagination?.totalCount || 0,
        permissions: permissionsRes.pagination?.totalCount || 0,
      });
    } catch (error) {
      toast.error('Failed to load statistics');
    }
  };

  const statCards = [
    {
      title: 'Total Users',
      value: stats.users,
      icon: Users,
      color: 'primary',
      gradient: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    },
    {
      title: 'User Groups',
      value: stats.groups,
      icon: FolderTree,
      color: 'success',
      gradient: 'linear-gradient(135deg, #10b981 0%, #059669 100%)',
    },
    {
      title: 'Roles',
      value: stats.roles,
      icon: Shield,
      color: 'info',
      gradient: 'linear-gradient(135deg, #3b82f6 0%, #2563eb 100%)',
    },
    {
      title: 'Permissions',
      value: stats.permissions,
      icon: Key,
      color: 'warning',
      gradient: 'linear-gradient(135deg, #f59e0b 0%, #d97706 100%)',
    },
  ];

  return (
    <div>
      <h2 className="mb-4">Dashboard</h2>
      
      <Row className="g-4">
        {statCards.map((stat) => {
          const Icon = stat.icon;
          return (
            <Col key={stat.title} xs={12} sm={6} lg={3}>
              <Card className="border-0 shadow-sm h-100">
                <Card.Body>
                  <div className="d-flex justify-content-between align-items-start">
                    <div>
                      <p className="text-muted mb-1">{stat.title}</p>
                      <h3 className="mb-0">{stat.value}</h3>
                    </div>
                    <div
                      className="rounded p-3"
                      style={{ background: stat.gradient }}
                    >
                      <Icon size={24} className="text-white" />
                    </div>
                  </div>
                </Card.Body>
              </Card>
            </Col>
          );
        })}
      </Row>

      <Row className="mt-4">
        <Col md={12}>
          <Card className="border-0 shadow-sm">
            <Card.Body>
              <Card.Title>Welcome to RBAC System</Card.Title>
              <Card.Text>
                Manage your users, roles, permissions, and groups from this centralized dashboard.
                Use the navigation menu to access different sections of the system.
              </Card.Text>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default Dashboard;

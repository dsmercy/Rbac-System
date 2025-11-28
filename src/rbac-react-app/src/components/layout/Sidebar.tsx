import React from 'react';
import { Nav } from 'react-bootstrap';
import { Link, useLocation } from 'react-router-dom';
import {
  LayoutDashboard,
  Users,
  FolderTree,
  Shield,
  Key,
  Link as LinkIcon,
  Download,
} from 'lucide-react';
import { useUIStore } from '@/store/uiStore';
import './Sidebar.css';

const Sidebar: React.FC = () => {
  const location = useLocation();
  const sidebarOpen = useUIStore((state) => state.sidebarOpen);

  const menuItems = [
    { path: '/', icon: LayoutDashboard, label: 'Dashboard' },
    { path: '/users', icon: Users, label: 'Users' },
    { path: '/groups', icon: FolderTree, label: 'Groups' },
    { path: '/roles', icon: Shield, label: 'Roles' },
    { path: '/permissions', icon: Key, label: 'Permissions' },
    { path: '/assignments', icon: LinkIcon, label: 'Assignments' },
    { path: '/export', icon: Download, label: 'Export Data' },
  ];

  const isActive = (path: string) => {
    if (path === '/') {
      return location.pathname === '/';
    }
    return location.pathname.startsWith(path);
  };

  return (
    <div className={`sidebar bg-dark text-white ${sidebarOpen ? 'open' : 'closed'}`}>
      <div className="sidebar-header p-3 border-bottom border-secondary">
        <h4 className="mb-0">
          {sidebarOpen ? 'RBAC System' : 'RBAC'}
        </h4>
      </div>
      <Nav className="flex-column p-2">
        {menuItems.map((item) => {
          const Icon = item.icon;
          return (
            <Nav.Link
              key={item.path}
              as={Link}
              to={item.path}
              className={`sidebar-item d-flex align-items-center gap-3 px-3 py-2 rounded ${
                isActive(item.path) ? 'active' : ''
              }`}
            >
              <Icon size={20} />
              {sidebarOpen && <span>{item.label}</span>}
            </Nav.Link>
          );
        })}
      </Nav>
    </div>
  );
};

export default Sidebar;

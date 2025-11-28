import React from 'react';
import { Navbar as BSNavbar, Container, Button, Dropdown } from 'react-bootstrap';
import { Menu, User, LogOut } from 'lucide-react';
import { useAuthStore } from '@/store/authStore';
import { useUIStore } from '@/store/uiStore';
import { useNavigate } from 'react-router-dom';

const Navbar: React.FC = () => {
  const user = useAuthStore((state) => state.user);
  const logout = useAuthStore((state) => state.logout);
  const toggleSidebar = useUIStore((state) => state.toggleSidebar);
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <BSNavbar bg="white" className="border-bottom shadow-sm">
      <Container fluid>
        <Button
          variant="link"
          className="text-dark"
          onClick={toggleSidebar}
        >
          <Menu size={24} />
        </Button>

        <BSNavbar.Collapse className="justify-content-end">
          <Dropdown align="end">
            <Dropdown.Toggle variant="link" className="text-dark text-decoration-none">
              <User size={20} className="me-2" />
              {user?.username || 'User'}
            </Dropdown.Toggle>

            <Dropdown.Menu>
              <Dropdown.Item disabled>
                <small className="text-muted">{user?.email}</small>
              </Dropdown.Item>
              <Dropdown.Divider />
              <Dropdown.Item onClick={handleLogout}>
                <LogOut size={16} className="me-2" />
                Logout
              </Dropdown.Item>
            </Dropdown.Menu>
          </Dropdown>
        </BSNavbar.Collapse>
      </Container>
    </BSNavbar>
  );
};

export default Navbar;

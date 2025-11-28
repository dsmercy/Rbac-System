import React, { Suspense } from 'react';
import { Outlet, Navigate } from 'react-router-dom';
import { Container } from 'react-bootstrap';
import Sidebar from '@/components/layout/Sidebar';
import Navbar from '@/components/layout/Navbar';
import { useAuthStore } from '@/store/authStore';
import { useUIStore } from '@/store/uiStore';
import LoadingSpinner from '@/components/common/LoadingSpinner';

const MainLayout: React.FC = () => {
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
  const sidebarOpen = useUIStore((state) => state.sidebarOpen);

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return (
    <div className="d-flex vh-100">
      <Sidebar />
      <div className={`flex-grow-1 d-flex flex-column ${sidebarOpen ? 'ms-0' : ''}`}>
        <Navbar />
        <main className="flex-grow-1 overflow-auto bg-light">
          <Container fluid className="py-4">
            <Suspense fallback={<LoadingSpinner />}>
              <Outlet />
            </Suspense>
          </Container>
        </main>
      </div>
    </div>
  );
};

export default MainLayout;

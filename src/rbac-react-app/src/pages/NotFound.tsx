import React from 'react';
import { Container, Button } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { Home } from 'lucide-react';

const NotFound: React.FC = () => {
  const navigate = useNavigate();

  return (
    <Container className="min-vh-100 d-flex align-items-center justify-content-center">
      <div className="text-center">
        <h1 className="display-1 fw-bold">404</h1>
        <p className="fs-3">
          <span className="text-danger">Oops!</span> Page not found
        </p>
        <p className="lead">The page you're looking for doesn't exist.</p>
        <Button variant="primary" onClick={() => navigate('/')}>
          <Home size={20} className="me-2" />
          Go Home
        </Button>
      </div>
    </Container>
  );
};

export default NotFound;

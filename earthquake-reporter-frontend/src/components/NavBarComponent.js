import React from 'react';
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import NavDropdown from 'react-bootstrap/NavDropdown';
import { Link, useNavigate } from 'react-router-dom';
import Button from 'react-bootstrap/Button';

function NavbarComponent() {
  const navigate = useNavigate(); // Initialize navigate function

  const handleLogout = async () => {
    try {
      // Clear tokens from both storage mechanisms
      localStorage.removeItem('token');
      sessionStorage.removeItem('token');
      
      // Optionally, notify the server to invalidate the token if necessary
      await fetch('https://localhost:5001/api/auth/logout', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token') || sessionStorage.getItem('token')}`
        }
      });
  
      // Redirect to login page using React Router
      navigate('/');
    } catch (error) {
      console.error('Logout error:', error);
    }
  };  

  return (
    <Navbar expand="lg" className="bg-body-tertiary">
      <Container>
        <Navbar.Brand as={Link} to="/dashboard">Earthquake Reporter</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="me-auto">
            <Nav.Link as={Link} to="/email-notification">Email Notification</Nav.Link>
            <Nav.Link as={Link} to="/family-members">Family Members</Nav.Link>
            <Nav.Link as={Link} to="/profile">Profile</Nav.Link>
            <NavDropdown title="More" id="basic-nav-dropdown">
              <NavDropdown.Item href="https://www.afad.gov.tr/" target="_blank" rel="noopener noreferrer">
                AFAD Page
              </NavDropdown.Item>
              <NavDropdown.Item href="http://www.koeri.boun.edu.tr/scripts/lst0.asp" target="_blank" rel="noopener noreferrer">
                Kandilli Rasathanesi Page
              </NavDropdown.Item>
              <NavDropdown.Item as={Link} to="/about">
                About Us
              </NavDropdown.Item>
            </NavDropdown>
          </Nav>
          <Nav className="ms-auto">
            <Button variant="danger" onClick={handleLogout}>
              Log Out
            </Button>
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default NavbarComponent;

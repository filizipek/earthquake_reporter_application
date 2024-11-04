import React, { useContext, useState, useEffect } from 'react';
import { Container, Card, Button, Form, Table, Alert, Spinner } from 'react-bootstrap';
import {jwtDecode} from 'jwt-decode'; // Import jwt-decode
import { AuthContext } from '../AuthContext'; 
import NavbarComponent from './NavBarComponent';
import { Link } from 'react-router-dom';
import FamilyMap from './FamilyMap';  // Ensure correct import path

function FamilyMembers() {
  const [members, setMembers] = useState([]);
  const [showForm, setShowForm] = useState(false);
  const [newMember, setNewMember] = useState({
    name: '',
    surname: '',
    province: '',
    country: ''
  });
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);
  const [userEmail, setUserEmail] = useState(null);

  useEffect(() => {
    // Decode token to get user email
    const token = localStorage.getItem('token') || sessionStorage.getItem('token');
    if (token) {
      try {
        const decodedToken = jwtDecode(token);
        setUserEmail(decodedToken.email); // Assuming the token contains an 'email' field
      } catch (err) {
        console.error('Error decoding token:', err);
        setError('Error decoding token.');
      }
    }
  }, []);

  const apiUrl = 'https://localhost:5001/api/familymembers';

  const fetchFamilyMembers = async () => {
    setLoading(true);
    try {
      const response = await fetch(apiUrl, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token') || sessionStorage.getItem('token')}`
        }
      });
      if (!response.ok) {
        throw new Error('Network response was not ok');
      }
      const data = await response.json();
      setMembers(data);
      setError(null); // Clear any previous errors
    } catch (error) {
      console.error('Error fetching family members:', error);
      setError('Error fetching family members.');
    } finally {
      setLoading(false);
    }
  };

  const handleAddMember = async () => {
    if (!userEmail) {
      console.error('UserEmail is required');
      return;
    }

    const memberDto = {
      Name: newMember.name,
      Surname: newMember.surname,
      Province: newMember.province,
      Country: newMember.country,
      UserEmail: userEmail
    };

    try {
      const response = await fetch(apiUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token') || sessionStorage.getItem('token')}`
        },
        body: JSON.stringify(memberDto)
      });

      if (!response.ok) {
        const errorData = await response.text();
        // Check for specific error message
        if (errorData.includes('maximum number of family members')) {
          throw new Error('You have reached the maximum number of family members!');
        }
        throw new Error(`Network response was not ok: ${errorData}`);
      }

      setNewMember({ name: '', surname: '', province: '', country: '' });
      fetchFamilyMembers();
      setError(null); // Clear any previous error
    } catch (error) {
      console.error('Error adding family member:', error);
      setError(error.message); // Set specific error message
    }
  };

  useEffect(() => {
    fetchFamilyMembers();
  }, []);

  return (
    <div>
      <NavbarComponent />
      <Container className="mt-4">
        <Link to="/dashboard">
          <Button variant="secondary" className="mb-3">Return to Dashboard</Button>
        </Link>
        <Card className="mb-4">
          <Card.Body>
            <Card.Title>Family Members</Card.Title>
            <Button variant="primary" onClick={() => setShowForm(!showForm)}>
              {showForm ? 'Hide Form' : 'Add New Member'}
            </Button>
            {showForm && (
              <Form className="mt-4">
                <Form.Group className="mb-3">
                  <Form.Label>Name</Form.Label>
                  <Form.Control
                    type="text"
                    value={newMember.name}
                    onChange={(e) => setNewMember({ ...newMember, name: e.target.value })}
                  />
                </Form.Group>
                <Form.Group className="mb-3">
                  <Form.Label>Surname</Form.Label>
                  <Form.Control
                    type="text"
                    value={newMember.surname}
                    onChange={(e) => setNewMember({ ...newMember, surname: e.target.value })}
                  />
                </Form.Group>
                <Form.Group className="mb-3">
                  <Form.Label>Province</Form.Label>
                  <Form.Control
                    type="text"
                    value={newMember.province}
                    onChange={(e) => setNewMember({ ...newMember, province: e.target.value })}
                  />
                </Form.Group>
                <Form.Group className="mb-3">
                  <Form.Label>Country</Form.Label>
                  <Form.Control
                    type="text"
                    value={newMember.country}
                    onChange={(e) => setNewMember({ ...newMember, country: e.target.value })}
                  />
                </Form.Group>
                <Button variant="primary" onClick={handleAddMember}>
                  Add Member
                </Button>
                {error && <Alert variant="danger" className="mt-3">{error}</Alert>}
              </Form>
            )}
            {loading ? (
              <Spinner animation="border" />
            ) : (
              <>
                <FamilyMap members={members} />
                <Table className="mt-4" striped bordered hover>
                  <thead>
                    <tr>
                      <th>Name</th>
                      <th>Surname</th>
                      <th>Province</th>
                      <th>Country</th>
                    </tr>
                  </thead>
                  <tbody>
                    {members.map((member, index) => (
                      <tr key={index}>
                        <td>{member.name}</td>
                        <td>{member.surname}</td>
                        <td>{member.province}</td>
                        <td>{member.country}</td>
                      </tr>
                    ))}
                  </tbody>
                </Table>
              </>
            )}
          </Card.Body>
        </Card>
      </Container>
    </div>
  );
}

export default FamilyMembers;

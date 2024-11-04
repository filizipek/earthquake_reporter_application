import React, { useState, useEffect } from 'react';
import Card from 'react-bootstrap/Card';
import Button from 'react-bootstrap/Button';
import Table from 'react-bootstrap/Table';
import Form from 'react-bootstrap/Form';
import NavbarComponent from './NavBarComponent';
import axios from 'axios';
import Carousel from 'react-bootstrap/Carousel';
import './Dashboard.css';

function Dashboard() {
  const [showEarthquakeTable, setShowEarthquakeTable] = useState(false);
  const [earthquakes, setEarthquakes] = useState([]);
  const [filterMagnitude, setFilterMagnitude] = useState('');
  const [filterLocation, setFilterLocation] = useState('');
  const [filterType, setFilterType] = useState('');
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    if (showEarthquakeTable) {
      fetchEarthquakes();
    }
  }, [showEarthquakeTable]);

  const fetchEarthquakes = async () => {
    try {
      const response = await axios.get('https://localhost:5001/api/earthquakes');
      setEarthquakes(response.data);
      setErrorMessage(''); // Clear any previous error messages
    } catch (error) {
      console.error('Error fetching earthquake data:', error);
      setErrorMessage('Error occurred while fetching earthquake data.');
    }
  };

  const handleToggleEarthquakeTable = () => setShowEarthquakeTable(!showEarthquakeTable);

  const handleFilter = async () => {
    try {
      const params = new URLSearchParams();
      if (filterMagnitude) params.append('magnitude', filterMagnitude);
      if (filterLocation) params.append('location', filterLocation);
      if (filterType) params.append('filterType', filterType);

      const response = await axios.get(`https://localhost:5001/api/earthquakes/filter?${params.toString()}`);
      if (response.data.length === 0) {
        setEarthquakes([]); // Reset the table to empty
        setErrorMessage('No earthquakes found with the applied filters.');
      } else {
        setEarthquakes(response.data);
        setErrorMessage(''); // Clear any previous error messages
      }
    } catch (error) {
      console.error('Error filtering earthquake data:', error);
      setEarthquakes([]); // Ensure the table is reset on error
      setErrorMessage('Error occurred while filtering earthquake data.');
    }
  };

  return (
    <div>
      <NavbarComponent />

      <div className="welcome-section">
        <Carousel>
          <Carousel.Item>
            <div className="carousel-content">
              <h1>Welcome to the Dashboard</h1>
              <p>Explore daily earthquake data, receive notifications, and more.</p>
            </div>
          </Carousel.Item>
          <Carousel.Item>
            <div className="carousel-content">
              <h1>Stay Informed</h1>
              <p>Get the latest updates and insights on earthquake activity.</p>
            </div>
          </Carousel.Item>
          <Carousel.Item>
            <div className="carousel-content">
              <h1>Your Earthquake Insights</h1>
              <p>Track and analyze earthquake data with ease and precision.</p>
            </div>
          </Carousel.Item>
        </Carousel>
      </div>

      <main className='container mt-4'>
        <Card className="mb-4">
          <Card.Body>
            <Card.Title>Daily Earthquakes</Card.Title>
            <Button variant="primary" onClick={handleToggleEarthquakeTable}>
              {showEarthquakeTable ? 'Hide Table' : 'Show Table'}
            </Button>

            {showEarthquakeTable && (
              <>
                <Form className="mt-4">
                  <Form.Group controlId="filterMagnitude">
                    <Form.Label>Filter by Magnitude</Form.Label>
                    <Form.Control
                      type="number"
                      placeholder="Enter magnitude"
                      value={filterMagnitude}
                      onChange={(e) => setFilterMagnitude(e.target.value)}
                    />
                  </Form.Group>

                  <Form.Group controlId="filterLocation" className="mt-3">
                    <Form.Label>Filter by Country or Province</Form.Label>
                    <Form.Control
                      type="text"
                      placeholder="Enter country or province"
                      value={filterLocation}
                      onChange={(e) => setFilterLocation(e.target.value)}
                    />
                  </Form.Group>

                  <Form.Group controlId="filterType" className="mt-3">
                    <Form.Label>Filter Type</Form.Label>
                    <Form.Control
                      as="select"
                      value={filterType}
                      onChange={(e) => setFilterType(e.target.value)}
                    >
                      <option value="">Select Filter Type</option>
                      <option value="country">Country</option>
                      <option value="province">Province</option>
                    </Form.Control>
                  </Form.Group>

                  <Button className="mt-3" variant="secondary" onClick={handleFilter}>
                    Apply Filter
                  </Button>
                </Form>

                {errorMessage && (
                  <div className="alert alert-warning mt-4">
                    {errorMessage}
                  </div>
                )}

                <Table className="mt-4" striped bordered hover>
                  <thead>
                    <tr>
                      <th>Date</th>
                      <th>Location</th>
                      <th>Magnitude</th>
                      <th>Depth</th>
                    </tr>
                  </thead>
                  <tbody>
                    {earthquakes.length > 0 ? (
                      earthquakes.map((quake) => (
                        <tr key={quake.eventId}>
                          <td>{new Date(quake.date).toLocaleDateString()}</td>
                          <td>{quake.location}</td>
                          <td>{quake.magnitude}</td>
                          <td>{quake.depth}</td>
                        </tr>
                      ))
                    ) : (
                      <tr>
                        <td colSpan="4">No earthquake data available.</td>
                      </tr>
                    )}
                  </tbody>
                </Table>
              </>
            )}
          </Card.Body>
        </Card>
      </main>
    </div>
  );
}

export default Dashboard;

import React from 'react';
import NavbarComponent from './NavBarComponent'; // Import your Navbar component
import Card from 'react-bootstrap/Card';
import Container from 'react-bootstrap/Container';
import './About.css'; // Optional, for custom styling

function About() {
  return (
    <div>
      <NavbarComponent /> {/* Render the Navbar */}

      <Container className="mt-4">
        <Card>
          <Card.Body>
            <Card.Title>About Us</Card.Title>
            <Card.Text>
              Welcome to our Earthquake Reporter application! Our platform provides real-time earthquake data to help you stay informed and prepared. With features like instant email notifications for significant earthquakes, you can keep track of seismic activities that matter to you and your loved ones.
              <br />
              Explore our application to discover all its features and capabilities. We are dedicated to ensuring that you have the information you need to stay safe and well-informed.
            </Card.Text>
            <Card.Title>Earthquake Awareness in Turkey</Card.Title>
            <Card.Text>
              Turkey is one of the most seismically active regions in the world due to its location on the complex tectonic boundary between the Eurasian, Arabian, and African plates. The country experiences frequent earthquakes, some of which can be quite severe. Understanding this risk and being prepared is crucial for ensuring safety and minimizing damage.
              <br />
              Our application aims to help residents and visitors stay alert and informed about earthquake activities, providing timely data and notifications. Being prepared and informed is essential for safety in a country where earthquakes are a significant natural hazard.
            </Card.Text>
            <Card.Title>Contact Us</Card.Title>
            <Card.Text>
              Have questions or need more information? We’re here to help! Reach out to us via email at:
              <br />
              <a href="mailto:earthquake.reporter01@gmail.com">earthquake.reporter01@gmail.com</a>
            </Card.Text>
          </Card.Body>
        </Card>
      </Container>
    </div>
  );
}

export default About;

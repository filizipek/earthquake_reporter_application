import React, { useState, useEffect } from 'react';
import { Container, Form, Button, Alert, Card } from 'react-bootstrap';
import NavbarComponent from './NavBarComponent';

function EmailNotification() {
  const [emailNotificationsEnabled, setEmailNotificationsEnabled] = useState(false);
  const [saved, setSaved] = useState(false);

  // Load preferences from local storage on component mount
  useEffect(() => {
    const savedPreference = localStorage.getItem('emailNotificationsEnabled');
    if (savedPreference !== null) {
      setEmailNotificationsEnabled(JSON.parse(savedPreference));
    }
  }, []);

  // Save preferences to local storage and show a confirmation message
  const handleSaveSettings = () => {
    localStorage.setItem('emailNotificationsEnabled', JSON.stringify(emailNotificationsEnabled));
    setSaved(true);

    // Clear the saved state after a few seconds to hide the message
    setTimeout(() => {
      setSaved(false);
    }, 3000);
  };

  return (
    <div>
      <NavbarComponent /> {/* Include the navbar */}
      <Container className="mt-4">
        <Card>
          <Card.Body>
            <Card.Title>Email Notification Consent</Card.Title>
            <Card.Text>
              By enabling this service, you consent to receive email notifications if an earthquake occurs 
              near any of your registered family members. This service aims to keep you informed about potential 
              natural disasters that could impact your loved ones. You can disable this notification service at any time.
            </Card.Text>

            <Form>
              <Form.Group controlId="emailNotificationToggle">
                <Form.Check
                  type="switch"
                  label="I consent to receive email notifications about earthquakes near my family members"
                  checked={emailNotificationsEnabled}
                  onChange={() => setEmailNotificationsEnabled(!emailNotificationsEnabled)}
                />
              </Form.Group>

              <Button variant="primary" className="mt-3" onClick={handleSaveSettings}>
                Save Consent
              </Button>

              {saved && (
                <Alert variant="success" className="mt-3">
                  Your preferences have been saved!
                </Alert>
              )}
            </Form>
          </Card.Body>
        </Card>
      </Container>
    </div>
  );
}

export default EmailNotification;

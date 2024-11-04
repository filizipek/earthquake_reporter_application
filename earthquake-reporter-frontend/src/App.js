import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css'; // Import Bootstrap CSS
import Dashboard from './components/Dashboard';
import FamilyMembers from './components/FamilyMembers';
import Profile from './components/Profile'; // Ensure Profile component is correctly created
import Login from './components/Login'; // Ensure Login component is correctly created
import About from './components/About';
import EmailNotification from './components/EmailNotification';
import { AuthProvider } from './AuthContext'; // Adjust the path as needed

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/" element={<Login />} /> {/* Default route */}
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/family-members" element={<FamilyMembers />} />
          <Route path="/profile" element={<Profile />} />
          <Route path="/about" element={<About />} />
          <Route path="/email-notification" element={<EmailNotification />} />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;

import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

function Login() {
  const [activeTab, setActiveTab] = useState('tab1');
  const [loginData, setLoginData] = useState({ email: '', password: '' });
  const [registerData, setRegisterData] = useState({
    name: '',
    surname: '',
    email: '',
    password: '',
    birthday: '',
    province: '',
    country: '',
  });
  const [errorMessage, setErrorMessage] = useState('');
  const [rememberMe, setRememberMe] = useState(false);
  const navigate = useNavigate();

  const apiBaseUrl = 'https://localhost:5001';

  const handleTabChange = (tab) => {
    if (tab !== activeTab) {
      setActiveTab(tab);
      setErrorMessage('');
    }
  };

  const handleSignIn = async () => {
    if (!loginData.email || !loginData.password) {
      setErrorMessage('Please fill in all fields.');
      return;
    }

    try {
      console.log('Sending login request with:', loginData);
      const response = await fetch(`${apiBaseUrl}/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(loginData),
      });

      if (response.status === 401) {
        setErrorMessage('Invalid email or password. Please try again.');
        return;
      }

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      const data = await response.json();
      console.log('Sign-in response:', data);

      if (data.success) {
        if (rememberMe) {
          localStorage.setItem('token', data.token); // Store token in localStorage
        } else {
          sessionStorage.setItem('token', data.token); // Store token in sessionStorage
        }
        navigate('/dashboard');
      } else {
        setErrorMessage(data.message || 'Invalid email or password');
      }
    } catch (error) {
      console.error('Sign-in error:', error);
      setErrorMessage('An error occurred. Please try again.');
    }
  };

  const handleSignUp = async () => {
    if (Object.values(registerData).some(field => !field)) {
      setErrorMessage('Please fill in all fields.');
      return;
    }

    try {
      console.log('Sending registration request with:', registerData);
      const response = await fetch(`${apiBaseUrl}/auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          ...registerData,
          birthday: new Date(registerData.birthday).toISOString(),
        }),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      const data = await response.json();
      console.log('Registration response:', data);

      if (data.success) {
        setErrorMessage('Registration successful. Please log in to continue.');
        handleTabChange('tab1');
      } else {
        setErrorMessage(data.message || 'Registration failed.');
      }
    } catch (error) {
      console.error('Registration error:', error);
      setErrorMessage('An error occurred. Please try again.');
    }
  };

  const handleForgotPassword = () => {
    // Handle forgot password action here
  };

  const handleRegisterLinkClick = () => {
    handleTabChange('tab1');
  };

  const headingStyle = {
    color: 'black',
    textAlign: 'center',
    marginBottom: '1.5rem',
  };

  return (
    <div className="container p-3 my-5 d-flex flex-column w-50">
      <h1 style={headingStyle}>Welcome to the Earthquake Reporter</h1>

      <div className="nav nav-pills nav-justified mb-4">
        <button
          className={`nav-link ${activeTab === 'tab1' ? 'active' : ''}`}
          onClick={() => handleTabChange('tab1')}
        >
          Login
        </button>
        <button
          className={`nav-link ${activeTab === 'tab2' ? 'active' : ''}`}
          onClick={() => handleTabChange('tab2')}
        >
          Register
        </button>
      </div>

      {errorMessage && <div className="alert alert-danger">{errorMessage}</div>}

      <div className="tab-content w-100">
        <div className={`tab-pane fade ${activeTab === 'tab1' ? 'show active' : ''}`}>
          <input
            className="form-control mb-4"
            placeholder="Email address"
            type="email"
            value={loginData.email}
            onChange={(e) => setLoginData({ ...loginData, email: e.target.value })}
          />
          <input
            className="form-control mb-4"
            placeholder="Password"
            type="password"
            value={loginData.password}
            onChange={(e) => setLoginData({ ...loginData, password: e.target.value })}
          />

          <div className="row mb-4">
            <div className="col d-flex align-items-center">
              <input
                type="checkbox"
                id="rememberMe"
                className="form-check-input me-2"
                checked={rememberMe}
                onChange={(e) => setRememberMe(e.target.checked)}
              />
              <label htmlFor="rememberMe" className="form-label">Remember me</label>
            </div>
            <div className="col text-end">
              <button className="btn btn-link p-0" onClick={handleForgotPassword}>
                Forgot password?
              </button>
            </div>
          </div>

          <button className="btn btn-primary w-100 mb-4" onClick={handleSignIn}>
            Sign In
          </button>
        </div>

        <div className={`tab-pane fade ${activeTab === 'tab2' ? 'show active' : ''}`}>
          <input
            className="form-control mb-4"
            placeholder="First name"
            type="text"
            value={registerData.name}
            onChange={(e) => setRegisterData({ ...registerData, name: e.target.value })}
          />
          <input
            className="form-control mb-4"
            placeholder="Surname"
            type="text"
            value={registerData.surname}
            onChange={(e) => setRegisterData({ ...registerData, surname: e.target.value })}
          />
          <input
            className="form-control mb-4"
            placeholder="Email address"
            type="email"
            value={registerData.email}
            onChange={(e) => setRegisterData({ ...registerData, email: e.target.value })}
          />
          <input
            className="form-control mb-4"
            placeholder="Password"
            type="password"
            value={registerData.password}
            onChange={(e) => setRegisterData({ ...registerData, password: e.target.value })}
          />
          <input
            className="form-control mb-4"
            placeholder="Birthday"
            type="date"
            value={registerData.birthday}
            onChange={(e) => setRegisterData({ ...registerData, birthday: e.target.value })}
          />
          <input
            className="form-control mb-4"
            placeholder="Province"
            type="text"
            value={registerData.province}
            onChange={(e) => setRegisterData({ ...registerData, province: e.target.value })}
          />
          <input
            className="form-control mb-4"
            placeholder="Country"
            type="text"
            value={registerData.country}
            onChange={(e) => setRegisterData({ ...registerData, country: e.target.value })}
          />

          <button className="btn btn-primary w-100 mb-4" onClick={handleSignUp}>
            Register
          </button>

          <button className="btn btn-link w-100" onClick={handleRegisterLinkClick}>
            Already have an account? Log in here
          </button>
        </div>
      </div>
    </div>
  );
}

export default Login;

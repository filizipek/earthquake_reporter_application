import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import NavbarComponent from './NavBarComponent'; // Import NavbarComponent
import './Profile.css'; // Import the updated CSS file

const apiBaseUrl = 'https://localhost:5001/api/userprofile'; // Replace with your actual API base URL

function isTokenExpired(token) {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const expiration = payload.exp; // Assuming 'exp' is the expiration time in seconds since epoch
    return expiration < Date.now() / 1000;
  } catch (e) {
    return true; // If there's an error decoding the token, consider it expired
  }
}

function generateRandomUser() {
  return {
    name: 'John Doe',
    surname: 'Doe',
    email: 'johndoe@example.com',
    province: 'Province',
    country: 'Country',
    birthday: '1990-01-01'
  };
}

function formatDate(dateString) {
  const options = { day: '2-digit', month: '2-digit', year: 'numeric' };
  const date = new Date(dateString);
  return date.toLocaleDateString('en-GB', options); // Format date as dd-MM-yyyy
}

function Profile() {
  const [profileData, setProfileData] = useState(null);
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const fetchProfileData = async () => {
      const token = localStorage.getItem('token') || sessionStorage.getItem('token');

      if (!token) {
        console.error('No token found. Redirecting to login.');
        setErrorMessage('No token found. Redirecting to login.');
        return;
      }

      if (isTokenExpired(token)) {
        console.error('Token is expired. Redirecting to login.');
        setErrorMessage('Token is expired. Redirecting to login.');
        navigate('/login');
        return;
      }

      try {
        const response = await fetch(`${apiBaseUrl}/profile`, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}` // Ensure token is sent in the header
          }
        });

        console.log('Response Status:', response.status);
        console.log('Response Headers:', response.headers);

        if (response.status === 401) {
          setErrorMessage('Unauthorized. Token might be invalid or expired.');
          return;
        }

        if (!response.ok) {
          console.error(`Failed to fetch profile data: ${response.statusText}`);
          setErrorMessage(`Failed to fetch profile data: ${response.statusText}`);
          return;
        }

        const data = await response.json();
        console.log('Profile data:', data);

        if (data.success && data.profile) {
          setProfileData(data.profile);
        } else {
          setErrorMessage('Profile data not found.');
        }
      } catch (error) {
        console.error('Error fetching profile data:', error);
        setErrorMessage('Error fetching profile data.');
      }
    };

    fetchProfileData();
  }, [navigate]);

  const userData = profileData || generateRandomUser(); 

  return (
    <>
      <div> {/* Navbar div */}
        <NavbarComponent className="navbar-spacing" /> {/* Apply spacing class */}
      </div>
      <div className="container mt-5 profile-container"> {/* Main content container */}
        {errorMessage && <div className="alert alert-danger">{errorMessage}</div>}
        
        <div className="card mt-3">
          <div className="card-body">
            <h5 className="card-title">Profile Information</h5>
            <table className="table">
              <tbody>
                <tr>
                  <th>Name</th>
                  <td>{userData.name || 'N/A'}</td>
                </tr>
                <tr>
                  <th>Surname</th>
                  <td>{userData.surname || 'N/A'}</td>
                </tr>
                <tr>
                  <th>Email</th>
                  <td>{userData.email || 'N/A'}</td>
                </tr>
                <tr>
                  <th>Province</th>
                  <td>{userData.province || 'N/A'}</td>
                </tr>
                <tr>
                  <th>Country</th>
                  <td>{userData.country || 'N/A'}</td>
                </tr>
                <tr>
                  <th>Birthday</th>
                  <td>{userData.birthday ? formatDate(userData.birthday) : 'N/A'}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </>
  );
}

export default Profile;

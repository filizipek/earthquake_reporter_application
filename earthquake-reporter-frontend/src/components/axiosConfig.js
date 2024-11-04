import axios from 'axios';

// Fetch the token and log it for debugging
const token = localStorage.getItem('token') || sessionStorage.getItem('token');
if (!token) {
  console.error('No token found');
}

// Create an Axios instance
const axiosInstance = axios.create({
  baseURL: 'https://localhost:5001',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`, // Ensure this line is correct
  },
});

async function fetchProfileData() {
  try {
    const response = await axiosInstance.get('/api/userprofile/profile');
    console.log('Profile data:', response.data);
  } catch (error) {
    console.error('Error fetching profile data:', error.response ? error.response.data : error.message);
  }
}

fetchProfileData();

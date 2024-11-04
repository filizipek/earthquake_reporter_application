import React, { createContext, useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode'; // Correct named export

// Create Context
export const AuthContext = createContext();

// AuthProvider component
export const AuthProvider = ({ children }) => {
  const [userEmail, setUserEmail] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem('token'); // Fetch the token from localStorage
    if (token) {
      try {
        const decodedToken = jwtDecode(token);
        setUserEmail(decodedToken.email); // Adjust according to your token structure
      } catch (error) {
        console.error('Failed to decode token:', error);
      }
    }
  }, []);

  const value = { userEmail };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

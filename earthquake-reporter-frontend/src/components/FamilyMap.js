import React, { useEffect, useState } from 'react';
import { MapContainer, TileLayer, Marker, Popup } from 'react-leaflet';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';

// Define a custom icon
const familyMemberIcon = new L.Icon({
  iconUrl: '/pins/family-pin.png', // Path to your custom icon
  iconSize: [32, 32], // Size of the icon
  iconAnchor: [16, 32], // Point of the icon which will correspond to marker's location
  popupAnchor: [0, -32] // Point from which the popup should open relative to the iconAnchor
});

// Function to get latitude and longitude from province and country
const getLatLngFromAddress = async (province, country) => {
  try {
    const response = await fetch(`https://nominatim.openstreetmap.org/search?q=${province},${country}&format=json&limit=1`);
    const data = await response.json();
    if (data && data[0]) {
      return {
        latitude: parseFloat(data[0].lat),
        longitude: parseFloat(data[0].lon),
      };
    } else {
      return { latitude: null, longitude: null };
    }
  } catch (error) {
    console.error('Error fetching location:', error);
    return { latitude: null, longitude: null };
  }
};

const FamilyMap = ({ members }) => {
  const [updatedMembers, setUpdatedMembers] = useState([]);

  useEffect(() => {
    const fetchLocations = async () => {
      const membersWithLocation = await Promise.all(
        members.map(async (member) => {
          if (member.province && member.country) {
            const { latitude, longitude } = await getLatLngFromAddress(member.province, member.country);
            return { ...member, latitude, longitude };
          }
          return { ...member, latitude: null, longitude: null };
        })
      );
      setUpdatedMembers(membersWithLocation);
    };

    fetchLocations();
  }, [members]);

  return (
    <MapContainer center={[0, 0]} zoom={2} style={{ height: '500px', width: '100%' }}>
      <TileLayer
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
      />
      {updatedMembers.map((member, index) => (
        member.latitude && member.longitude ? (
          <Marker
            key={index}
            position={[member.latitude, member.longitude]}
            icon={familyMemberIcon} // Use custom icon
          >
            <Popup>
              <div>
                <strong>{member.name} {member.surname}</strong>
                <p>{member.province}, {member.country}</p>
              </div>
            </Popup>
          </Marker>
        ) : null
      ))}
    </MapContainer>
  );
};

export default FamilyMap;

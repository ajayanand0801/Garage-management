#!/usr/bin/env bash
# Create Service Request - Garage Management API
# Usage: ./create-service-request.sh [BASE_URL]
# Example: ./create-service-request.sh http://localhost:5000

BASE_URL="${1:-http://localhost:5000}"

curl -s -X POST "${BASE_URL}/api/ServiceRequest" \
  -H "Content-Type: application/json" \
  -d '{
    "DomainType": "Vehicle",
    "ServiceType": "Repair",
    "Description": "Oil change and brake inspection",
    "Priority": "Medium",
    "CreatedBy": "api-user",
    "Customer": {
      "FirstName": "John",
      "LastName": "Doe",
      "Email": "john.doe@example.com",
      "Phone": "+971501234567",
      "MobilePhone": "+971501234567",
      "Address": "123 Main Street",
      "City": "Dubai",
      "State": "Dubai"
    },
    "Booking": {
      "BookingType": "Service",
      "StartDate": "2025-02-15T09:00:00Z",
      "EndDate": "2025-02-15T11:00:00Z",
      "BookedBy": "api-user",
      "Status": "Scheduled"
    },
    "DomainData": {
      "Vehicle": {
        "Make": "Toyota",
        "Model": "Camry",
        "Year": 2022,
        "VIN": "1HGBH41JXMN109186"
      }
    }
  }'

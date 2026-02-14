#!/usr/bin/env bash
# Create Quote (Quotation) - Garage Management API
# Requires an existing Service Request ID. Create a service request first, then use its ID.
# Usage: ./create-quote.sh [BASE_URL] [REQUEST_ID]
# Example: ./create-quote.sh http://localhost:5000 1

BASE_URL="${1:-http://localhost:5000}"
REQUEST_ID="${2:-1}"

curl -s -X POST "${BASE_URL}/api/Quotation/${REQUEST_ID}" \
  -H "Content-Type: application/json" \
  -d '{
    "QuotationNo": "QT-2025-001",
    "EstimatedTotal": 500.00,
    "Currency": "AED",
    "Discount": 25.00,
    "Tax": 47.50,
    "GrandTotal": 522.50,
    "Status": "Draft",
    "CreatedBy": "api-user",
    "QuotationItems": [
      {
        "Name": "Oil Change",
        "PartNumber": "OIL-SYN-5W30",
        "Description": "Full synthetic oil change",
        "Quantity": 1,
        "UnitPrice": 150.00,
        "TotalPrice": 150.00,
        "ItemType": "Labour",
        "Hours": 1.0,
        "IsOptional": false
      },
      {
        "Name": "Brake Pad Set",
        "PartNumber": "BRK-PAD-FR",
        "Description": "Front brake pads",
        "Quantity": 1,
        "UnitPrice": 350.00,
        "TotalPrice": 350.00,
        "ItemType": "Part",
        "IsOptional": false
      }
    ]
  }'

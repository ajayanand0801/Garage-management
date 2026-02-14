#!/usr/bin/env bash
# Create Work Order - Garage Management API
# Requires an existing Vehicle and Quotation. Create service request and quote first, then use VehicleId and QuotationId.
# Usage: ./create-work-order.sh [BASE_URL]
# Example: ./create-work-order.sh http://localhost:5000

BASE_URL="${1:-http://localhost:5000}"

curl -s -X POST "${BASE_URL}/api/WorkOrder" \
  -H "Content-Type: application/json" \
  -d '{
    "VehicleId": 1,
    "QuotationId": 1,
    "CustomerId": 1,
    "ScheduledStart": "2025-02-15T09:00:00Z",
    "ScheduledEnd": "2025-02-15T12:00:00Z",
    "Notes": "Customer requested morning slot. Check brake fluid level."
  }'

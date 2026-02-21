using GarageManagement.Application.DTOs;
using GarageManagement.Application.Interfaces;
using GarageManagement.Domain.Entites;
using GarageManagement.Domain.Entites.Request;
using GarageManagement.Domain.Entites.Vehicles;

namespace GarageManagement.Application.Services
{
    /// <summary>
    /// Validates that Customer, Vehicle (when provided), and Service Request exist and are active,
    /// and that booking CustomerId/VehicleId match the referenced Service Request.
    /// </summary>
    public class BookingReferenceValidator : IBookingReferenceValidator
    {
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IGenericRepository<Vehicle> _vehicleRepository;
        private readonly IGenericRepository<ServiceRequest> _serviceRequestRepository;
        private readonly IGarageServiceRepository _garageServiceRepository;

        public BookingReferenceValidator(
            IGenericRepository<Customer> customerRepository,
            IGenericRepository<Vehicle> vehicleRepository,
            IGenericRepository<ServiceRequest> serviceRequestRepository,
            IGarageServiceRepository garageServiceRepository)
        {
            _customerRepository = customerRepository;
            _vehicleRepository = vehicleRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _garageServiceRepository = garageServiceRepository;
        }

        /// <inheritdoc />
        public async Task ValidateAsync(BookingDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // ServiceID must reference a valid, active garage service
            var garageService = await _garageServiceRepository.GetByIdAsync(dto.ServiceID, null);
            if (garageService == null)
                throw new InvalidOperationException($"Garage service with ID {dto.ServiceID} does not exist or is not active.");

            // Service Request must exist and be active
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(dto.ServiceRequestId, null);
            if (serviceRequest == null)
                throw new InvalidOperationException($"Service request with ID {dto.ServiceRequestId} does not exist or is not active.");

            // Customer must exist and be active
            var customer = await _customerRepository.GetByIdAsync(dto.CustomerID, null);
            if (customer == null)
                throw new InvalidOperationException($"Customer with ID {dto.CustomerID} does not exist or is not active.");

            // Vehicle: if provided, must exist and be active
            if (dto.VehicleID.HasValue && dto.VehicleID.Value > 0)
            {
                var vehicle = await _vehicleRepository.GetByVehicleIdAsync(dto.VehicleID.Value, null);
                if (vehicle == null)
                    throw new InvalidOperationException($"Vehicle with ID {dto.VehicleID} does not exist or is not active.");
            }

            // Booking CustomerID must match Service Request's CustomerID
            if (dto.CustomerID != serviceRequest.CustomerID)
                throw new InvalidOperationException(
                    "Booking CustomerID must match the Service Request's CustomerID. " +
                    $"Expected {serviceRequest.CustomerID}, got {dto.CustomerID}.");

            // Booking VehicleID must match Service Request's VehicleID when either is specified
            long? bookingVehicleId = dto.VehicleID.HasValue && dto.VehicleID.Value > 0 ? dto.VehicleID : null;
            long? srVehicleId = serviceRequest.VehicleID.HasValue && serviceRequest.VehicleID.Value > 0 ? serviceRequest.VehicleID : null;

            if (bookingVehicleId.HasValue != srVehicleId.HasValue)
                throw new InvalidOperationException(
                    "Booking VehicleID and Service Request VehicleID must both be set or both be empty. " +
                    $"Service Request VehicleID: {(srVehicleId.HasValue ? srVehicleId.ToString() : "null")}, Booking VehicleID: {(bookingVehicleId.HasValue ? bookingVehicleId.ToString() : "null")}.");

            if (bookingVehicleId.HasValue && bookingVehicleId.Value != srVehicleId!.Value)
                throw new InvalidOperationException(
                    "Booking VehicleID must match the Service Request's VehicleID. " +
                    $"Expected {srVehicleId.Value}, got {bookingVehicleId.Value}.");
        }
    }
}

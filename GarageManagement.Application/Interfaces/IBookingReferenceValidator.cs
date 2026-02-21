using GarageManagement.Application.DTOs;

namespace GarageManagement.Application.Interfaces
{
    /// <summary>
    /// Validates that CustomerId, VehicleId (when provided), and ServiceRequestId exist, are active,
    /// and that CustomerId/VehicleId match the referenced Service Request.
    /// </summary>
    public interface IBookingReferenceValidator
    {
        /// <summary>
        /// Validates booking references for create or update. Throws <see cref="InvalidOperationException"/>
        /// when Customer, Vehicle (if provided), or Service Request does not exist or is not active, or when
        /// CustomerId/VehicleId do not match the Service Request.
        /// </summary>
        Task ValidateAsync(BookingDto dto, CancellationToken cancellationToken = default);
    }
}

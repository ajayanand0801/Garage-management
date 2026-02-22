namespace GarageManagement.Application.DTOs
{
    /// <summary>
    /// DTO for PATCH (partial) update of a booking. Only provided properties are updated.
    /// </summary>
    public class BookingPatchDto
    {
        /// <summary>Valid StatusID from [bkg].[BookingStatus]. Validated before update.</summary>
      //  public long? StatusID { get; set; }

        /// <summary>Status text (e.g. DRAFT, SCHEDULED). Can be set directly or derived from StatusID.</summary>
        public string? Status { get; set; }

        /// <summary>Booking type.</summary>
        public string? TypeCode { get; set; }

        /// <summary>Start date/time. Must not be in the past (>= current date).</summary>
        public DateTime? StartDate { get; set; }

        /// <summary>End date/time. Must not be in the past (>= current date/time).</summary>
        public DateTime? EndDate { get; set; }

        /// <summary>Duration type (e.g. hourly, daily).</summary>
        public string? DurationType { get; set; }

        /// <summary>Notes.</summary>
        public string? Notes { get; set; }
    }
}

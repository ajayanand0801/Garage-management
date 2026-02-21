using System;
using System.ComponentModel;

namespace GarageManagement.Application.Enums
{
    /// <summary>
    /// Quotation status. Only Approved and Rejected are allowed for status update API.
    /// Modified is set automatically when items change on an approved quotation.
    /// </summary>
    public enum QuotationStatus
    {
        [Description("Draft")]
        Draft = 0,

        [Description("Approved")]
        Approved = 1,

        [Description("Rejected")]
        Rejected = 2,

        [Description("Modified")]
        Modified = 3
    }

    /// <summary>
    /// Extension methods for QuotationStatus enum (description and storage value).
    /// </summary>
    public static class QuotationStatusExtensions
    {
        public static string GetDescription(this QuotationStatus status)
        {
            var field = status.GetType().GetField(status.ToString());
            if (field == null) return status.ToString();

            var attr = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attr?.Description ?? status.ToString();
        }

        /// <summary>
        /// Value stored in database (lowercase for consistency with existing "draft").
        /// </summary>
        public static string ToStorageValue(this QuotationStatus status)
        {
            return status switch
            {
                QuotationStatus.Draft => "draft",
                QuotationStatus.Approved => "approved",
                QuotationStatus.Rejected => "rejected",
                QuotationStatus.Modified => "modified",
                _ => status.ToString().ToLowerInvariant()
            };
        }

        /// <summary>
        /// Normalized status string for comparison (lowercase).
        /// </summary>
        public static bool IsApproved(string? status) =>
            string.Equals(status?.Trim(), "approved", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Normalized status string for comparison (lowercase).
        /// </summary>
        public static bool IsRejected(string? status) =>
            string.Equals(status?.Trim(), "rejected", StringComparison.OrdinalIgnoreCase);

        public static bool TryParseFromRequest(string? value, out QuotationStatus status)
        {
            status = default;
            if (string.IsNullOrWhiteSpace(value)) return false;

            var normalized = value.Trim();
            if (string.Equals(normalized, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                status = QuotationStatus.Approved;
                return true;
            }
            if (string.Equals(normalized, "Rejected", StringComparison.OrdinalIgnoreCase))
            {
                status = QuotationStatus.Rejected;
                return true;
            }
            return false;
        }
    }
}

namespace prjRentalManagement.Models
{
    /// <summary>Canonical apartment listing states for rental workflow.</summary>
    public static class ApartmentStatuses
    {
        public const string Available = "Available";
        public const string Booked = "Booked";
        public const string Maintenance = "Maintenance";

        /// <summary>Legacy values still recognized in queries and migrations.</summary>
        public const string Occupied = "Occupied";

        public const string Unavailable = "Unavailable";

        public static bool IsPubliclyBookable(string status) =>
            string.Equals(status?.Trim(), Available, System.StringComparison.OrdinalIgnoreCase);

        public static bool IsRentedOrBooked(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return false;
            var s = status.Trim();
            return s.Equals(Booked, System.StringComparison.OrdinalIgnoreCase)
                || s.Equals(Occupied, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}

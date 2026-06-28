namespace prjRentalManagement.Helpers
{
    /// <summary>Maps DB apartment status to tenant-friendly labels.</summary>
    public static class RentStatusDisplay
    {
        public static string Label(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "—";
            switch (status.Trim())
            {
                case "Available":
                    return "Available";
                case "Booked":
                    return "Rented";
                case "Occupied":
                    return "Rented";
                case "Maintenance":
                    return "Under maintenance";
                case "Unavailable":
                    return "Not available";
                default:
                    return status;
            }
        }

        public static string BadgeClass(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "bg-secondary";
            switch (status.Trim())
            {
                case "Available":
                    return "bg-success";
                case "Booked":
                    return "bg-primary";
                case "Occupied":
                    return "bg-primary";
                case "Maintenance":
                    return "bg-dark";
                case "Unavailable":
                    return "bg-secondary";
                default:
                    return "bg-secondary";
            }
        }
    }
}

namespace prjRentalManagement.Helpers
{
    public static class BookingStatusDisplay
    {
        public static string BadgeClass(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "bg-secondary";
            switch (status.Trim())
            {
                case "Confirmed":
                case "Approved":
                    return "bg-success";
                case "Pending":
                    return "bg-warning text-dark";
                case "Cancelled":
                case "Rejected":
                    return "bg-danger";
                case "Completed":
                    return "bg-info text-dark";
                default:
                    return "bg-secondary";
            }
        }
    }
}

using System.Collections.Generic;

namespace prjRentalManagement.Models
{
    public sealed class OwnerDashboardViewModel
    {
        public int TotalApartments { get; set; }
        public int PendingRequestCount { get; set; }
        public int ActiveConfirmedCount { get; set; }
        public IList<booking> PendingBookings { get; set; }
        public IList<booking> ActiveBookings { get; set; }
    }
}

//------------------------------------------------------------------------------
// Matches Entity Framework model from Model1.edmx (booking entity).
//------------------------------------------------------------------------------

namespace prjRentalManagement.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class booking
    {
        public const string StatusPending = "Pending";
        public const string StatusConfirmed = "Confirmed";
        public const string StatusCancelled = "Cancelled";
        public const string StatusCompleted = "Completed";

        public int bookingId { get; set; }

        [Display(Name = "Apartment")]
        public int apartmentId { get; set; }

        [Display(Name = "Tenant")]
        public int tenantId { get; set; }

        [StringLength(20)]
        [Display(Name = "Status")]
        public string status { get; set; }

        [Display(Name = "Request date")]
        public DateTime requestDate { get; set; }

        public virtual apartment apartment { get; set; }
        public virtual tenant tenant { get; set; }
    }
}

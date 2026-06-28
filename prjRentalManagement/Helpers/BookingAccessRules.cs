using System.Linq;
using prjRentalManagement.Models;

namespace prjRentalManagement.Helpers
{
    /// <summary>Centralized tenant/apartment booking visibility rules (used by controllers and views).</summary>
    public static class BookingAccessRules
    {
        public static bool CanTenantViewApartment(DbPropertyRentalEntities db, apartment apt, int tenantId)
        {
            if (apt.tenantId == tenantId)
                return true;
            if (db.bookings.Any(b => b.apartmentId == apt.apartmentId && b.tenantId == tenantId && b.status == booking.StatusPending))
                return true;
            return ApartmentStatuses.IsPubliclyBookable(apt.status) && apt.tenantId == null
                && !db.bookings.Any(b => b.apartmentId == apt.apartmentId && b.status == booking.StatusPending && b.tenantId != tenantId);
        }

        public static bool CanTenantRequestBooking(DbPropertyRentalEntities db, apartment apt, int tenantId)
        {
            if (!ApartmentStatuses.IsPubliclyBookable(apt.status) || apt.tenantId != null)
                return false;
            if (db.bookings.Any(b => b.apartmentId == apt.apartmentId && b.status == booking.StatusPending && b.tenantId != tenantId))
                return false;
            if (db.bookings.Any(b => b.apartmentId == apt.apartmentId && b.tenantId == tenantId && b.status == booking.StatusPending))
                return false;
            return true;
        }
    }
}

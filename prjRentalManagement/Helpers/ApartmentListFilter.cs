using System;
using System.Linq;
using prjRentalManagement.Models;

namespace prjRentalManagement.Helpers
{
    public static class ApartmentListFilter
    {
        public static IQueryable<apartment> ApplySearch(IQueryable<apartment> query, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            var t = search.Trim();
            return query.Where(a =>
                a.apartmentId.ToString().Contains(t) ||
                a.apartmentNo.ToString().Contains(t) ||
                (a.building != null && (
                    a.building.address.Contains(t) ||
                    a.building.city.Contains(t) ||
                    a.building.province.Contains(t) ||
                    a.building.postalCode.Contains(t))) ||
                a.nbRooms.ToString().Contains(t) ||
                a.price.ToString().Contains(t) ||
                (a.status != null && a.status.Contains(t)));
        }

        /// <summary>rentStatus: all, available, rented, unavailable, mine (tenant only).</summary>
        public static IQueryable<apartment> ApplyRentStatus(IQueryable<apartment> query, string rentStatus, int? tenantSessionId)
        {
            if (string.IsNullOrWhiteSpace(rentStatus) || rentStatus.Equals("all", StringComparison.OrdinalIgnoreCase))
                return query;

            var r = rentStatus.Trim().ToLowerInvariant();
            switch (r)
            {
                case "available":
                    return query.Where(a => a.status == "Available" && a.tenantId == null);
                case "rented":
                case "booked":
                    return query.Where(a => a.status == "Booked" || a.status == "Occupied");
                case "unavailable":
                    return query.Where(a =>
                        a.status == "Unavailable"
                        || a.status == ApartmentStatuses.Maintenance);
                case "mine":
                    if (!tenantSessionId.HasValue)
                        return query.Where(a => false);
                    return query.Where(a => a.tenantId == tenantSessionId.Value);
                default:
                    return query;
            }
        }
    }
}

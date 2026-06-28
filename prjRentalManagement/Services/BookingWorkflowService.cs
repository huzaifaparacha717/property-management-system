using System.Data;
using System.Data.Entity;
using System.Linq;
using prjRentalManagement.Models;

namespace prjRentalManagement.Services
{
    public sealed class BookingWorkflowService
    {
        private readonly DbPropertyRentalEntities _db;

        public BookingWorkflowService(DbPropertyRentalEntities db)
        {
            _db = db;
        }

        public BookingActionResult Approve(int ownerId, int bookingId)
        {
            using (var tx = _db.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var bk = _db.bookings
                        .Include(b => b.apartment)
                        .Include(b => b.apartment.building)
                        .FirstOrDefault(b => b.bookingId == bookingId);

                    if (bk == null)
                    {
                        tx.Rollback();
                        return BookingActionResult.Fail("Booking not found.");
                    }
                    if (bk.apartment?.building == null || bk.apartment.building.ownerId != ownerId)
                    {
                        tx.Rollback();
                        return BookingActionResult.Fail("You are not allowed to manage this booking.");
                    }
                    if (bk.status != booking.StatusPending)
                    {
                        tx.Rollback();
                        return BookingActionResult.Fail("This request is no longer pending.");
                    }

                    var apt = _db.apartments.Include(a => a.building).First(a => a.apartmentId == bk.apartmentId);

                    if (apt.tenantId != null || !ApartmentStatuses.IsPubliclyBookable(apt.status))
                    {
                        bk.status = booking.StatusCancelled;
                        _db.SaveChanges();
                        tx.Commit();
                        return BookingActionResult.Fail("That unit is no longer available; the request was closed.");
                    }

                    var otherPending = _db.bookings.Where(b =>
                        b.apartmentId == apt.apartmentId &&
                        b.bookingId != bk.bookingId &&
                        b.status == booking.StatusPending).ToList();
                    foreach (var o in otherPending)
                        o.status = booking.StatusCancelled;

                    bk.status = booking.StatusConfirmed;
                    apt.tenantId = bk.tenantId;
                    apt.status = ApartmentStatuses.Booked;

                    _db.SaveChanges();
                    tx.Commit();
                    return BookingActionResult.Ok("Booking confirmed. The apartment is now marked Booked.");
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        public BookingActionResult Reject(int ownerId, int bookingId)
        {
            using (var tx = _db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var bk = _db.bookings
                        .Include(b => b.apartment)
                        .Include(b => b.apartment.building)
                        .FirstOrDefault(b => b.bookingId == bookingId);

                    if (bk == null)
                    {
                        tx.Rollback();
                        return BookingActionResult.Fail("Booking not found.");
                    }
                    if (bk.apartment?.building == null || bk.apartment.building.ownerId != ownerId)
                    {
                        tx.Rollback();
                        return BookingActionResult.Fail("You are not allowed to manage this booking.");
                    }
                    if (bk.status != booking.StatusPending)
                    {
                        tx.Rollback();
                        return BookingActionResult.Fail("This request is no longer pending.");
                    }

                    bk.status = booking.StatusCancelled;
                    _db.SaveChanges();
                    tx.Commit();
                    return BookingActionResult.Ok("Booking request cancelled.");
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>Ends an active rental: booking Completed, apartment Available.</summary>
        public BookingActionResult CompleteRental(int ownerId, int bookingId)
        {
            using (var tx = _db.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var bk = _db.bookings
                        .Include(b => b.apartment)
                        .Include(b => b.apartment.building)
                        .FirstOrDefault(b => b.bookingId == bookingId);

                    if (bk == null)
                    {
                        tx.Rollback();
                        return BookingActionResult.Fail("Booking not found.");
                    }
                    if (bk.apartment?.building == null || bk.apartment.building.ownerId != ownerId)
                    {
                        tx.Rollback();
                        return BookingActionResult.Fail("You are not allowed to manage this booking.");
                    }
                    if (bk.status != booking.StatusConfirmed)
                    {
                        tx.Rollback();
                        return BookingActionResult.Fail("Only confirmed bookings can be completed.");
                    }

                    var apt = _db.apartments.First(a => a.apartmentId == bk.apartmentId);
                    if (apt.tenantId != bk.tenantId)
                    {
                        tx.Rollback();
                        return BookingActionResult.Fail("Apartment tenant does not match this booking.");
                    }

                    bk.status = booking.StatusCompleted;
                    apt.tenantId = null;
                    apt.status = ApartmentStatuses.Available;

                    _db.SaveChanges();
                    tx.Commit();
                    return BookingActionResult.Ok("Rental marked completed. The unit is Available again.");
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }

    public sealed class BookingActionResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        private BookingActionResult(bool success, string message)
        {
            Success = success;
            Message = message ?? string.Empty;
        }

        public static BookingActionResult Ok(string message) => new BookingActionResult(true, message);
        public static BookingActionResult Fail(string message) => new BookingActionResult(false, message);
    }
}

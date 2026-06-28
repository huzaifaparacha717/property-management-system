/*
  Booking/apartment status vocabulary (idempotent).
  Run after UpgradeBookings.sql. Widens apartment.status; migrates legacy strings.
*/
USE [PropertyRentalDB];
GO

IF COL_LENGTH(N'dbo.apartments', N'status') IS NOT NULL
BEGIN
    DECLARE @aptLen INT = (SELECT CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'apartments' AND COLUMN_NAME = N'status');
    IF @aptLen IS NOT NULL AND @aptLen < 20
        ALTER TABLE [dbo].[apartments] ALTER COLUMN [status] NVARCHAR(20) NOT NULL;
END
GO

UPDATE [dbo].[bookings] SET [status] = N'Confirmed' WHERE [status] IN (N'Approved', N'approved');
UPDATE [dbo].[bookings] SET [status] = N'Cancelled' WHERE [status] IN (N'Rejected', N'rejected');
GO

UPDATE [dbo].[apartments] SET [status] = N'Booked' WHERE [status] IN (N'Occupied', N'Booked');
GO

PRINT 'UpgradeBookingWorkflowStatus.sql completed.';
GO

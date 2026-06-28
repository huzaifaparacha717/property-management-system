/*
  Idempotent: tenant booking requests (owner approve/reject). Safe for existing PropertyRentalDB.
*/
USE [PropertyRentalDB];
GO

IF OBJECT_ID(N'[dbo].[bookings]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[bookings] (
        [bookingId] INT IDENTITY(1,1) NOT NULL,
        [apartmentId] INT NOT NULL,
        [tenantId] INT NOT NULL,
        [status] NVARCHAR(20) NOT NULL,
        [requestDate] DATETIME NOT NULL,
        CONSTRAINT [PK_bookings] PRIMARY KEY CLUSTERED ([bookingId] ASC)
    );

    ALTER TABLE [dbo].[bookings]
    ADD CONSTRAINT [FK_booking_apartment]
        FOREIGN KEY ([apartmentId]) REFERENCES [dbo].[apartments]([apartmentId]);

    ALTER TABLE [dbo].[bookings]
    ADD CONSTRAINT [FK_booking_tenant]
        FOREIGN KEY ([tenantId]) REFERENCES [dbo].[tenants]([tenantId]);

    CREATE NONCLUSTERED INDEX [IX_FK_booking_apartment] ON [dbo].[bookings]([apartmentId]);
    CREATE NONCLUSTERED INDEX [IX_FK_booking_tenant] ON [dbo].[bookings]([tenantId]);
END
GO

PRINT 'UpgradeBookings.sql completed.';
GO

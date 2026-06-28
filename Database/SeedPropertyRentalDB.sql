/*
  Sample data for PropertyRentalDB (matches README test accounts).
  Password for all sample users: password123
  SHA-256 (lowercase hex): same as app PasswordHasher.Sha256

  Run after schema exists (e.g. after Models\Model1.edmx.sql).
  Safe to run multiple times (idempotent).
*/
USE [PropertyRentalDB];
GO

SET NOCOUNT ON;

DECLARE @pwd NVARCHAR(64) = N'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f';

IF NOT EXISTS (SELECT 1 FROM dbo.managers WHERE email = N'ednamode@gmail.com')
    INSERT INTO dbo.managers ([name], [email], [password], [phoneNumber])
    VALUES (N'Edna Mode', N'ednamode@gmail.com', @pwd, N'03005550001');

IF NOT EXISTS (SELECT 1 FROM dbo.owners WHERE email = N'tylerdurden@gmail.com')
    INSERT INTO dbo.owners ([name], [email], [password], [phoneNumber])
    VALUES (N'Tyler Durden', N'tylerdurden@gmail.com', @pwd, N'03005550002');

IF NOT EXISTS (SELECT 1 FROM dbo.tenants WHERE email = N'michaelcorleone@gmail.com')
    INSERT INTO dbo.tenants ([name], [email], [password], [phoneNumber])
    VALUES (N'Michael Corleone', N'michaelcorleone@gmail.com', @pwd, N'03005550003');

DECLARE @managerId INT = (SELECT managerId FROM dbo.managers WHERE email = N'ednamode@gmail.com');
DECLARE @ownerId INT = (SELECT ownerId FROM dbo.owners WHERE email = N'tylerdurden@gmail.com');
DECLARE @tenantId INT = (SELECT tenantId FROM dbo.tenants WHERE email = N'michaelcorleone@gmail.com');

IF @managerId IS NULL OR @ownerId IS NULL OR @tenantId IS NULL
BEGIN
    RAISERROR ('Seed failed: manager, owner, or tenant row missing after insert.', 16, 1);
    RETURN;
END

IF NOT EXISTS (
    SELECT 1 FROM dbo.buildings
    WHERE address = N'123 Main Boulevard' AND ownerId = @ownerId AND managerId = @managerId
)
    INSERT INTO dbo.buildings ([address], [city], [province], [postalCode], [imagePath], [ownerId], [managerId])
    VALUES (N'123 Main Boulevard', N'Karachi', N'Sindh', N'75500', NULL, @ownerId, @managerId);

DECLARE @buildingId INT = (
    SELECT TOP 1 buildingId FROM dbo.buildings
    WHERE ownerId = @ownerId AND managerId = @managerId
    ORDER BY buildingId
);

IF NOT EXISTS (SELECT 1 FROM dbo.apartments WHERE buildingId = @buildingId AND apartmentNo = 101)
    INSERT INTO dbo.apartments ([apartmentNo], [nbRooms], [price], [status], [buildingId], [tenantId])
    VALUES (101, 2, 1200.0000, N'Available', @buildingId, NULL);

IF NOT EXISTS (SELECT 1 FROM dbo.apartments WHERE buildingId = @buildingId AND apartmentNo = 102)
    INSERT INTO dbo.apartments ([apartmentNo], [nbRooms], [price], [status], [buildingId], [tenantId])
    VALUES (102, 3, 1850.0000, N'Booked', @buildingId, @tenantId);

PRINT 'Seed complete. Test logins (password: password123):';
PRINT '  Manager: ednamode@gmail.com';
PRINT '  Owner:   tylerdurden@gmail.com';
PRINT '  Tenant:  michaelcorleone@gmail.com';
GO

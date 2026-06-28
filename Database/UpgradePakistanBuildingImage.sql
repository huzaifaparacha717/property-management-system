/*
  Idempotent upgrades for existing PropertyRentalDB instances (skip if already applied).
*/
USE [PropertyRentalDB];
GO

IF COL_LENGTH('dbo.buildings', 'imagePath') IS NULL
    ALTER TABLE dbo.buildings ADD [imagePath] NVARCHAR(500) NULL;
GO

/* COL_LENGTH for nvarchar(n) is 2*n bytes, so nvarchar(10) => 20 — do not compare to 10. */
IF COL_LENGTH('dbo.managers', 'phoneNumber') IS NOT NULL
   AND COL_LENGTH('dbo.managers', 'phoneNumber') < 30
    ALTER TABLE dbo.managers ALTER COLUMN [phoneNumber] NVARCHAR(15) NOT NULL;
GO
IF COL_LENGTH('dbo.owners', 'phoneNumber') IS NOT NULL
   AND COL_LENGTH('dbo.owners', 'phoneNumber') < 30
    ALTER TABLE dbo.owners ALTER COLUMN [phoneNumber] NVARCHAR(15) NOT NULL;
GO
IF COL_LENGTH('dbo.tenants', 'phoneNumber') IS NOT NULL
   AND COL_LENGTH('dbo.tenants', 'phoneNumber') < 30
    ALTER TABLE dbo.tenants ALTER COLUMN [phoneNumber] NVARCHAR(15) NOT NULL;
GO

PRINT 'UpgradePakistanBuildingImage.sql completed.';
GO

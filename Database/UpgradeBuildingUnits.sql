/*
  Idempotent: total / advertised available units on buildings (for list when no apartment rows yet).
*/
USE [PropertyRentalDB];
GO

IF COL_LENGTH('dbo.buildings', 'totalUnits') IS NULL
    ALTER TABLE dbo.buildings ADD [totalUnits] INT NULL;
GO

IF COL_LENGTH('dbo.buildings', 'availableUnits') IS NULL
    ALTER TABLE dbo.buildings ADD [availableUnits] INT NULL;
GO

PRINT 'UpgradeBuildingUnits.sql completed.';
GO

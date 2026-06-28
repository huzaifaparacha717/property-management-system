
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/12/2026 02:27:44
-- Generated from EDMX file: C:\Users\Aftab Gujjar\Downloads\Property-Rental-Management-main\Property-Rental-Management-main\prjRentalManagement\Models\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [PropertyRentalDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_apartment_building]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[apartments] DROP CONSTRAINT [FK_apartment_building];
GO
IF OBJECT_ID(N'[dbo].[FK_apartment_tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[apartments] DROP CONSTRAINT [FK_apartment_tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_appointment_manager]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[appointments] DROP CONSTRAINT [FK_appointment_manager];
GO
IF OBJECT_ID(N'[dbo].[FK_appointment_tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[appointments] DROP CONSTRAINT [FK_appointment_tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_building_manager]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[buildings] DROP CONSTRAINT [FK_building_manager];
GO
IF OBJECT_ID(N'[dbo].[FK_building_owner]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[buildings] DROP CONSTRAINT [FK_building_owner];
GO
IF OBJECT_ID(N'[dbo].[FK_eventOwner_apartment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[eventOwners] DROP CONSTRAINT [FK_eventOwner_apartment];
GO
IF OBJECT_ID(N'[dbo].[FK_eventOwner_manager]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[eventOwners] DROP CONSTRAINT [FK_eventOwner_manager];
GO
IF OBJECT_ID(N'[dbo].[FK_eventOwner_owner]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[eventOwners] DROP CONSTRAINT [FK_eventOwner_owner];
GO
IF OBJECT_ID(N'[dbo].[FK_booking_apartment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[bookings] DROP CONSTRAINT [FK_booking_apartment];
GO
IF OBJECT_ID(N'[dbo].[FK_booking_tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[bookings] DROP CONSTRAINT [FK_booking_tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_messageManager_manager]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[messageManagers] DROP CONSTRAINT [FK_messageManager_manager];
GO
IF OBJECT_ID(N'[dbo].[FK_messageManager_tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[messageManagers] DROP CONSTRAINT [FK_messageManager_tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_messageOwner_manager]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[messageOwners] DROP CONSTRAINT [FK_messageOwner_manager];
GO
IF OBJECT_ID(N'[dbo].[FK_messageOwner_owner]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[messageOwners] DROP CONSTRAINT [FK_messageOwner_owner];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[eventOwners]', 'U') IS NOT NULL
    DROP TABLE [dbo].[eventOwners];
GO
IF OBJECT_ID(N'[dbo].[messageManagers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[messageManagers];
GO
IF OBJECT_ID(N'[dbo].[messageOwners]', 'U') IS NOT NULL
    DROP TABLE [dbo].[messageOwners];
GO
IF OBJECT_ID(N'[dbo].[appointments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[appointments];
GO
IF OBJECT_ID(N'[dbo].[bookings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[bookings];
GO
IF OBJECT_ID(N'[dbo].[apartments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[apartments];
GO
IF OBJECT_ID(N'[dbo].[buildings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[buildings];
GO
IF OBJECT_ID(N'[dbo].[managers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[managers];
GO
IF OBJECT_ID(N'[dbo].[owners]', 'U') IS NOT NULL
    DROP TABLE [dbo].[owners];
GO
IF OBJECT_ID(N'[dbo].[tenants]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tenants];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'apartments'
CREATE TABLE [dbo].[apartments] (
    [apartmentId] int IDENTITY(1,1) NOT NULL,
    [apartmentNo] int  NOT NULL,
    [nbRooms] int  NOT NULL,
    [price] decimal(19,4)  NOT NULL,
    [status] nvarchar(20)  NOT NULL,
    [buildingId] int  NOT NULL,
    [tenantId] int  NULL
);
GO

-- Creating table 'appointments'
CREATE TABLE [dbo].[appointments] (
    [appointmentId] int IDENTITY(1,1) NOT NULL,
    [managerId] int  NOT NULL,
    [tenantId] int  NOT NULL,
    [appointmentDate] datetime  NOT NULL,
    [description] nvarchar(255)  NOT NULL
);
GO

-- Creating table 'buildings'
CREATE TABLE [dbo].[buildings] (
    [buildingId] int IDENTITY(1,1) NOT NULL,
    [address] nvarchar(50)  NOT NULL,
    [city] nvarchar(50)  NOT NULL,
    [province] nvarchar(50)  NOT NULL,
    [postalCode] nvarchar(10)  NOT NULL,
    [imagePath] nvarchar(500)  NULL,
    [ownerId] int  NOT NULL,
    [managerId] int  NOT NULL,
    [totalUnits] int  NULL,
    [availableUnits] int  NULL
);
GO

-- Creating table 'managers'
CREATE TABLE [dbo].[managers] (
    [managerId] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(50)  NOT NULL,
    [email] nvarchar(50)  NOT NULL,
    [password] nvarchar(64)  NOT NULL,
    [phoneNumber] nvarchar(15)  NOT NULL
);
GO

-- Creating table 'messageManagers'
CREATE TABLE [dbo].[messageManagers] (
    [messageId] int IDENTITY(1,1) NOT NULL,
    [managerId] int  NOT NULL,
    [tenantId] int  NOT NULL,
    [message] nvarchar(100)  NOT NULL,
    [responseMessage] nvarchar(100)  NULL
);
GO

-- Creating table 'messageOwners'
CREATE TABLE [dbo].[messageOwners] (
    [messageId] int IDENTITY(1,1) NOT NULL,
    [ownerId] int  NOT NULL,
    [managerId] int  NOT NULL,
    [message] nvarchar(100)  NOT NULL,
    [responseMessage] nvarchar(100)  NULL
);
GO

-- Creating table 'owners'
CREATE TABLE [dbo].[owners] (
    [ownerId] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(50)  NOT NULL,
    [email] nvarchar(50)  NOT NULL,
    [password] nvarchar(64)  NOT NULL,
    [phoneNumber] nvarchar(15)  NOT NULL
);
GO

-- Creating table 'tenants'
CREATE TABLE [dbo].[tenants] (
    [tenantId] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(50)  NOT NULL,
    [email] nvarchar(50)  NOT NULL,
    [password] nvarchar(64)  NOT NULL,
    [phoneNumber] nvarchar(15)  NOT NULL
);
GO

-- Creating table 'bookings'
CREATE TABLE [dbo].[bookings] (
    [bookingId] int IDENTITY(1,1) NOT NULL,
    [apartmentId] int  NOT NULL,
    [tenantId] int  NOT NULL,
    [status] nvarchar(20)  NOT NULL,
    [requestDate] datetime  NOT NULL
);
GO

-- Creating table 'eventOwners'
CREATE TABLE [dbo].[eventOwners] (
    [eventId] int IDENTITY(1,1) NOT NULL,
    [managerId] int  NOT NULL,
    [ownerId] int  NOT NULL,
    [apartmentId] int  NOT NULL,
    [eventDate] datetime  NOT NULL,
    [description] nvarchar(255)  NOT NULL,
    [status] nvarchar(50)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [apartmentId] in table 'apartments'
ALTER TABLE [dbo].[apartments]
ADD CONSTRAINT [PK_apartments]
    PRIMARY KEY CLUSTERED ([apartmentId] ASC);
GO

-- Creating primary key on [appointmentId] in table 'appointments'
ALTER TABLE [dbo].[appointments]
ADD CONSTRAINT [PK_appointments]
    PRIMARY KEY CLUSTERED ([appointmentId] ASC);
GO

-- Creating primary key on [buildingId] in table 'buildings'
ALTER TABLE [dbo].[buildings]
ADD CONSTRAINT [PK_buildings]
    PRIMARY KEY CLUSTERED ([buildingId] ASC);
GO

-- Creating primary key on [managerId] in table 'managers'
ALTER TABLE [dbo].[managers]
ADD CONSTRAINT [PK_managers]
    PRIMARY KEY CLUSTERED ([managerId] ASC);
GO

-- Creating primary key on [messageId] in table 'messageManagers'
ALTER TABLE [dbo].[messageManagers]
ADD CONSTRAINT [PK_messageManagers]
    PRIMARY KEY CLUSTERED ([messageId] ASC);
GO

-- Creating primary key on [messageId] in table 'messageOwners'
ALTER TABLE [dbo].[messageOwners]
ADD CONSTRAINT [PK_messageOwners]
    PRIMARY KEY CLUSTERED ([messageId] ASC);
GO

-- Creating primary key on [ownerId] in table 'owners'
ALTER TABLE [dbo].[owners]
ADD CONSTRAINT [PK_owners]
    PRIMARY KEY CLUSTERED ([ownerId] ASC);
GO

-- Creating primary key on [tenantId] in table 'tenants'
ALTER TABLE [dbo].[tenants]
ADD CONSTRAINT [PK_tenants]
    PRIMARY KEY CLUSTERED ([tenantId] ASC);
GO

-- Creating primary key on [bookingId] in table 'bookings'
ALTER TABLE [dbo].[bookings]
ADD CONSTRAINT [PK_bookings]
    PRIMARY KEY CLUSTERED ([bookingId] ASC);
GO

-- Creating primary key on [eventId] in table 'eventOwners'
ALTER TABLE [dbo].[eventOwners]
ADD CONSTRAINT [PK_eventOwners]
    PRIMARY KEY CLUSTERED ([eventId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [buildingId] in table 'apartments'
ALTER TABLE [dbo].[apartments]
ADD CONSTRAINT [FK_apartment_building]
    FOREIGN KEY ([buildingId])
    REFERENCES [dbo].[buildings]
        ([buildingId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_apartment_building'
CREATE INDEX [IX_FK_apartment_building]
ON [dbo].[apartments]
    ([buildingId]);
GO

-- Creating foreign key on [tenantId] in table 'apartments'
ALTER TABLE [dbo].[apartments]
ADD CONSTRAINT [FK_apartment_tenant]
    FOREIGN KEY ([tenantId])
    REFERENCES [dbo].[tenants]
        ([tenantId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_apartment_tenant'
CREATE INDEX [IX_FK_apartment_tenant]
ON [dbo].[apartments]
    ([tenantId]);
GO

-- Creating foreign key on [apartmentId] in table 'bookings'
ALTER TABLE [dbo].[bookings]
ADD CONSTRAINT [FK_booking_apartment]
    FOREIGN KEY ([apartmentId])
    REFERENCES [dbo].[apartments]
        ([apartmentId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_booking_apartment]
ON [dbo].[bookings]
    ([apartmentId]);
GO

-- Creating foreign key on [tenantId] in table 'bookings'
ALTER TABLE [dbo].[bookings]
ADD CONSTRAINT [FK_booking_tenant]
    FOREIGN KEY ([tenantId])
    REFERENCES [dbo].[tenants]
        ([tenantId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

CREATE INDEX [IX_FK_booking_tenant]
ON [dbo].[bookings]
    ([tenantId]);
GO

-- Creating foreign key on [managerId] in table 'appointments'
ALTER TABLE [dbo].[appointments]
ADD CONSTRAINT [FK_appointment_manager]
    FOREIGN KEY ([managerId])
    REFERENCES [dbo].[managers]
        ([managerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_appointment_manager'
CREATE INDEX [IX_FK_appointment_manager]
ON [dbo].[appointments]
    ([managerId]);
GO

-- Creating foreign key on [tenantId] in table 'appointments'
ALTER TABLE [dbo].[appointments]
ADD CONSTRAINT [FK_appointment_tenant]
    FOREIGN KEY ([tenantId])
    REFERENCES [dbo].[tenants]
        ([tenantId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_appointment_tenant'
CREATE INDEX [IX_FK_appointment_tenant]
ON [dbo].[appointments]
    ([tenantId]);
GO

-- Creating foreign key on [managerId] in table 'buildings'
ALTER TABLE [dbo].[buildings]
ADD CONSTRAINT [FK_building_manager]
    FOREIGN KEY ([managerId])
    REFERENCES [dbo].[managers]
        ([managerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_building_manager'
CREATE INDEX [IX_FK_building_manager]
ON [dbo].[buildings]
    ([managerId]);
GO

-- Creating foreign key on [ownerId] in table 'buildings'
ALTER TABLE [dbo].[buildings]
ADD CONSTRAINT [FK_building_owner]
    FOREIGN KEY ([ownerId])
    REFERENCES [dbo].[owners]
        ([ownerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_building_owner'
CREATE INDEX [IX_FK_building_owner]
ON [dbo].[buildings]
    ([ownerId]);
GO

-- Creating foreign key on [managerId] in table 'messageManagers'
ALTER TABLE [dbo].[messageManagers]
ADD CONSTRAINT [FK_messageManager_manager]
    FOREIGN KEY ([managerId])
    REFERENCES [dbo].[managers]
        ([managerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_messageManager_manager'
CREATE INDEX [IX_FK_messageManager_manager]
ON [dbo].[messageManagers]
    ([managerId]);
GO

-- Creating foreign key on [managerId] in table 'messageOwners'
ALTER TABLE [dbo].[messageOwners]
ADD CONSTRAINT [FK_messageOwner_manager]
    FOREIGN KEY ([managerId])
    REFERENCES [dbo].[managers]
        ([managerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_messageOwner_manager'
CREATE INDEX [IX_FK_messageOwner_manager]
ON [dbo].[messageOwners]
    ([managerId]);
GO

-- Creating foreign key on [tenantId] in table 'messageManagers'
ALTER TABLE [dbo].[messageManagers]
ADD CONSTRAINT [FK_messageManager_tenant]
    FOREIGN KEY ([tenantId])
    REFERENCES [dbo].[tenants]
        ([tenantId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_messageManager_tenant'
CREATE INDEX [IX_FK_messageManager_tenant]
ON [dbo].[messageManagers]
    ([tenantId]);
GO

-- Creating foreign key on [ownerId] in table 'messageOwners'
ALTER TABLE [dbo].[messageOwners]
ADD CONSTRAINT [FK_messageOwner_owner]
    FOREIGN KEY ([ownerId])
    REFERENCES [dbo].[owners]
        ([ownerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_messageOwner_owner'
CREATE INDEX [IX_FK_messageOwner_owner]
ON [dbo].[messageOwners]
    ([ownerId]);
GO

-- Creating foreign key on [apartmentId] in table 'eventOwners'
ALTER TABLE [dbo].[eventOwners]
ADD CONSTRAINT [FK_eventOwner_apartment]
    FOREIGN KEY ([apartmentId])
    REFERENCES [dbo].[apartments]
        ([apartmentId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_eventOwner_apartment'
CREATE INDEX [IX_FK_eventOwner_apartment]
ON [dbo].[eventOwners]
    ([apartmentId]);
GO

-- Creating foreign key on [managerId] in table 'eventOwners'
ALTER TABLE [dbo].[eventOwners]
ADD CONSTRAINT [FK_eventOwner_manager]
    FOREIGN KEY ([managerId])
    REFERENCES [dbo].[managers]
        ([managerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_eventOwner_manager'
CREATE INDEX [IX_FK_eventOwner_manager]
ON [dbo].[eventOwners]
    ([managerId]);
GO

-- Creating foreign key on [ownerId] in table 'eventOwners'
ALTER TABLE [dbo].[eventOwners]
ADD CONSTRAINT [FK_eventOwner_owner]
    FOREIGN KEY ([ownerId])
    REFERENCES [dbo].[owners]
        ([ownerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_eventOwner_owner'
CREATE INDEX [IX_FK_eventOwner_owner]
ON [dbo].[eventOwners]
    ([ownerId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
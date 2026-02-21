USE [RepairDb]
GO

SET NOCOUNT ON;
GO

DECLARE @Now DATETIME = GETUTCDATE();
DECLARE @SystemUser VARCHAR(100) = 'SYSTEM';

-- DRAFT
IF NOT EXISTS (SELECT 1 FROM [bkg].[BookingStatus] WHERE [StatusName] = 'DRAFT')
BEGIN
    INSERT INTO [bkg].[BookingStatus]
    ([StatusName], [IsDeleted], [CreatedAt], [CreatedBy])
    VALUES
    ('DRAFT', 0, @Now, @SystemUser);
END

-- SCHEDULED
IF NOT EXISTS (SELECT 1 FROM [bkg].[BookingStatus] WHERE [StatusName] = 'SCHEDULED')
BEGIN
    INSERT INTO [bkg].[BookingStatus]
    ([StatusName], [IsDeleted], [CreatedAt], [CreatedBy])
    VALUES
    ('SCHEDULED', 0, @Now, @SystemUser);
END

-- CONFIRMED
IF NOT EXISTS (SELECT 1 FROM [bkg].[BookingStatus] WHERE [StatusName] = 'CONFIRMED')
BEGIN
    INSERT INTO [bkg].[BookingStatus]
    ([StatusName], [IsDeleted], [CreatedAt], [CreatedBy])
    VALUES
    ('CONFIRMED', 0, @Now, @SystemUser);
END

-- RESCHEDULED
IF NOT EXISTS (SELECT 1 FROM [bkg].[BookingStatus] WHERE [StatusName] = 'RESCHEDULED')
BEGIN
    INSERT INTO [bkg].[BookingStatus]
    ([StatusName], [IsDeleted], [CreatedAt], [CreatedBy])
    VALUES
    ('RESCHEDULED', 0, @Now, @SystemUser);
END

-- CHECKED_IN
IF NOT EXISTS (SELECT 1 FROM [bkg].[BookingStatus] WHERE [StatusName] = 'CHECKED_IN')
BEGIN
    INSERT INTO [bkg].[BookingStatus]
    ([StatusName], [IsDeleted], [CreatedAt], [CreatedBy])
    VALUES
    ('CHECKED_IN', 0, @Now, @SystemUser);
END

-- IN_PROGRESS
IF NOT EXISTS (SELECT 1 FROM [bkg].[BookingStatus] WHERE [StatusName] = 'IN_PROGRESS')
BEGIN
    INSERT INTO [bkg].[BookingStatus]
    ([StatusName], [IsDeleted], [CreatedAt], [CreatedBy])
    VALUES
    ('IN_PROGRESS', 0, @Now, @SystemUser);
END

-- READY_FOR_DELIVERY
IF NOT EXISTS (SELECT 1 FROM [bkg].[BookingStatus] WHERE [StatusName] = 'READY_FOR_DELIVERY')
BEGIN
    INSERT INTO [bkg].[BookingStatus]
    ([StatusName], [IsDeleted], [CreatedAt], [CreatedBy])
    VALUES
    ('READY_FOR_DELIVERY', 0, @Now, @SystemUser);
END

-- COMPLETED
IF NOT EXISTS (SELECT 1 FROM [bkg].[BookingStatus] WHERE [StatusName] = 'COMPLETED')
BEGIN
    INSERT INTO [bkg].[BookingStatus]
    ([StatusName], [IsDeleted], [CreatedAt], [CreatedBy])
    VALUES
    ('COMPLETED', 0, @Now, @SystemUser);
END

-- CANCELLED
IF NOT EXISTS (SELECT 1 FROM [bkg].[BookingStatus] WHERE [StatusName] = 'CANCELLED')
BEGIN
    INSERT INTO [bkg].[BookingStatus]
    ([StatusName], [IsDeleted], [CreatedAt], [CreatedBy])
    VALUES
    ('CANCELLED', 0, @Now, @SystemUser);
END

-- NO_SHOW
IF NOT EXISTS (SELECT 1 FROM [bkg].[BookingStatus] WHERE [StatusName] = 'NO_SHOW')
BEGIN
    INSERT INTO [bkg].[BookingStatus]
    ([StatusName], [IsDeleted], [CreatedAt], [CreatedBy])
    VALUES
    ('NO_SHOW', 0, @Now, @SystemUser);
END

GO
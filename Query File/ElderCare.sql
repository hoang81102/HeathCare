-- ElderCare Database Creation Script
-- Drop database if it exists and create a new one
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = 'ElderCare')
BEGIN
    USE master;
    ALTER DATABASE ElderCare SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ElderCare;
END
GO

CREATE DATABASE ElderCare;
GO

USE ElderCare;
GO

-- Create base tables
CREATE TABLE Role (
    roleId INT PRIMARY KEY IDENTITY(1,1),
    roleName VARCHAR(50) NOT NULL
);

CREATE TABLE Account (
    accountId INT PRIMARY KEY IDENTITY(1,1),
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    phone VARCHAR(15),
    email VARCHAR(100) UNIQUE,
    fullname VARCHAR(100) NOT NULL,
    address TEXT,
    birthdate DATE,
    hobby TEXT,
    accountStatus VARCHAR(20) CHECK (accountStatus IN ('active', 'inactive')) NOT NULL,
    roleId INT NOT NULL,
    FOREIGN KEY (roleId) REFERENCES Role(roleId)
);

-- Create ServiceCategory table before Service table
CREATE TABLE ServiceCategory (
    categoryId INT PRIMARY KEY IDENTITY(1,1),
    categoryName VARCHAR(50) NOT NULL CHECK (categoryName IN ('Health Care', 'Personal Care', 'Emergency Care'))
);

CREATE TABLE Service (
    serviceId INT PRIMARY KEY IDENTITY(1,1),
    serviceName VARCHAR(100) NOT NULL,
    description TEXT,
    price DECIMAL(10,2) NOT NULL,
    categoryId INT NOT NULL,
    FOREIGN KEY (categoryId) REFERENCES ServiceCategory(categoryId)
);

CREATE TABLE Elder (
    elderId INT PRIMARY KEY IDENTITY(1,1),
    accountId INT NOT NULL,
    fullname VARCHAR(100) NOT NULL,
    phone VARCHAR(15),
    address TEXT,
    birthdate DATE,
    hobby TEXT,
    medicalNote TEXT,
    FOREIGN KEY (accountId) REFERENCES Account(accountId)
);

CREATE TABLE Caregiver (
    caregiverId INT PRIMARY KEY IDENTITY(1,1),
    accountId INT NOT NULL,
    experienceYears INT NOT NULL,
    specialty VARCHAR(100),
    certification VARCHAR(255),
    availability VARCHAR(50) CHECK (availability IN ('full-time', 'part-time', 'on-call')) NOT NULL,
    FOREIGN KEY (accountId) REFERENCES Account(accountId)
);

-- Create new CaregiverAvailability table
CREATE TABLE CaregiverAvailability (
    availabilityId INT PRIMARY KEY IDENTITY(1,1),
    caregiverId INT NOT NULL,
    dayOfWeek INT NOT NULL, -- 1-7 for Monday-Sunday
    startTime TIME NOT NULL,
    endTime TIME NOT NULL,
    isAvailable BIT DEFAULT 1,
    FOREIGN KEY (caregiverId) REFERENCES Caregiver(caregiverId),
    CONSTRAINT UC_CaregiverSchedule UNIQUE (caregiverId, dayOfWeek, startTime, endTime)
);

-- Create Booking table with updated status options
CREATE TABLE Booking (
    bookingId INT PRIMARY KEY IDENTITY(1,1),
    accountId INT NOT NULL,
    serviceId INT NOT NULL,
    caregiverId INT NOT NULL,
    elderId INT NULL,
    bookingDateTime DATETIME NOT NULL,
    status VARCHAR(20) CHECK (status IN ('pending', 'accepted', 'rejected', 'in-progress', 'completed', 'canceled')) NOT NULL,
    rejectionReason TEXT NULL,
    FOREIGN KEY (accountId) REFERENCES Account(accountId),
    FOREIGN KEY (serviceId) REFERENCES Service(serviceId),
    FOREIGN KEY (caregiverId) REFERENCES Caregiver(caregiverId),
    FOREIGN KEY (elderId) REFERENCES Elder(elderId)
);

-- Create BookingTimeSlot table for more detailed scheduling
CREATE TABLE BookingTimeSlot (
    slotId INT PRIMARY KEY IDENTITY(1,1),
    bookingId INT NOT NULL,
    bookingDate DATE NOT NULL,
    startTime TIME NOT NULL,
    endTime TIME NOT NULL,
    FOREIGN KEY (bookingId) REFERENCES Booking(bookingId)
);

-- Create enhanced Record table
CREATE TABLE Record (
    recordId INT PRIMARY KEY IDENTITY(1,1),
    elderId INT NOT NULL,
    description TEXT NOT NULL,
    bookingId INT NOT NULL,
    status VARCHAR(50) CHECK (status IN ('Accepted', 'InProgress', 'Finished')) NOT NULL,
    last_updated DATETIME DEFAULT GETDATE(),
    clockInTime DATETIME NULL,
    clockOutTime DATETIME NULL,
    exerciseGuidelines TEXT NULL,
    dietGuidelines TEXT NULL,
    otherGuidelines TEXT NULL,
    FOREIGN KEY (elderId) REFERENCES Elder(elderId),
    FOREIGN KEY (bookingId) REFERENCES Booking(bookingId)
);

-- Create enhanced Tracking table
CREATE TABLE Tracking (
    trackingId INT PRIMARY KEY IDENTITY(1,1),
    elderId INT NOT NULL,
    date DATE NOT NULL,
    weight DECIMAL(5,2),
    bloodPressure VARCHAR(20),
    FOREIGN KEY (elderId) REFERENCES Elder(elderId)
);

-- Create enhanced Feedback table
CREATE TABLE Feedback (
    feedbackId INT PRIMARY KEY IDENTITY(1,1),
    bookingId INT NOT NULL,
    note TEXT,
    rating INT CHECK (rating BETWEEN 1 AND 5),
    feedbackDate DATETIME DEFAULT GETDATE(),
    caregiverProfessionalism INT CHECK (caregiverProfessionalism BETWEEN 1 AND 5),
    serviceQuality INT CHECK (serviceQuality BETWEEN 1 AND 5),
    overallExperience INT CHECK (overallExperience BETWEEN 1 AND 5),
    FOREIGN KEY (bookingId) REFERENCES Booking(bookingId)
);

-- Create new MedicalRecord table
CREATE TABLE MedicalRecord (
    medicalRecordId INT PRIMARY KEY IDENTITY(1,1),
    elderId INT NOT NULL,
    recordDate DATE NOT NULL,
    diagnosis TEXT,
    medications TEXT,
    allergies TEXT,
    chronicConditions TEXT,
    FOREIGN KEY (elderId) REFERENCES Elder(elderId)
);

-- Insert initial data
INSERT INTO Role (roleName) VALUES ('Admin'), ('Customer'), ('Caregiver');

-- Insert service categories
INSERT INTO ServiceCategory (categoryName)
VALUES ('Health Care'), ('Personal Care'), ('Emergency Care');

-- Admin account (password: admin123)
INSERT INTO Account (username, password, fullname, accountStatus, roleId)
VALUES ('admin', 'e5a93371cfc7d2e4bd3fd84f3d304985d63fce1773810316177a710b3e5b84b7', 'System Administrator', 'active', 1);

GO

-- Create stored procedures for common operations

-- Procedure to get all available caregivers for a specific time
CREATE PROCEDURE GetAvailableCaregivers
    @bookingDate DATE,
    @startTime TIME,
    @endTime TIME
AS
BEGIN
    SELECT c.caregiverId, a.fullname
    FROM Caregiver c
    JOIN Account a ON c.accountId = a.accountId
    JOIN CaregiverAvailability ca ON c.caregiverId = ca.caregiverId
    WHERE a.accountStatus = 'active'
    AND ca.dayOfWeek = DATEPART(WEEKDAY, @bookingDate)
    AND ca.startTime <= @startTime
    AND ca.endTime >= @endTime
    AND ca.isAvailable = 1
    AND c.caregiverId NOT IN (
        SELECT DISTINCT b.caregiverId
        FROM Booking b
        JOIN BookingTimeSlot bts ON b.bookingId = bts.bookingId
        WHERE bts.bookingDate = @bookingDate
        AND (
            (bts.startTime <= @startTime AND bts.endTime > @startTime) OR
            (bts.startTime < @endTime AND bts.endTime >= @endTime) OR
            (bts.startTime >= @startTime AND bts.endTime <= @endTime)
        )
        AND b.status IN ('pending', 'accepted', 'in-progress')
    )
END
GO

-- Procedure to get available time slots for a specific caregiver
CREATE PROCEDURE GetCaregiverAvailability
    @caregiverId INT,
    @bookingDate DATE
AS
BEGIN
    SELECT ca.startTime, ca.endTime
    FROM CaregiverAvailability ca
    WHERE ca.caregiverId = @caregiverId
    AND ca.dayOfWeek = DATEPART(WEEKDAY, @bookingDate)
    AND ca.isAvailable = 1
    AND NOT EXISTS (
        SELECT 1
        FROM Booking b
        JOIN BookingTimeSlot bts ON b.bookingId = bts.bookingId
        WHERE b.caregiverId = @caregiverId
        AND bts.bookingDate = @bookingDate
        AND (
            (bts.startTime <= ca.startTime AND bts.endTime > ca.startTime) OR
            (bts.startTime < ca.endTime AND bts.endTime >= ca.endTime) OR
            (bts.startTime >= ca.startTime AND bts.endTime <= ca.endTime)
        )
        AND b.status IN ('pending', 'accepted', 'in-progress')
    )
END
GO

-- Procedure to create a new booking with time slot
CREATE PROCEDURE CreateBookingWithTimeSlot
    @accountId INT,
    @serviceId INT,
    @caregiverId INT,
    @elderId INT,
    @bookingDate DATE,
    @startTime TIME,
    @endTime TIME
AS
BEGIN
    BEGIN TRANSACTION;
    
    DECLARE @bookingId INT;
    
    -- Create the booking
    INSERT INTO Booking (accountId, serviceId, caregiverId, elderId, bookingDateTime, status)
    VALUES (@accountId, @serviceId, @caregiverId, @elderId, @bookingDate, 'pending');
    
    SET @bookingId = SCOPE_IDENTITY();
    
    -- Create the time slot
    INSERT INTO BookingTimeSlot (bookingId, bookingDate, startTime, endTime)
    VALUES (@bookingId, @bookingDate, @startTime, @endTime);
    
    COMMIT TRANSACTION;
    
    SELECT @bookingId AS NewBookingId;
END
GO

-- Procedure to accept or reject a booking
CREATE PROCEDURE UpdateBookingStatus
    @bookingId INT,
    @newStatus VARCHAR(20),
    @rejectionReason TEXT = NULL
AS
BEGIN
    BEGIN TRANSACTION;
    
    UPDATE Booking
    SET status = @newStatus,
        rejectionReason = CASE WHEN @newStatus = 'rejected' THEN @rejectionReason ELSE rejectionReason END
    WHERE bookingId = @bookingId;
    
    -- If accepted, create a record
    IF @newStatus = 'accepted'
    BEGIN
        DECLARE @elderId INT;
        
        SELECT @elderId = elderId
        FROM Booking
        WHERE bookingId = @bookingId;
        
        INSERT INTO Record (elderId, bookingId, description, status)
        VALUES (@elderId, @bookingId, 'Booking accepted', 'Accepted');
    END
    
    COMMIT TRANSACTION;
END
GO

-- Procedure to update record status (clock in/out)
CREATE PROCEDURE UpdateRecordStatus
    @recordId INT,
    @newStatus VARCHAR(50)
AS
BEGIN
    UPDATE Record
    SET status = @newStatus,
        clockInTime = CASE WHEN @newStatus = 'InProgress' THEN GETDATE() ELSE clockInTime END,
        clockOutTime = CASE WHEN @newStatus = 'Finished' THEN GETDATE() ELSE clockOutTime END,
        last_updated = GETDATE()
    WHERE recordId = @recordId;
END
GO

-- Procedure to update record guidelines
CREATE PROCEDURE UpdateRecordGuidelines
    @recordId INT,
    @exerciseGuidelines TEXT = NULL,
    @dietGuidelines TEXT = NULL,
    @otherGuidelines TEXT = NULL
AS
BEGIN
    UPDATE Record
    SET exerciseGuidelines = ISNULL(@exerciseGuidelines, exerciseGuidelines),
        dietGuidelines = ISNULL(@dietGuidelines, dietGuidelines),
        otherGuidelines = ISNULL(@otherGuidelines, otherGuidelines),
        last_updated = GETDATE()
    WHERE recordId = @recordId;
END
GO

-- Procedure to add feedback for a booking
CREATE PROCEDURE AddFeedback
    @bookingId INT,
    @note TEXT = NULL,
    @rating INT,
    @caregiverProfessionalism INT,
    @serviceQuality INT,
    @overallExperience INT
AS
BEGIN
    INSERT INTO Feedback (bookingId, note, rating, caregiverProfessionalism, serviceQuality, overallExperience)
    VALUES (@bookingId, @note, @rating, @caregiverProfessionalism, @serviceQuality, @overallExperience);
END
GO

-- Create views for common queries

-- View to get customer information with their elders
CREATE VIEW CustomerElders AS
SELECT a.accountId, a.fullname AS customerName, a.email, a.phone,
       e.elderId, e.fullname AS elderName, e.birthdate, e.medicalNote
FROM Account a
JOIN Elder e ON a.accountId = e.accountId
WHERE a.roleId = 2; -- Customer role
GO

-- View to get caregiver information with their schedule
CREATE VIEW CaregiverSchedules AS
SELECT a.accountId, a.fullname, c.caregiverId, c.experienceYears, c.specialty,
       ca.dayOfWeek, ca.startTime, ca.endTime, ca.isAvailable
FROM Account a
JOIN Caregiver c ON a.accountId = c.accountId
LEFT JOIN CaregiverAvailability ca ON c.caregiverId = ca.caregiverId
WHERE a.roleId = 3; -- Caregiver role
GO

-- View to get booking information with related details
CREATE VIEW BookingDetails AS
SELECT b.bookingId, b.status, 
       a.fullname AS customerName, 
       e.fullname AS elderName,
       ca.fullname AS caregiverName,
       s.serviceName, sc.categoryName,
       bts.bookingDate, bts.startTime, bts.endTime,
       b.rejectionReason
FROM Booking b
JOIN Account a ON b.accountId = a.accountId
JOIN Elder e ON b.elderId = e.elderId
JOIN Caregiver cg ON b.caregiverId = cg.caregiverId
JOIN Account ca ON cg.accountId = ca.accountId
JOIN Service s ON b.serviceId = s.serviceId
JOIN ServiceCategory sc ON s.categoryId = sc.categoryId
JOIN BookingTimeSlot bts ON b.bookingId = bts.bookingId;
GO

-- View to get completed services with feedback
CREATE VIEW ServiceFeedback AS
SELECT b.bookingId, a.fullname AS customerName, e.fullname AS elderName,
       ca.fullname AS caregiverName, s.serviceName,
       f.rating, f.caregiverProfessionalism, f.serviceQuality, f.overallExperience,
       f.note, f.feedbackDate
FROM Booking b
JOIN Account a ON b.accountId = a.accountId
JOIN Elder e ON b.elderId = e.elderId
JOIN Caregiver cg ON b.caregiverId = cg.caregiverId
JOIN Account ca ON cg.accountId = ca.accountId
JOIN Service s ON b.serviceId = s.serviceId
JOIN Feedback f ON b.bookingId = f.bookingId
WHERE b.status = 'completed';
GO

-- Create indexes for performance optimization

-- Indexes for Account table
CREATE INDEX IX_Account_RoleId ON Account(roleId);
CREATE INDEX IX_Account_Status ON Account(accountStatus);

-- Indexes for Booking table 
CREATE INDEX IX_Booking_AccountId ON Booking(accountId);
CREATE INDEX IX_Booking_CaregiverId ON Booking(caregiverId);
CREATE INDEX IX_Booking_ElderId ON Booking(elderId);
CREATE INDEX IX_Booking_Status ON Booking(status);

-- Indexes for Record table
CREATE INDEX IX_Record_BookingId ON Record(bookingId);
CREATE INDEX IX_Record_ElderId ON Record(elderId);
CREATE INDEX IX_Record_Status ON Record(status);

-- Indexes for CaregiverAvailability table
CREATE INDEX IX_CaregiverAvailability_DayTime ON CaregiverAvailability(caregiverId, dayOfWeek, startTime, endTime);

-- Indexes for BookingTimeSlot table
CREATE INDEX IX_BookingTimeSlot_BookingDate ON BookingTimeSlot(bookingDate, startTime, endTime);
CREATE INDEX IX_BookingTimeSlot_BookingId ON BookingTimeSlot(bookingId);

PRINT 'ElderCare database created successfully.';
GO


-- dotnet ef dbcontext scaffold "Server=localhost;uid=sa;pwd=12345;database=ElderCare;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer --project ..\businessObjects --context-dir .--
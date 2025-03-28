
USE ElderCare

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

CREATE TABLE Service (
    serviceId INT PRIMARY KEY IDENTITY(1,1),
    serviceName VARCHAR(100) NOT NULL,
    description TEXT,
    price DECIMAL(10,2) NOT NULL
);

CREATE TABLE Booking (
    bookingId INT PRIMARY KEY IDENTITY(1,1),
    accountId INT NOT NULL,
    serviceId INT NOT NULL,
    caregiverId INT NOT NULL,
    elderId INT NULL,
    bookingDateTime DATETIME NOT NULL,
    status VARCHAR(20) CHECK (status IN ('pending', 'completed', 'canceled')) NOT NULL,
    FOREIGN KEY (accountId) REFERENCES Account(accountId),
    FOREIGN KEY (serviceId) REFERENCES Service(serviceId),
    FOREIGN KEY (caregiverId) REFERENCES Caregiver(caregiverId),
    FOREIGN KEY (elderId) REFERENCES Elder(elderId)
);

CREATE TABLE Record (
    recordId INT PRIMARY KEY IDENTITY(1,1),
    elderId INT NOT NULL,
    description TEXT NOT NULL,
    bookingId INT NOT NULL,
    status VARCHAR(50) NOT NULL,
    last_updated DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (elderId) REFERENCES Elder(elderId),
    FOREIGN KEY (bookingId) REFERENCES Booking(bookingId)
);

CREATE TABLE Tracking (
    trackingId INT PRIMARY KEY IDENTITY(1,1),
    elderId INT NOT NULL,
    date DATE NOT NULL,
    weight DECIMAL(5,2),
    bloodPressure VARCHAR(20),
    FOREIGN KEY (elderId) REFERENCES Elder(elderId)
);

CREATE TABLE Feedback (
    feedbackId INT PRIMARY KEY IDENTITY(1,1),
    bookingId INT NOT NULL,
    note TEXT,
    rating INT CHECK (rating BETWEEN 1 AND 5),
    FOREIGN KEY (bookingId) REFERENCES Booking(bookingId)
);

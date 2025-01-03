-- V�rifiez si la base de donn�es existe, puis supprimez-la si elle existe
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'HotelManagementSystem')
BEGIN
    DROP DATABASE HotelManagementSystem;
END

-- Cr�ez la base de donn�es
CREATE DATABASE HotelManagementSystem;
GO

-- Utilisez la base de donn�es cr��e
USE HotelManagementSystem;
GO

-- Cr�ez les sch�mas n�cessaires
CREATE SCHEMA Authentication;
GO

CREATE SCHEMA Hotels;
GO

CREATE SCHEMA Bookings;
GO

CREATE SCHEMA HotelService;
GO

CREATE SCHEMA Rooms;
GO

-- Cr�ez la table Admin
CREATE TABLE Authentication.Admin (
    AdminId INT IDENTITY (1,1),
    Username NVARCHAR (30) UNIQUE NOT NULL,
    Password NVARCHAR (30) NOT NULL,
    CONSTRAINT PK_AdminId PRIMARY KEY (AdminId)
);

-- Ins�rez un administrateur par d�faut
INSERT INTO Authentication.Admin (Username, Password) VALUES ('admin110', 'hotels123');

-- Cr�ez la table Hotel
CREATE TABLE Hotels.Hotel (
    HotelId INT IDENTITY (1,1),
    Name NVARCHAR (50) NOT NULL UNIQUE,
    ContactNumber NVARCHAR (15) NOT NULL UNIQUE,
    Email NVARCHAR (50) NOT NULL UNIQUE,
    Website NVARCHAR (50) UNIQUE,
    Description NVARCHAR (500),
    FloorCount INT NOT NULL,
    TotalRooms INT NOT NULL,
    AddressLine NVARCHAR(50) UNIQUE NOT NULL,
    Street NVARCHAR(50) NOT NULL,
    City NVARCHAR(20) NOT NULL,
    Zip NVARCHAR(50) NOT NULL,
    Country NVARCHAR(30) NOT NULL,
    CONSTRAINT PK_HotelId PRIMARY KEY (HotelId)
);

-- Cr�ez la table Departments
CREATE TABLE Hotels.Departments (
    DepartmentId INT IDENTITY(1,1),
    DepartmentName NVARCHAR (50) NOT NULL,
    DepartmentDescription NVARCHAR (50) NOT NULL,
    InitialSalary INT NOT NULL,
    HotelId INT,
    Designation NVARCHAR (50) NOT NULL DEFAULT 'Intern',
    CONSTRAINT PK_DepartmentId PRIMARY KEY (DepartmentId),
    CONSTRAINT FK_HotelId_Dep FOREIGN KEY (HotelId) 
    REFERENCES Hotels.Hotel (HotelId) ON DELETE NO ACTION ON UPDATE NO ACTION
);

-- Cr�ez la table Employees
CREATE TABLE Hotels.Employees (
    EmployeeId INT IDENTITY (1,1),
    EmployeeFirstName NVARCHAR (50) NOT NULL,
    EmployeeLastName NVARCHAR (50) NOT NULL,
    EmployeeDesignation NVARCHAR (50) NOT NULL,
    EmployeeContactNumber NVARCHAR (15) NOT NULL,
    EmployeeEmailAddress NVARCHAR(50) NOT NULL UNIQUE,
    EmployeeJoiningDate DATE NOT NULL, 
    AddressLine NVARCHAR(50) NOT NULL,
    Street NVARCHAR(50) NOT NULL,
    City NVARCHAR(20) NOT NULL,
    Zip NVARCHAR(50) NOT NULL,
    DepartmentId INT,
    HotelId INT NOT NULL,
    CNIC NVARCHAR(20) NOT NULL UNIQUE,
    CONSTRAINT PK_EmployeeId PRIMARY KEY (EmployeeId),
    CONSTRAINT FK_DepartmentId_Employee FOREIGN KEY (DepartmentId)
    REFERENCES Hotels.Departments (DepartmentId) ON DELETE SET NULL ON UPDATE NO ACTION,
    CONSTRAINT FK_HotelId_Employee FOREIGN KEY (HotelId)
    REFERENCES Hotels.Hotel (HotelId) ON DELETE CASCADE ON UPDATE NO ACTION
);

-- Cr�ez la table Login
CREATE TABLE Authentication.Login (
    LoginId INT IDENTITY (1,1),
    Username NVARCHAR (30) NOT NULL UNIQUE,
    Password NVARCHAR (30) NOT NULL,
    EmployeeId INT,
    SecurityQuestion NVARCHAR(100),
    Answer NVARCHAR(50),
    HotelId INT NOT NULL,
    NewUser CHAR(5) CHECK (NewUser IN ('Yes', 'No')) DEFAULT 'Yes',
    CONSTRAINT PK_LoginId PRIMARY KEY (LoginId),
    CONSTRAINT FK_EmployeeId_Login FOREIGN KEY (EmployeeId)
    REFERENCES Hotels.Employees (EmployeeId) ON DELETE CASCADE ON UPDATE NO ACTION,
    CONSTRAINT FK_HotelId_Login FOREIGN KEY (HotelId) 
    REFERENCES Hotels.Hotel (HotelId) ON DELETE NO ACTION ON UPDATE NO ACTION
);

-- Ajoutez le reste des tables et leurs relations ici...

-- V�rifiez le bon fonctionnement de la base
SELECT * FROM Authentication.Admin;
SELECT * FROM Hotels.Hotel;

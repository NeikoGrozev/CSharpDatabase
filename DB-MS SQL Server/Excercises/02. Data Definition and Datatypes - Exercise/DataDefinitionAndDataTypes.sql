--====================================--
--Problem 1.	Create Database--
--====================================--

CREATE DATABASE Minions

USE Minions

--====================================--
--Problem 2.	Create Tables--
--====================================--

CREATE TABLE Minions(
	Id INT PRIMARY KEY,
	[Name] NVARCHAR(30) NOT NULL,
	Age INT
)

CREATE TABLE Towns(
	Id INT PRIMARY KEY,
	[Name] NVARCHAR(30) NOT NULL
)

--====================================--
--Problem 3.	Alter Minions Table--
--====================================--

ALTER TABLE Minions
ADD TownId INT NOT NULL

ALTER TABLE Minions
ADD CONSTRAINT FK_TownId
FOREIGN KEY (FK_TownId) REFERENCES Towns(Id)

--====================================--
--Problem 4.	Insert Records in Both Tables--
--====================================--

INSERT INTO Towns(Id, [Name]) VALUES
	(1, 'Sofia'),
	(2, 'Plovdiv'),
	(3, 'Varna')

INSERT INTO Minions (Id, [Name], Age, TownId) VALUES
	(1, 'Kevin', 22, 1),
	(2, 'Bob', 15, 3),
	(3, 'Steward', NULL, 2)

SELECT [Id], [Name], [Age], [TownId] FROM Minions

--====================================--
--Problem 5.	Truncate Table Minions--
--====================================--

TRUNCATE TABLE Minions

--====================================--
--Problem 6.	Drop All Tables--
--====================================--

DROP TABLE Minions

DROP TABLE Towns

--====================================--
--Problem 7.	Create Table People--
--====================================--

CREATE TABLE People(
	Id SMALLINT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(200) NOT NULL,
	Picture VARBINARY(MAX) CHECK (DATALENGTH(Picture) > 1024 * 1024 * 2),
	Height DECIMAL(3, 2),
	[Weight]  DECIMAL (5, 2),
	Gender CHAR(1) CHECK (Gender = 'm' OR Gender = 'f') NOT NULL,
	Birthdate DATE NOT NULL,
	Biography  NVARCHAR(MAX)
)

INSERT INTO People ([Name], Picture, Height, [Weight], Gender, Birthdate, Biography) VALUES
	('Ivan Ivanov', NULL, 1.80, 78.00, 'm', '1984-11-10', NULL),
	('Petar Petrov', NULL, 1.86, 88.50, 'm', '1990-05-13', NULL),
	('Mariana Dimitrova', NULL, 1.70, 51.00, 'f', '1988-02-07', NULL),
	('Pesho Peshov', NULL, 1.90, 95.00, 'm', '1992-03-20', NULL),
	('Diana Kostadinova', NULL, 1.78, 55.70, 'f', '1989-07-18', NULL)

SELECT * FROM People

--====================================--
--Problem 8.	Create Table Users--
--====================================--

CREATE TABLE Users(
	Id BIGINT UNIQUE IDENTITY,
	Username VARCHAR(30) UNIQUE NOT NULL,
	[Password] VARCHAR(26) NOT NULL,
	ProfilePicture VARBINARY CHECK (DATALENGTH(ProfilePicture) <= 1024 * 900),
	LastLoginTime DATETIME,
	IsDelete BIT
)

ALTER TABLE Users
ADD CONSTRAINT PK_Users PRIMARY KEY(Id)

INSERT INTO Users (Username, [Password], ProfilePicture, LastLoginTime, IsDelete) VALUES
	('Ivancho', '123abc', NULL, GETDATE(), 0),
	('Petar', '123abc', NULL, GETDATE(), 0),
	('Mari', '123abc', NULL, GETDATE(), 0),
	('Pesho', '123abc', NULL, GETDATE(), 0),
	('Didi', '123abc', NULL, GETDATE(), 0)

SELECT * FROM Users

--====================================--
--Problem 9.	Change Primary Key--
--====================================--

ALTER TABLE Users
DROP CONSTRAINT PK_Users

ALTER TABLE Users
ADD CONSTRAINT PK_Users PRIMARY KEY (Id, Username)

--====================================--
--Problem 10.	Add Check Constraint--
--====================================--

ALTER TABLE Users
ADD CONSTRAINT PasswordLength CHECK (LEN(Password) >= 5)

--====================================--
--Problem 11.	Set Default Value of a Field--
--====================================--

ALTER TABLE Users
ADD DEFAULT GETDATE() FOR LastLoginTime

--====================================--
--Problem 12.	Set Unique Field--
--====================================--

ALTER TABLE Users
DROP CONSTRAINT PK_Users

ALTER TABLE Users
ADD CONSTRAINT PK_Users PRIMARY KEY(Id)

ALTER TABLE Users
ADD CONSTRAINT UsernameLength CHECK (LEN(Username) >= 3)

--====================================--
--Problem 13.	Movies Database--
--====================================--

CREATE DATABASE Movies

USE Movies

CREATE TABLE Directors(
	Id INT PRIMARY KEY IDENTITY,
	DirectorName NVARCHAR(30) NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Genres(
	Id INT PRIMARY KEY IDENTITY,
	GenresName NVARCHAR(30) NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Categories(
	Id INT PRIMARY KEY IDENTITY,
	CategoryName NVARCHAR(30) NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Movies(
	Id INT PRIMARY KEY IDENTITY,
	Title NVARCHAR(60) NOT NULL UNIQUE,
	DirectorId INT FOREIGN KEY REFERENCES Directors(Id) NOT NULL,
	CopyrightYear INT NOT NULL,
	[Length] TIME,
	GenreId INT FOREIGN KEY REFERENCES Genres(Id) NOT NULL,
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL,
	Rating DECIMAL(4, 1),
	Notes NVARCHAR(MAX)
)

INSERT INTO Directors (DirectorName, Notes) VALUES
	('Ivan Petrov', ''),
	('Dimitar Kolev', ''),
	('Nadejda Simeonova', '1234'),
	('Kaloian Kaloianov', ''),
	('Viktor Jelev', 'QWERTY')
	
INSERT INTO Genres (GenresName, Notes) VALUES
	('Comedy', ''),
	('Drama', ''),
	('Thriller', ''),
	('Fantasy', ''),
	('Horror', '@')

INSERT INTO Categories (CategoryName, Notes) VALUES
	('A', 'note'),
	('B', 'note'),
	('C', 'note'),
	('D', 'note'),
	('E', 'note')

INSERT INTO Movies (Title, DirectorId, CopyrightYear, [Length], GenreId, CategoryId, Rating, Notes) VALUES
	('Star Wars', 1, 2019, '2:10:00', 4, 2, 9.9, 'note'),
	('Joker', 2, 2019, '1:30:15', 1, 1, 9.0, 'note'),
	('Rambo', 5, 2019, '1:59:50', 3, 2, 9.6, 'note'),
	('Jumanji: The Next Level', 2, 2019, '1:30:11', 5, 2, 8.9, 'note'),
	('Home Alone', 2, 1990, '2:05:00', 4, 2, 9.8, 'note')

SELECT * FROM Movies

--====================================--
--Problem 14.	Car Rental Database--
--====================================--

CREATE DATABASE CarRental

USE CarRental

CREATE TABLE Categories(
	Id INT PRIMARY KEY IDENTITY,
	CategoryName NVARCHAR(50) NOT NULL UNIQUE,
	DailyRate DECIMAL(6, 2) NOT NULL,
	WeeklyRate DECIMAL(6, 2) NOT NULL,
	MonthlyRate DECIMAL(6, 2) NOT NULL,
	WeekendRate DECIMAL(6, 2) NOT NULL
)

CREATE TABLE Cars(
	Id INT PRIMARY KEY IDENTITY,
	PlateNumber NVARCHAR(60) NOT NULL,
	Manufacturer NVARCHAR(60) NOT NULL,
	Model NVARCHAR(60) NOT NULL,
	CarYear INT NOT NULL,
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
	Doors INT NOT NULL,
	Picture VARBINARY(MAX),
	Condition NVARCHAR(40),
	Available BIT NOT NULL
) 

CREATE TABLE Employees(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(30) NOT NULL,
	LastName NVARCHAR(30) NOT NULL,
	Title NVARCHAR(40) NOT NULL,
	Notes NVARCHAR(60)
)

CREATE TABLE Customers(
	Id INT PRIMARY KEY IDENTITY,
	DriverLicenceNumber VARCHAR(30) NOT NULL,
	FullName NVARCHAR(60) NOT NULL,
	[Address] NVARCHAR(100) NOT NULL,
	City NVARCHAR(30) NOT NULL,
	ZIPCode INT NOT NULL,
	Notes NVARCHAR(60)
)

CREATE TABLE RentalOrders(
	Id INT PRIMARY KEY IDENTITY,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id),
	CustomerId INT FOREIGN KEY REFERENCES Customers(Id),
	CarId INT FOREIGN KEY REFERENCES Cars(Id),
	TankLevel INT NOT NULL,
	KilometrageStart INT NOT NULL,
	KilometrageEnd INT NOT NULL,
	TotalKilometrage INT NOT NULL,
	StartDate DATE NOT NULL,
	EndDate DATE NOT NULL,
	TotalDays INT NOT NULL,
	RateApplied DECIMAL(6, 2) NOT NULL,
	TaxRate AS RateApplied * 0.2,
	OrderStatus BIT NOT NULL,
	Notes NVARCHAR(100)
)

INSERT INTO Categories VALUES
	('Economic', 40, 230, 850, 70.50),
	('Sport', 65.50, 350, 1350, 120.70),
	('SUV', 85.60, 500, 1800, 160.30)

INSERT INTO Cars VALUES
	('ABS28', 'Audi', 'A8', 2011, 1, 4, NULL, 'Good', 1),
	('ADSD837', 'Toyota', 'Ayris', 2008, 3, 5, NULL, 'Very good', 0),
	('CSKK4K5', 'VW', 'Touareg', 2012, 2, 5, NULL, 'Poor', 1)

INSERT INTO Employees VALUES
	('Ivan', 'Tonchev', 'Manager', 'note'),
	('Georgi', 'Petkov', 'CEO', 'note'),
	('Petar', 'Kolev', 'CTO', 'note')

INSERT INTO Customers VALUES
	('DOSO9DS7C', 'Pavel Kolev', 'Rezbarska 25', 'Sofia', 1234, NULL),
	('XSXKSS7S8D9S', 'Ivan Mishev', 'Mysala 37', 'Plovdiv', 2035, NULL),
	('XSXSX8888S9', 'Diana Ivanova', 'Kraibrejna 56', 'Burgas', 8001, NULL)

INSERT INTO RentalOrders VALUES
	(2, 3, 3, 40, 18005, 18055, 50, '2019-08-08', '2019-08-10', 3, 200.00, 0, NULL),
	(1, 2, 1, 50, 75524, 75590, 66, '2019-09-06', '2019-09-20', 14, 1500.00, 1, NULL),
	(3, 1, 2, 34, 36005, 36155, 150, '2019-10-08', '2019-10-10', 18, 2200.00, 0, NULL)

--====================================--
--Problem 15.	Hotel Database--
--====================================--

CREATE DATABASE Hotel

USE Hotel

CREATE TABLE Employees(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(30) NOT NULL,
	LastName NVARCHAR(30) NOT NULL,
	Title NVARCHAR(60) NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Customers(
	AccountNumber INT PRIMARY KEY NOT NULL,
	FirstNamE NVARCHAR(30) NOT NULL,
	LastNamE NVARCHAR(30) NOT NULL,
	PhoneNumber VARCHAR(10) NOT NULL,
	EmergencyName NVARCHAR(30) NOT NULL,
	EmergencyNumber VARCHAR(10) NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE RoomStatus(
	RoomStatus NVARCHAR(30) PRIMARY KEY NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE RoomTypes(
	RoomType NVARCHAR(30) PRIMARY KEY NOT NULL,
	Notes NVARCHAR(MAX)
)

CREATE TABLE BedTypes(
	BedType NVARCHAR(30) PRIMARY KEY NOT NULL,
	Notes NVARCHAR(MAX) 
)

CREATE TABLE Rooms(
	RoomNumber INT PRIMARY KEY NOT NULL,
	RoomType NVARCHAR(30) FOREIGN KEY REFERENCES RoomTypes(RoomType),
	BedType NVARCHAR(30) FOREIGN KEY REFERENCES BedTypes(BedType),
	Rate INT NOT NULL,
	RoomStatus NVARCHAR(30) FOREIGN KEY REFERENCES RoomStatus(RoomStatus),
	Notes NVARCHAR(MAX)
)

CREATE TABLE Payments(
	Id INT PRIMARY KEY IDENTITY,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id),
	PaymentDate DATETIME2 NOT NULL,
	AccountNumber INT FOREIGN KEY REFERENCES Customers(AccountNumber),
	FirstDateOccupied DATETIME NOT NULL,
	LastDateOccupied DATETIME NOT NULL,
	TotalDays INT NOT NULL,
	AmountCharged DECIMAL(15, 2) NOT NULL,
	TaxRate DECIMAL(15, 2) NOT NULL,
	TaxAmount AS AmountCharged * TaxRate,
	PaymentTotal AS AmountCharged + AmountCharged * TaxRate,
	Notes NVARCHAR(MAX)
)

CREATE TABLE Occupancies(
	Id INT PRIMARY KEY IDENTITY,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id),
	DateOccupied DATETIME2 NOT NULL,
	AccountNumber INT FOREIGN KEY REFERENCES Customers(AccountNumber),
	RoomNumber INT FOREIGN KEY REFERENCES Rooms(RoomNumber),
	RateApplied DECIMAL(15, 2) NOT NULL,
	PhoneCharge VARCHAR(10) NOT NULL,
	Notes NVARCHAR(MAX)
)

INSERT INTO Employees(FirstName, LastName, Title) VALUES
	('Ivan', 'Ivanov', 'Manager'),
	('Pesho', 'Kolev', 'Receptionist'),
	('Elena', 'Petrov', 'Receptionist')

INSERT INTO Customers(AccountNumber, FirstName, LastName, PhoneNumber, EmergencyName, EmergencyNumber) VALUES
	(1234, 'Georgi', 'Georgiev', 0888889900, 'Ivan', 0888345123),
	(1235, 'Pavel', 'Tonev', 0898456789, 'Boiko', 0899345234),
	(1236, 'Ilian', 'Ivanov', 0898123456, 'Ivailo', 0878890567)

INSERT INTO RoomStatus(RoomStatus) VALUES
	('Occupied'),
	('Non occupied'),
	('Repairs')

INSERT INTO RoomTypes(RoomType) VALUES
	('Single'),
	('Double'),
	('Appartment')

INSERT INTO BedTypes(BedType) VALUES
	('Single'),
	('Double'),
	('Couch')

INSERT INTO Rooms(RoomNumber, RoomType, BedType, Rate, RoomStatus) VALUES
	(101, 'single', 'single', 40.0, 'Non occupied'),
	(605, 'double', 'double', 75.0, 'Non occupied'),
	(707, 'appartment', 'double', 120.0, 'Occupied')

INSERT INTO Payments(EmployeeId, PaymentDate, AccountNumber, FirstDateOccupied, LastDateOccupied, TotalDays, AmountCharged, TaxRate) VALUES
	(3, '2019-12-20', 1235, '2019-12-26', '2019-12-01', 6, 550.0, 0.2),
	(2, '2019-12-01', 1234, '2019-12-03', '2019-11-20', 3, 150.0, 0.2),
	(2, '2019-12-30', 1236, '2020-01-02', '2019-12-04', 4, 870.0, 0.2)

INSERT INTO Occupancies(EmployeeId, DateOccupied, AccountNumber, RoomNumber, RateApplied, PhoneCharge) VALUES
	(3, '2019-12-01', 1235, 101, 40.0, 11.60),
	(2, '2019-11-20', 1234, 605, 75.0, 13.22),
	(2, '2019-12-04', 1236, 707, 120.0, 9.05)

--====================================--
--Problem 16.	Create SoftUni Database--
--====================================--

CREATE DATABASE SoftUni

USE SoftUni

CREATE TABLE Towns(
	Id INT PRIMARY KEY IDENTITY,
	Name NVARCHAR(50) NOT NULL
) 

CREATE TABLE Addresses(
	Id INT PRIMARY KEY IDENTITY,
	AddressText NVARCHAR(200) NOT NULL,
	TownId INT FOREIGN KEY REFERENCES Towns(Id)
)

CREATE TABLE Departments(
	Id INT PRIMARY KEY IDENTITY,
	Name NVARCHAR(50) NOT NULL
)

CREATE TABLE Employees(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(30) NOT NULL,
	MiddleName NVARCHAR(30) NOT NULL,
	LastName NVARCHAR(30) NOT NULL,
	JobTitle NVARCHAR(30) NOT NULL,
	DepartmentId INT FOREIGN KEY REFERENCES Departments(Id),
	HireDate DATETIME2 NOT NULL,
	Salary DECIMAL(15, 2) NOT NULL,
	AddressId INT FOREIGN KEY REFERENCES Addresses(Id)
)

--====================================--
--Problem 17.	Backup Database--
--====================================--

BACKUP DATABASE SoftUni
TO DISK = 'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\Backup\softuni-backup.bak';

--====================================--
--Problem 18.	Basic Insert--
--====================================--

INSERT INTO Towns([Name]) VALUES
	('Sofia'),
	('Plovdiv'),
	('Varna'),
	('Burgas')

INSERT INTO Departments([Name]) VALUES
	('Engineering'),
	('Sales'),
	('Marketing'),
	('Software Development'),
	('Quality Assurance')

INSERT INTO Employees(FirstName, MiddleName, LastName, JobTitle, DepartmentId, HireDate, Salary, AddressId) VALUES
	('Ivan', 'Ivanov', 'Ivanov', '.NET Developer', 4, '2013-02-01', 3500.00, NULL),
	('Petar', 'Petrov', 'Petrov', 'Senior Engineer', 1, '2004-03-02', 4000.00, NULL),
	('Maria', 'Petrova', 'Ivanova', 'Intern', 5, '2016-08-28', 525.25, NULL),
	('Georgi', 'Terziev', 'Ivanov', 'CEO', 2, '2007-12-09', 3000.00, NULL),
	('Peter', 'Pan', 'Pan', 'Intern', 3, '2016-08-28', 599.88, NULL)

--====================================--
--Problem 19.	Basic Select All Fields--
--====================================--

SELECT * FROM Towns

SELECT * FROM Departments

SELECT * FROM Employees

--====================================--
--Problem 20.	Basic Select All Fields and Order Them--
--====================================--

SELECT * FROM Towns ORDER BY [Name]

SELECT * FROM Departments ORDER BY [Name]

SELECT * FROM Employees ORDER BY Salary DESC

--====================================--
--Problem 21.	Basic Select Some Fields--
--====================================--

SELECT [Name] FROM Towns ORDER BY [Name]

SELECT [Name] FROM Departments ORDER BY [Name]

SELECT FirstName, LastName, JobTitle, Salary FROM Employees ORDER BY Salary DESC

--====================================--
--Problem 22.	Increase Employees Salary--
--====================================--

UPDATE Employees
SET Salary += Salary * 0.1

SELECT Salary FROM Employees

--====================================--
--Problem 23.	Decrease Tax Rate--
--====================================--

USE Hotel

UPDATE Payments
SET TaxRate -= TaxRate * 0.03

SELECT TaxRate FROM Payments

--====================================--
--Problem 24.	Delete All Records--
--====================================--

TRUNCATE TABLE Occupancies 
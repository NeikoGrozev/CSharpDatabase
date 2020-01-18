--=====================================================--
--Problem 1. Database Design--
--=====================================================--

CREATE DATABASE Airport

USE Airport

CREATE TABLE Planes(
	Id INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(30) NOT NULL,
	Seats INT NOT NULL,
	[Range] INT NOT NULL
)

CREATE TABLE Flights(
	Id INT PRIMARY KEY IDENTITY,
	DepartureTime DATETIME2,
	ArrivalTime DATETIME2,
	Origin VARCHAR(50) NOT NULL,
	Destination VARCHAR(50) NOT NULL,
	PlaneId INT FOREIGN KEY REFERENCES Planes(Id) NOT NULL
)

CREATE TABLE Passengers(
	Id INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(30) NOT NULL,
	LastName VARCHAR(30) NOT NULL,
	Age INT NOT NULL,
	Address VARCHAR(30) NOT NULL,
	PassportId CHAR(11) NOT NULL
)

CREATE TABLE LuggageTypes(
	Id INT PRIMARY KEY IDENTITY,
	[Type] VARCHAR(30) NOT NULL
)

CREATE TABLE Luggages(
	Id INT PRIMARY KEY IDENTITY,
	LuggageTypeId INT FOREIGN KEY REFERENCES LuggageTypes(Id) NOT NULL,
	PassengerId INT FOREIGN KEY REFERENCES Passengers(Id) NOT NULL
)

CREATE TABLE Tickets(
	Id INT PRIMARY KEY IDENTITY,
	PassengerId INT FOREIGN KEY REFERENCES Passengers(Id) NOT NULL,
	FlightId INT FOREIGN KEY REFERENCES Flights(Id) NOT NULL,
	LuggageId INT FOREIGN KEY REFERENCES Luggages(Id) NOT NULL,
	Price DECIMAL(15, 2) NOT NULL
)

--=====================================================--
--Problem 2. Insert--
--=====================================================--

INSERT INTO Planes([Name], Seats, [Range]) VALUES
('Airbus 336', 112,	5132),
('Airbus 330', 432,	5325),
('Boeing 369', 231,	2355),
('Stelt 297', 254,	2143),
('Boeing 338', 165,	5111),
('Airbus 558', 387,	1342),
('Boeing 128', 345,	5541)

INSERT INTO LuggageTypes([Type]) VALUES
('Crossbody Bag'),
('School Backpack'),
('Shoulder Bag')


--=====================================================--
--Problem 3. Update--
--=====================================================--

UPDATE Tickets
SET Price *= 1.13
WHERE FlightId IN (SELECT Id FROM Flights
					WHERE Destination = 'Carlsbad'
)

--=====================================================--
--Problem 4. Delete--
--=====================================================--

DELETE FROM Tickets
WHERE FlightId = (SELECT Id FROM Flights
					WHERE Destination = 'Ayn Halagim'
)

DELETE FROM Flights
WHERE Destination = 'Ayn Halagim'

--=====================================================--
--Problem 5. The "Tr" Planes--
--=====================================================--

SELECT *
FROM Planes
WHERE [Name] LIKE '%tr%'
ORDER BY Id, [Name], Seats, [Range]

--=====================================================--
--Problem 6. Flight Profits--
--=====================================================--

SELECT FlightId, SUM(t.Price) AS Price
FROM Tickets AS t
GROUP BY t.FlightId
ORDER BY Price DESC, FlightId

--=====================================================--
--Problem 7. Passenger Trips--
--=====================================================--

SELECT CONCAT(p.FirstName, ' ', p.LastName) AS FullName, f.Origin, f.Destination
FROM Passengers AS p
JOIN Tickets AS t ON p.Id = t.PassengerId
JOIN Flights AS f ON t.FlightId = f.Id
ORDER BY FullName, f.Origin, f.Destination

--=====================================================--
--Problem 8. Non Adventures People--
--=====================================================--

SELECT p.FirstName, p.LastName, p.Age
FROM Passengers AS p
LEFT JOIN Tickets AS t ON p.Id = t.PassengerId
WHERE t.PassengerId IS NULL
ORDER BY p.Age DESC, p.FirstName, p.LastName

--=====================================================--
--Problem 9. Full Info--
--=====================================================--

SELECT CONCAT(p.FirstName, ' ', p.LastName) AS FullName,
			 pl.[Name] AS [Plane Name], 
			 CONCAT(f.Origin, ' - ', f.Destination) AS Trip,
			 lt.[Type] AS [Luggage Type]
FROM Passengers AS p
JOIN Tickets AS t ON t.PassengerId = p.Id
JOIN Flights AS f ON f.Id = t.FlightId
JOIN Planes AS pl ON pl.Id = f.PlaneId
JOIN Luggages AS l ON l.Id = t.LuggageId
JOIN LuggageTypes AS lt ON lt.Id = l.LuggageTypeId
ORDER BY FullName, [Plane Name], f.Origin, f.Destination, [Luggage Type]

--=====================================================--
--Problem 10. PSP--
--=====================================================--

SELECT p.[Name], p.Seats, COUNT(t.Id) AS [Passengers Count]
FROM Planes AS p
LEFT JOIN Flights AS f ON f.PlaneId = p.Id
LEFT JOIN Tickets AS t ON t.FlightId = f.Id
GROUP BY p.[Name], p.Seats
ORDER BY [Passengers Count] DESC, p.[Name], p.Seats

--=====================================================--
--Problem 11. Vacation--
--=====================================================--

GO

CREATE FUNCTION udf_CalculateTickets(@origin VARCHAR(30), @destination VARCHAR(30), @peopleCount INT) 
RETURNS VARCHAR(100)
AS
BEGIN 
	DECLARE @trip INT = (SELECT f.Id FROM Flights AS f
						 WHERE f.Destination = @destination AND f.Origin = @origin)
	DECLARE @totalSum DECIMAL(15, 2) = (SELECT t.Price FROM Flights AS f
										JOIN Tickets AS t ON t.FlightId = f.Id
										WHERE f.Destination = @destination AND f.Origin = @origin)

	IF(@peopleCount <= 0)
	BEGIN 
		RETURN 'Invalid people count!'
	END
	
	IF(@trip IS NULL)
	BEGIN
		RETURN 'Invalid flight!'
	END
	
	SET @totalSum *= @peopleCount
	RETURN 'Total price ' + CAST(@totalSum AS VARCHAR(20))
END

GO

SELECT dbo.udf_CalculateTickets('Kolyshley','Rancabolang', 33)
SELECT dbo.udf_CalculateTickets('Kolyshley','Rancabolang', -1)
SELECT dbo.udf_CalculateTickets('Kolyshley','Rancabolang', -1)
SELECT dbo.udf_CalculateTickets('Invalid','Rancabolang', 33)

--=====================================================--
--Problem 12. Wrong Data--
--=====================================================--

GO

CREATE PROC usp_CancelFlights
AS
UPDATE Flights
SET DepartureTime = NULL, ArrivalTime = NULL
WHERE ArrivalTime > DepartureTime

EXEC dbo.usp_CancelFlights
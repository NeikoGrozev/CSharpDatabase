CREATE DATABASE TripService

USE TripService

GO

--=====================================================--
--Problem 1. Database design--
--=====================================================--

CREATE TABLE Cities
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(20) NOT NULL,
	CountryCode CHAR(2) NOT NULL
)

CREATE TABLE Hotels
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(30) NOT NULL,
	CityId INT FOREIGN KEY REFERENCES Cities(Id) NOT NULL,
	EmployeeCount INT NOT NULL,
	BaseRate DECIMAL(15,2)
)

CREATE TABLE Rooms
(
	Id INT PRIMARY KEY IDENTITY,
	Price DECIMAL(15,2) NOT NULL,
	[Type] NVARCHAR(20) NOT NULL,
	Beds INT NOT NULL,
	HotelId INT FOREIGN KEY REFERENCES Hotels(Id) NOT NULL
)

CREATE TABLE Trips
(
	Id INT PRIMARY KEY IDENTITY,
	RoomId INT FOREIGN KEY REFERENCES Rooms(Id) NOT NULL,
	BookDate DATE NOT NULL,
	ArrivalDate DATE NOT NULL,
	ReturnDate DATE NOT NULL,
	CancelDate DATE,
	CHECK(BookDate < ArrivalDate),
	CHECK(ArrivalDate < ReturnDate)
)

CREATE TABLE Accounts
(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(50) NOT NULL,
	MiddleName NVARCHAR(20),
	LastName NVARCHAR(50) NOT NULL,
	CityId INT FOREIGN KEY REFERENCES Cities(Id) NOT NULL,
	BirthDate DATE NOT NULL,
	Email NVARCHAR(100) UNIQUE NOT NULL
)

CREATE TABLE AccountsTrips
(
	AccountId INT FOREIGN KEY REFERENCES Accounts(Id) NOT NULL,
	TripId INT FOREIGN KEY REFERENCES Trips(Id) NOT NULL,
	Luggage INT CHECK(Luggage >= 0) NOT NULL

	CONSTRAINT PK_AccountsTrips PRIMARY KEY (AccountId, TripId)
)

--=====================================================--
--Problem 2. Insert--
--=====================================================--

INSERT INTO Accounts(FirstName, MiddleName, LastName, CityId, BirthDate, Email) VALUES
('John', 'Smith', 'Smith',	34,	'1975-07-21', 'j_smith@gmail.com'),
('Gosho', NULL,	'Petrov',	11,	'1978-05-16', 'g_petrov@gmail.com'),
('Ivan', 'Petrovich', 'Pavlov',	59,	'1849-09-26', 'i_pavlov@softuni.bg'),
('Friedrich', 'Wilhelm', 'Nietzsche', 2, '1844-10-15', 'f_nietzsche@softuni.bg')

INSERT INTO Trips(RoomId, BookDate, ArrivalDate, ReturnDate, CancelDate) VALUES
(101, '2015-04-12', '2015-04-14', '2015-04-20',	'2015-02-02'),
(102, '2015-07-07', '2015-07-15', '2015-07-22',	'2015-04-29'),
(103, '2013-07-17', '2013-07-23', '2013-07-24',	NULL),
(104, '2012-03-17', '2012-03-31', '2012-04-01',	'2012-01-10'),
(109, '2017-08-07', '2017-08-28', '2017-08-29',	NULL)

--=====================================================--
--Problem 3. Update--
--=====================================================--

UPDATE Rooms
SET Price *= 1.14
WHERE HotelId IN (5, 7, 9)

--=====================================================--
--Problem 4. Delete--
--=====================================================--

DELETE FROM AccountsTrips
WHERE AccountId = 47

DELETE FROM Accounts
WHERE Id = 47

--=====================================================--
--Problem 5. EEE-Mails--
--=====================================================--

	SELECT 
		a.FirstName,
		a.LastName,
		FORMAT(a.BirthDate, 'MM-dd-yyyy'),
		c.[Name] AS Hometown,
		a.Email
	 FROM Accounts AS a
	 JOIN Cities AS c ON a.CityId = c.Id
	WHERE Email LIKE 'e%'
 ORDER BY Hometown

--=====================================================--
--Problem 6. City Statistics--
--=====================================================--

  SELECT 
		c.[Name] AS City,
		COUNT(h.Id) AS Hotels
    FROM Cities AS c
    JOIN Hotels AS h ON c.Id = h.CityId
GROUP BY c.[Name]
ORDER BY Hotels DESC, City

--=====================================================--
--Problem 7. Longest and Shortest Trips--
--=====================================================--

  SELECT 
		a.Id AS AccountId,
		CONCAT(a.FirstName, ' ', a.LastName) AS FullName,
		MAX(DATEDIFF(DAY,t.ArrivalDate, t.ReturnDate)) AS LongestTrip,
		MIN(DATEDIFF(DAY,t.ArrivalDate, t.ReturnDate)) AS ShortestTrip
	FROM Accounts AS a
	JOIN AccountsTrips AS at ON a.Id = at.AccountId
	JOIN Trips AS t ON at.TripId = t.Id
   WHERE a.MiddleName IS NULL AND t.CancelDate IS NULL
GROUP BY a.FirstName, a.LastName, a.Id
ORDER BY LongestTrip DESC, ShortestTrip ASC

--=====================================================--
--Problem 8. Metropolis--
--=====================================================--

  SELECT TOP(10)
		c.Id,
		c.[Name] AS City,
		c.CountryCode AS Country,
		COUNT(a.CityId) AS Accounts
    FROM Accounts AS a
    JOIN Cities AS c ON a.CityId = c.Id
GROUP BY c.[Name], c.Id, c.CountryCode
ORDER BY Accounts DESC

--=====================================================--
--Problem 9. Romantic Getaways--
--=====================================================--

  SELECT 
		a.Id,
		a.Email,
		c.[Name] AS City,
		COUNT(a.Id) AS Trips
	FROM Accounts AS a
	JOIN AccountsTrips AS at ON a.Id = at.AccountId
	JOIN Trips AS t ON at.TripId = t.Id
	JOIN Rooms AS r ON t.RoomId = r.Id
	JOIN Hotels AS h ON r.HotelId = h.Id
	JOIN Cities AS c ON a.CityId = c.Id
   WHERE a.CityId = h.CityId
GROUP BY a.Id, a.Email, c.Name
ORDER BY Trips DESC, a.Id

--=====================================================--
--Problem 10. GDPR Violation--
--=====================================================--

  SELECT 
		t.Id,
		CONCAT(a.FirstName, ' ', ISNULL(a.MiddleName + ' ', ''), a.LastName) AS [Full Name],
		c.[Name] AS [From],
		ci.[Name] AS [To],
		CASE
			WHEN t.CancelDate IS NULL THEN CONCAT(DATEDIFF(DAY, t.ArrivalDate, t.ReturnDate), ' days')
		ELSE
			'Canceled'
		END AS Duration
    FROM Accounts AS a
	JOIN Cities AS c ON a.CityId = c.Id
	JOIN AccountsTrips AS at ON a.Id = at.AccountId
	JOIN Trips AS t ON at.TripId = t.Id
	JOIN Rooms AS r ON t.RoomId = r.Id
	JOIN Hotels AS h ON r.HotelId = h.Id
	JOIN Cities AS ci ON h.CityId = ci.Id
ORDER BY [Full Name], t.Id

--=====================================================--
--Problem 11. Available Room--
--=====================================================--

GO

CREATE FUNCTION udf_GetAvailableRoom (@hotelId INT , @date DATE, @people INT)
RETURNS VARCHAR(200)
AS
BEGIN
	DECLARE @result VARCHAR(200)
	DECLARE @targetHotelId INT = (SELECT Id FROM Hotels WHERE @hotelId = Id)
	DECLARE @targetRoomId INT = (SELECT TOP(1)
									r.Id
								   FROM Hotels AS h
								   JOIN Rooms AS r ON h.Id = r.HotelId
								   JOIN Trips AS t ON r.Id = t.RoomId
								  WHERE h.Id = @hotelId 
										AND (@date NOT BETWEEN t.ArrivalDate AND t.ReturnDate)
										AND t.CancelDate IS NULL
										AND r.Beds >= @people
										AND YEAR(@date) = YEAR(t.ArrivalDate)
							   ORDER BY r.Price DESC)

	IF(@targetRoomId IS NULL)
	SET @result = 'No rooms available'
	ELSE
	BEGIN
		DECLARE @roomType NVARCHAR(20) = (SELECT [Type] FROM Rooms WHERE Id = @targetRoomId)
		DECLARE @roomBeds INT = (SELECT Beds FROM Rooms WHERE Id = @targetRoomId)

		DECLARE @hotelBaseRate DECIMAL(15,2) = (SELECT BaseRate From Hotels WHERE Id = @hotelId)
		DECLARE @roomPrice DECIMAL(15,2) = (SELECT Price FROM Rooms WHERE Id = @targetRoomId)
		DECLARE @roomTotalPrice DECIMAL(15,2) = (@hotelBaseRate + @roomPrice) * @people

		SET @result = 'Room ' +  CAST(@targetRoomId AS VARCHAR(3)) + ': ' +
				@roomType + ' (' + CAST(@roomBeds AS VARCHAR(2)) + ' beds) - $' +
				CAST(@roomTotalPrice AS VARCHAR(6))
	END
	RETURN @result
END

GO

SELECT dbo.udf_GetAvailableRoom(112, '2011-12-17', 2)
SELECT dbo.udf_GetAvailableRoom(94, '2015-07-26', 3)

GO

--=====================================================--
--Problem 12. Switch Room--
--=====================================================--

CREATE PROCEDURE usp_SwitchRoom(@tripId INT, @targetRoomId INT)
AS
BEGIN
	DECLARE @targetHotelWithtripId NVARCHAR(30) = (SELECT h.[Name]
													 FROM Trips AS t
													 JOIN Rooms AS r ON t.RoomId = r.Id
													 JOIN Hotels AS h ON r.HotelId = h.Id
													WHERE t.Id = @tripId)

	DECLARE @targetHotelWithRoomId NVARCHAR(30) = (SELECT h.[Name]
													 FROM Rooms AS r
													 JOIN Hotels AS h ON r.HotelId = h.Id
													WHERE r.Id = @targetRoomId)


	IF(@targetHotelWithtripId != @targetHotelWithRoomId)
		THROW 50001, 'Target room is in another hotel!', 1

	DECLARE @people INT = (SELECT COUNT(*)
							 FROM Trips AS t
							 JOIN AccountsTrips AS at ON t.Id = at.TripId
							WHERE t.Id = @tripId)

	DECLARE @bedsInRoom INT = (SELECT Beds
								 FROM Rooms
								WHERE Id = @targetRoomId)

	IF(@people > @bedsInRoom)
	THROW 50002, 'Not enough beds in target room!', 1

	UPDATE Trips
	SET RoomId = @targetRoomId
	WHERE Id = @tripId
END
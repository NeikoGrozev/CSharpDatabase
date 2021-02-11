CREATE DATABASE WMS

USE WMS

GO

--=====================================================--
--Problem 1. Database design--
--=====================================================--

CREATE TABLE Clients
(
	ClientId INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	Phone CHAR(12) CHECK(LEN(Phone) = 12) NOT NULL
)

CREATE TABLE Mechanics
(
	MechanicId INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	[Address] VARCHAR(255) NOT NULL
)

CREATE TABLE Models
(
	ModelId INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) UNIQUE NOT NULL
)

CREATE TABLE Jobs
(
	JobId INT PRIMARY KEY IDENTITY,
	ModelId INT FOREIGN KEY REFERENCES Models(ModelId) NOT NULL,
	[Status] VARCHAR(11) DEFAULT('Pending') CHECK([Status] IN ('Pending', 'In Progress', 'Finished')),
	ClientId INT FOREIGN KEY REFERENCES Clients(ClientId) NOT NULL,
	MechanicId INT FOREIGN KEY REFERENCES Mechanics(MechanicId),
	IssueDate DATE NOT NULL,
	FinishDate DATE
)

CREATE TABLE Orders
(
	OrderId INT PRIMARY KEY IDENTITY,
	JobId INT FOREIGN KEY REFERENCES Jobs(JobId) NOT NULL,
	IssueDate DATE,
	Delivered BIT DEFAULT(0)
)

CREATE TABLE Vendors
(
	VendorId INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) UNIQUE NOT NULL
)

CREATE TABLE Parts
(
	PartId INT PRIMARY KEY IDENTITY,
	SerialNumber VARCHAR(50) UNIQUE NOT NULL,
	[Description] VARCHAR(255),
	Price DECIMAL(6,2) CHECK(Price > 0 AND Price < 10000) NOT NULL,
	VendorId INT FOREIGN KEY REFERENCES Vendors(VendorId) NOT NULL,
	StockQty INT DEFAULT(0) CHECK(StockQty >= 0 ) 
)

CREATE TABLE OrderParts
(
	OrderId INT FOREIGN KEY REFERENCES Orders(OrderId) NOT NULL,
	PartId INT FOREIGN KEY REFERENCES Parts(PartId) NOT NULL,
	Quantity INT DEFAULT(1) CHECK(Quantity > 0),
	CONSTRAINT PK_OrderParts PRIMARY KEY (OrderId, PartId)
)

CREATE TABLE PartsNeeded
(
	JobId INT FOREIGN KEY REFERENCES Jobs(JobId) NOT NULL,
	PartId INT FOREIGN KEY REFERENCES Parts(PartId) NOT NULL,
	Quantity INT DEFAULT(1) CHECK(Quantity > 0),
	CONSTRAINT PK_PartsNeeded PRIMARY KEY (JobId, PartId)
)

--=====================================================--
--Problem 2. Insert--
--=====================================================--

INSERT INTO Clients(FirstName, LastName, Phone) VALUES
('Teri', 'Ennaco', '570-889-5187'),
('Merlyn', 'Lawler', '201-588-7810'),
('Georgene', 'Montezuma', '925-615-5185'),
('Jettie', 'Mconnell', '908-802-3564'),
('Lemuel', 'Latzke', '631-748-6479'),
('Melodie', 'Knipp', '805-690-1682'),
('Candida', 'Corbley', '908-275-8357')

INSERT INTO Parts(SerialNumber, Description, Price, VendorId) VALUES
('WP8182119', 'Door Boot Seal', 117.86, 2),
('W10780048', 'Suspension Rod',	42.81, 1),
('W10841140', 'Silicone Adhesive', 6.77, 4),
('WPY055980', 'High Temperature Adhesive', 13.94, 3)

--=====================================================--
--Problem 3. Update--
--=====================================================--

UPDATE Jobs
SET MechanicId = 3,[Status] ='In Progress'
WHERE [Status] = 'Pending'

--=====================================================--
--Problem 4. Delete--
--=====================================================--

DELETE FROM OrderParts
WHERE OrderId = 19

DELETE FROM Orders
WHERE OrderId = 19

--=====================================================--
--Problem 5. Mechanic Assignments--
--=====================================================--

SELECT 
	CONCAT(m.FirstName, ' ', m.LastName) AS Mechanic,
	j.[Status],
	j.IssueDate
  FROM Mechanics AS m
  JOIN Jobs AS j ON m.MechanicId = j.MechanicId
ORDER BY m.MechanicId, j.IssueDate, j.JobId

--=====================================================--
--Problem 6. Current Clients--
--=====================================================--

SELECT 
	CONCAT(c.FirstName, ' ', c.LastName) AS Client,
	DATEDIFF(DAY, j.IssueDate, '2017-04-24'),
	j.[Status]
  FROM Clients AS c
  JOIN Jobs AS j ON c.ClientId = j.ClientId
 WHERE j.[Status] <> 'Finished'

--=====================================================--
--Problem 7. Mechanic Performance--
--=====================================================--

  SELECT 	
		CONCAT(m.FirstName, ' ', m.LastName) AS Mechanic,
		AVG(DATEDIFF(DAY, j.IssueDate, j.FinishDate)) AS [Average Days]
    FROM Mechanics AS m
    JOIN Jobs AS j ON m.MechanicId = j.MechanicId
GROUP BY m.FirstName, m.LastName, m.MechanicId
ORDER BY m.MechanicId

--=====================================================--
--Problem 8. Available Mechanics--
--=====================================================--

   SELECT CONCAT(m.FirstName, ' ', m.LastName) AS Available
     FROM Mechanics AS m
LEFT JOIN Jobs AS j ON m.MechanicId = j.MechanicId
    WHERE j.JobId IS NULL OR J.[Status] = 'Finished'
 GROUP BY m.FirstName, m.LastName, m.MechanicId
 ORDER BY m.MechanicId

--=====================================================--
--Problem 9. Past Expenses--
--=====================================================--

   SELECT 
		j.JobId,
		ISNULL(SUM(op.Quantity * p.Price), 0) AS Total
     FROM Jobs AS j
LEFT JOIN Orders AS o ON j.JobId = o.JobId
LEFT JOIN OrderParts AS op ON o.OrderId = op.OrderId
LEFT JOIN Parts AS p ON op.PartId = p.PartId
    WHERE j.[Status] = 'Finished'
 GROUP BY j.JobId
 ORDER BY Total DESC, j.JobId

--=====================================================--
--Problem 10. Missing Parts--
--=====================================================--

   SELECT 
		p.PartId,
		p.[Description],
		pn.Quantity AS [Required],
		p.StockQty AS [In Stock],
		IIF(o.Delivered = 0, op.Quantity, 0) AS Ordered
     FROM Parts AS p
LEFT JOIN OrderParts AS op ON p.PartId = op.PartId
LEFT JOIN Orders AS o ON op.OrderId = o.OrderId
LEFT JOIN PartsNeeded AS pn ON p.PartId = pn.PartId
LEFT JOIN Jobs AS j ON pn.JobId = j.JobId
    WHERE j.[Status] NOT LIKE 'Finished' 
			AND (p.StockQty + IIF(o.Delivered = 0, op.Quantity, 0)) < pn.Quantity
 ORDER BY p.PartId

--=====================================================--
--Problem 11. Place Order--
--=====================================================--

GO

CREATE PROCEDURE usp_PlaceOrder (@jobId INT, @serialNumber VARCHAR(50), @quantity INT)
AS
BEGIN
	IF (@quantity <= 0)
		THROW 50012, 'Part quantity must be more than zero!', 1;
	ELSE IF ((SELECT Count(JobId) FROM Jobs WHERE JobId = @jobId) = 0)
		THROW 50013, 'Job not found!' , 1;
	ELSE IF ((SELECT [Status] FROM Jobs WHERE JobId = @jobId) = 'Finished')
		THROW 50011, 'This job is not active!', 1;
	ELSE IF ((SELECT Count(SerialNumber) FROM Parts WHERE SerialNumber = @serialNumber) = 0)
		THROW 50014, 'Part not found!', 1;

	DECLARE @partId INT = (SELECT PartId 
							 FROM Parts 
							WHERE SerialNumber = @serialNumber)

	DECLARE @orderId INT = (SELECT o.OrderId
							  FROM Orders AS o
							  JOIN Jobs AS j ON o.JobId = j.JobId
							  JOIN OrderParts op ON o.OrderId = op.OrderId
							 WHERE o.IssueDate IS NULL AND j.JobId = @jobId AND op.PartId = @partId)

	IF(@orderId IS NULL)
	BEGIN
		INSERT INTO Orders(JobId, IssueDate) VALUES
		(@jobId, NULL)

		SELECT @orderId = o.OrderId 
		  FROM Orders AS o
		  JOIN Jobs AS j ON o.JobId = j.JobId
		 WHERE o.IssueDate IS NULL AND j.JobId = @jobId

		INSERT INTO OrderParts(OrderId, PartId, Quantity) VALUES
		(@orderId, @partId, @quantity)
	END
	ELSE
	BEGIN
		UPDATE OrderParts
		SET Quantity += @quantity
		WHERE OrderId = @orderId AND PartId = @partId
	END
END

--=====================================================--
--Problem 12. Cost Of Order--
--=====================================================--

GO

CREATE FUNCTION udf_GetCost(@jobId INT)
RETURNS DECIMAL(15,2)
AS
BEGIN
	DECLARE @result DECIMAL(15,2)

	SET @result = (SELECT SUM(p.Price)
					 FROM Jobs AS j
					 JOIN Orders AS o ON j.JobId = o.JobId
					 JOIN OrderParts AS op ON o.OrderId = op.OrderId
					 JOIN Parts AS p ON op.PartId = p.PartId
					 WHERE j.JobId = @jobId)

	IF(@result IS NULL)
	SET @result = 0

	RETURN @result
END
CREATE DATABASE Bakery

USE Bakery

--=====================================================--
--Problem 1. Database Design--
--=====================================================--

GO

CREATE TABLE Countries
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL UNIQUE
)

CREATE TABLE Customers
(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(25) NOT NULL,
	LastName NVARCHAR(25) NOT NULL,
	Gender CHAR(1) NOT NULL CHECK(LEN(Gender) = 1 AND (Gender = 'M' OR Gender = 'F')),
	Age INT NOT NULL,
	PhoneNumber CHAR(10) NOT NULL CHECK(LEN(PhoneNumber) = 10),
	CountryId INT FOREIGN KEY REFERENCES Countries(Id) NOT NULL
)

CREATE TABLE Products
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(25) NOT NULL UNIQUE,
	[Description] NVARCHAR(250) NOT NULL,
	Recipe NVARCHAR(MAX) NOT NULL,
	Price DECIMAL(15,2) NOT NULL CHECK(Price > 0)
)

CREATE TABLE Feedbacks
(
	Id INT PRIMARY KEY IDENTITY,
	[Description] NVARCHAR(255),
	Rate DECIMAL(15,2) NOT NULL CHECK(Rate BETWEEN 0 AND 10),
	ProductId INT FOREIGN KEY REFERENCES Products(Id) NOT NULL,
	CustomerId INT FOREIGN KEY REFERENCES Customers(Id) NOT NULL
)

CREATE TABLE Distributors
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(25) NOT NULL UNIQUE,
	AddressText NVARCHAR(30) NOT NULL,
	Summary NVARCHAR(200) NOT NULL,
	CountryId INT FOREIGN KEY REFERENCES Countries(Id) NOT NULL
)

CREATE TABLE Ingredients
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(30) NOT NULL,
	[Description] NVARCHAR(200) NOT NULL,
	OriginCountryId INT FOREIGN KEY REFERENCES Countries(Id) NOT NULL,
	DistributorId INT FOREIGN KEY REFERENCES Distributors(Id) NOT NULL
)

CREATE TABLE ProductsIngredients
(
	ProductId INT FOREIGN KEY REFERENCES Products(Id) NOT NULL,
	IngredientId INT FOREIGN KEY REFERENCES Ingredients(Id) NOT NULL
	CONSTRAINT PK_ProductsIngredients PRIMARY KEY(ProductId, IngredientId)
)

GO

--=====================================================--
--Problem 2. Insert--
--=====================================================--

INSERT INTO Distributors([Name], CountryId,	AddressText, Summary) VALUES
('Deloitte & Touche', 2, '6 Arch St #9757', 'Customizable neutral traveling'),
('Congress Title', 13, '58 Hancock St', 'Customer loyalty'),
('Kitchen People', 1, '3 E 31st St #77', 'Triple-buffered stable delivery'),
('General Color Co Inc', 21, '6185 Bohn St #72', 'Focus group'),
('Beck Corporation', 23, '21 E 64th Ave', 'Quality-focused 4th generation hardware')

INSERT INTO Customers(FirstName, LastName, Age, Gender, PhoneNumber, CountryId) VALUES
('Francoise', 'Rautenstrauch', 15, 'M', '0195698399', 5),
('Kendra', 'Loud', 22, 'F', '0063631526', 11),
('Lourdes', 'Bauswell', 50, 'M', '0139037043', 8),
('Hannah', 'Edmison', 18, 'F', '0043343686', 1),
('Tom', 'Loeza', 31, 'M', '0144876096',	23),
('Queenie', 'Kramarczyk', 30, 'F', '0064215793', 29),
('Hiu', 'Portaro', 25, 'M', '0068277755', 16),
('Josefa', 'Opitz',	43, 'F', '0197887645', 17)

--=====================================================--
--Problem 3. Update--
--=====================================================--

UPDATE Ingredients 
SET DistributorId = 35
WHERE [Name] IN ('Bay Leaf', 'Paprika', 'Poppy')

UPDATE Ingredients
SET OriginCountryId = 14
WHERE OriginCountryId = 8

--=====================================================--
--Problem 4. Delete--
--=====================================================--

DELETE FROM Feedbacks
WHERE CustomerId = 14 OR ProductId = 5

--=====================================================--
--Problem 5. Products by Price--
--=====================================================--

  SELECT [Name], Price, [Description]
    FROM Products
ORDER BY Price DESC, [Name]

--=====================================================--
--Problem 6. Negative Feedback--
--=====================================================--

  SELECT f.ProductId, f.Rate, f.[Description], f.CustomerId, c.Age, c.Gender
    FROM Feedbacks AS f
    JOIN Customers AS c ON f.CustomerId = c.Id
   WHERE f.Rate < 5.0
ORDER BY f.ProductId DESC, f.Rate

--=====================================================--
--Problem 7. Customers without Feedback--
--=====================================================--

   SELECT 
		CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
		c.PhoneNumber,
		c.Gender
     FROM Customers AS c
LEFT JOIN Feedbacks AS f ON c.Id = f.CustomerId
    WHERE f.CustomerId IS NULL
 ORDER BY c.Id

--=====================================================--
--Problem 8. Customers by Criteria--
--=====================================================--

  SELECT c.FirstName, c.Age, c.PhoneNumber
    FROM Customers AS c
    JOIN Countries As co ON c.CountryId = co.Id
   WHERE (c.Age >= 21 AND c.FirstName LIKE '%an%')
		  OR (c.PhoneNumber LIKE '%38' AND co.[Name] <> 'Greece')
ORDER BY c.FirstName, c.Age DESC

--=====================================================--
--Problem 9. Middle Range Distributors--
--=====================================================--

  SELECT 
		d.[Name] AS DistributorName,
		i.[Name] AS IngredientName,
		p.[Name] AS ProductName,
		AVG(f.Rate) AS AverageRate
    FROM Distributors AS d
    JOIN Ingredients AS i ON d.Id = i.DistributorId
    JOIN ProductsIngredients AS pin ON i.Id = pin.IngredientId
    JOIN Products AS p ON pin.ProductId = p.Id
    JOIN Feedbacks AS f ON p.Id = f.ProductId
GROUP BY d.[Name], i.[Name], p.[Name]
  HAVING AVG(f.Rate) BETWEEN 5.0 AND 8.0
ORDER BY d.[Name], i.[Name], p.[Name]

--=====================================================--
--Problem 10. Country Representative--
--=====================================================--

  SELECT k.CountryName, k.DisributorName
    FROM (SELECT
				c.[Name] AS CountryName,
				d.[Name] AS DisributorName,
				DENSE_RANK() OVER (PARTITION BY c.[Name] ORDER BY COUNT(d.Id) DESC) AS [Rank]
            FROM Countries AS c
            JOIN Distributors AS d ON c.Id = d.CountryId
            JOIN Ingredients AS i ON d.Id = i.DistributorId
	    GROUP BY c.[Name], d.[Name]) AS k
   WHERE k.[Rank] = 1
ORDER BY k.CountryName, k.DisributorName

--=====================================================--
--Problem 11. Customers with Countries--
--=====================================================--

GO

CREATE VIEW v_UserWithCountries 
AS
SELECT 
	CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName ,
	c.Age,
	c.Gender,
	co.[Name] AS CountryName
  FROM Customers AS c
  JOIN Countries AS co ON c.CountryId = co.Id

--=====================================================--
--Problem 12. Delete Products--
--=====================================================--

GO

CREATE TRIGGER tr_DeleteProduct ON Products INSTEAD OF DELETE
AS
BEGIN
	DECLARE @productId INT = (SELECT Id From deleted)

	DELETE FROM Feedbacks
	WHERE ProductId = @productId

	DELETE FROM ProductsIngredients
	WHERE ProductId = @productId

	DELETE FROM Products
	WHERE Id = @productId
END
--=====================================================--
--Problem 1. Number of Users for Email Provider--
--=====================================================--

USE DIABLO

GO

SELECT SUBSTRING(u.Email, u.AtIndex + 1, LEN(u.Email) - u.AtIndex) AS [Email Provider], COUNT(*) AS [Number Of Users]
  FROM (SELECT Email, CHARINDEX('@', Email, 1) AS AtIndex FROM Users) AS u
 GROUP BY SUBSTRING(u.Email, u.AtIndex + 1, LEN(u.Email) - u.AtIndex)
 ORDER BY [Number Of Users] DESC, [Email Provider]

 GO

--=====================================================--
--Problem 2. All User in Games--
--=====================================================--

  SELECT 
  		g.[Name] AS [Game],
  		gt.[Name] AS [Game Type],
  		u.Username,
  		ug.[Level],
  		ug.Cash,
  		c.[Name] AS [Character]
    FROM Users AS u
    JOIN UsersGames AS ug ON u.Id = ug.UserId
    JOIN Characters AS c ON ug.CharacterId = c.Id
    JOIN Games AS g ON ug.GameId = g.Id
    JOIN GameTypes AS gt ON g.GameTypeId = gt.Id
ORDER BY ug.[Level] DESC, u.Username, [Game]

GO

--=====================================================--
--Problem 3. Users in Games with Their Items--
--=====================================================--

 SELECT 
  		u.Username,
  		g.[Name] AS [Game],
  		COUNT(i.Id) AS [Items Count],
  		SUM(i.Price) AS [Items Price]
    FROM UserGameItems AS ugi
	JOIN Items AS i ON ugi.ItemId = i.Id
    JOIN UsersGames AS ug ON ugi.UserGameId = ug.Id
    JOIN Games AS g ON ug.GameId = g.Id
	JOIN Users AS u ON u.Id = ug.UserId 
GROUP BY u.Username, g.[Name]
  HAVING COUNT(i.Name) >= 10
ORDER BY [Items Count] DESC, [Items Price] DESC, u.Username

--=====================================================--
--Problem 4. * User in Games with Their Statistics--
--=====================================================--

SELECT u.Username, 
	   g.Name AS Game, 
	   MAX(c.Name) AS Character,
	   SUM(si.Strength) + MAX(st.Strength) + MAX(sc.Strength) AS Strength,
	   SUM(si.Defence) + MAX(st.Defence) + MAX(sc.Defence) AS Defence,
	   SUM(si.Speed) + MAX(st.Speed) + MAX(sc.Speed) AS Speed,
	   SUM(si.Mind) + MAX(st.Mind) + MAX(sc.Mind) AS Mind,
	   SUM(si.Luck) + MAX(st.Luck) + MAX(sc.Luck) AS Luck
FROM UsersGames AS ug
JOIN Users AS u ON u.Id = ug.UserId
JOIN Games AS g ON g.Id = ug.GameId
JOIN Characters AS c ON c.Id = ug.CharacterId
JOIN UserGameItems AS ugt ON ugt.UserGameId = ug.Id
JOIN Items AS i ON i.Id = ugt.ItemId
JOIN GameTypes AS gt ON gt.Id = g.GameTypeId
JOIN [Statistics] AS st ON st.Id = gt.BonusStatsId
JOIN [Statistics] AS sc ON sc.Id = c.StatisticId
JOIN [Statistics] AS si ON si.Id = i.StatisticId
GROUP BY u.Username, g.Name
ORDER BY Strength DESC, Defence DESC, Speed DESC, Mind DESC, Luck DESC

--=====================================================--
--Problem 5. All Items with Greater than Average Statistics--
--=====================================================--

SELECT 
	i.[Name],
	i.Price,
	i.MinLevel,
	s.Strength,
	s.Defence,
	s.Speed,
	s.Luck,
	s.Mind
  FROM Items AS i
  JOIN [Statistics] AS s ON i.StatisticId = s.Id
 WHERE s.Mind > (SELECT AVG(Mind) FROM [Statistics])
	AND s.Luck > (SELECT AVG(Luck) FROM [Statistics])
	AND s.Speed > (SELECT AVG(Speed) FROM [Statistics])
ORDER BY i.[Name]

--=====================================================--
--Problem 6. Display All Items with Information about Forbidden Game Type--
--=====================================================--

    SELECT i.[Name] AS Item, i.Price, i.MinLevel, gt.[Name] AS [Forbidden Game Type] 
      FROM GameTypeForbiddenItems AS gtfi
      JOIN GameTypes AS gt ON gt.Id = gtfi.GameTypeId
RIGHT JOIN Items AS i ON i.Id = gtfi.ItemId
  ORDER BY gt.[Name] DESC, i.[Name]

--=====================================================--
--Problem 7. Buy Items for User in Game--
--=====================================================--

GO

DECLARE @AlexMoney MONEY =
	(SELECT ug.Cash FROM UsersGames AS ug
	JOIN Users AS u ON u.Id = ug.UserId
	JOIN Games AS g ON g.Id = ug.GameId
	WHERE u.Username = 'Alex' AND g.Name = 'Edinburgh')

DECLARE @ItemsTotalPrice MONEY =
	(SELECT SUM(Price) FROM Items
	WHERE NAME IN ('Blackguard', 'Bottomless Potion of Amplification',
				'Eye of Etlich (Diablo III)', 'Gem of Efficacious Toxin',
				'Golden Gorget of Leoric', 'Hellfire Amulet'))

DECLARE @AlexUserGameID INT = 
	(SELECT DISTINCT UserGameId FROM UserGameItems
	WHERE UserGameId = 
		(SELECT ug.Id FROM UsersGames AS ug
		JOIN Users AS u ON u.Id = ug.UserId
		JOIN Games AS g ON g.Id = ug.GameId
		WHERE u.Username = 'Alex' AND g.Name = 'Edinburgh')) 

INSERT INTO UserGameItems
SELECT i.Id, @AlexUserGameID FROM Items AS i
WHERE i.Id IN (SELECT Id FROM Items
WHERE NAME IN ('Blackguard', 'Bottomless Potion of Amplification',
			'Eye of Etlich (Diablo III)', 'Gem of Efficacious Toxin',
			'Golden Gorget of Leoric', 'Hellfire Amulet'))

UPDATE UsersGames
SET Cash-= @ItemsTotalPrice 
WHERE Id = @AlexUserGameID

SELECT u.Username, g.Name, ug.Cash, i.Name FROM UsersGames AS ug
JOIN Users AS u ON u.Id = ug.UserId
JOIN Games AS g ON g.Id = ug.GameId
JOIN UserGameItems AS ugt ON ugt.UserGameId = ug.Id
JOIN Items AS i ON i.Id = ugt.ItemId
WHERE g.Name = 'Edinburgh'
ORDER BY i.Name

GO

--=====================================================--
--Problem 8. Peaks and Mountains--
--=====================================================--

USE [Geography]

GO

  SELECT p.PeakName, m.MountainRange AS Mountain, p.Elevation
    FROM Peaks AS p
    JOIN Mountains AS m ON p.MountainId = m.Id
ORDER BY p.Elevation DESC, p.PeakName

--=====================================================--
--Problem 9. Peaks with Their Mountain, Country and Continent--
--=====================================================--

  SELECT p.PeakName, m.MountainRange AS Mountain, ct.CountryName, cn.ContinentName
    FROM Peaks AS p
    JOIN Mountains AS m ON p.MountainId = m.Id
    JOIN MountainsCountries AS mc ON m.Id = mc.MountainId
    JOIN Countries AS ct ON mc.CountryCode = ct.CountryCode
    JOIN Continents AS cn ON ct.ContinentCode = cn.ContinentCode
ORDER BY p.PeakName, ct.CountryName

--=====================================================--
--Problem 10. Rivers by Country--
--=====================================================--

   SELECT 
		ct.CountryName,
		cn.ContinentName,
		COUNT(r.Id) AS RiversCount,
		ISNULL(SUM(r.[Length]), 0) AS TotalLength
     FROM Countries AS ct
     JOIN Continents AS cn ON ct.ContinentCode = cn.ContinentCode
LEFT JOIN CountriesRivers AS cr ON ct.CountryCode = cr.CountryCode
LEFT JOIN Rivers AS r ON cr.RiverId = r.Id
 GROUP BY ct.CountryName, cn.ContinentName
 ORDER BY RiversCount DESC, TotalLength DESC, ct.CountryName

--=====================================================--
--Problem 11. Count of Countries by Currency--
--=====================================================--

    SELECT cr.CurrencyCode, cr.[Description] AS Currency, COUNT(c.CountryCode) AS NumberOfCountries
      FROM Countries AS c
RIGHT JOIN Currencies AS cr ON cr.CurrencyCode = c.CurrencyCode
  GROUP BY cr.CurrencyCode, cr.[Description]
  ORDER BY NumberOfCountries DESC, Currency

--=====================================================--
--Problem 12. Population and Area by Continent--
--=====================================================--

  SELECT 
  	  cn.ContinentName,
  	  SUM(CAST(ct.AreaInSqKm AS BIGINT)) AS CountriesArea,
  	  SUM(CAST(ct.[Population] AS BIGINT)) AS CountriesPopulation
    FROM Continents AS cn
    JOIN Countries AS ct ON cn.ContinentCode = ct.ContinentCode
GROUP BY cn.ContinentName
ORDER BY CountriesPopulation DESC

--=====================================================--
--Problem 13. Monasteries by Country--
--=====================================================--

GO

CREATE TABLE Monasteries
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(200) NOT NULL,
	CountryCode CHAR(2) FOREIGN KEY REFERENCES Countries(CountryCode)
)

INSERT INTO Monasteries([Name], CountryCode) VALUES
('Rila Monastery “St. Ivan of Rila”', 'BG'), 
('Bachkovo Monastery “Virgin Mary”', 'BG'),
('Troyan Monastery “Holy Mother''s Assumption”', 'BG'),
('Kopan Monastery', 'NP'),
('Thrangu Tashi Yangtse Monastery', 'NP'),
('Shechen Tennyi Dargyeling Monastery', 'NP'),
('Benchen Monastery', 'NP'),
('Southern Shaolin Monastery', 'CN'),
('Dabei Monastery', 'CN'),
('Wa Sau Toi', 'CN'),
('Lhunshigyia Monastery', 'CN'),
('Rakya Monastery', 'CN'),
('Monasteries of Meteora', 'GR'),
('The Holy Monastery of Stavronikita', 'GR'),
('Taung Kalat Monastery', 'MM'),
('Pa-Auk Forest Monastery', 'MM'),
('Taktsang Palphug Monastery', 'BT'),
('S?mela Monastery', 'TR')

ALTER TABLE Countries
ADD IsDeleted BIT NOT NULL DEFAULT(0)

UPDATE Countries
SET IsDeleted = 1
WHERE CountryName IN (SELECT c.CountryName
         FROM Countries AS c
         JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
         JOIN Rivers AS r ON cr.RiverId = r.Id
     GROUP BY c.CountryName
	 HAVING COUNT(r.RiverName) > 3)


  SELECT m.[Name] AS Monastery, c.CountryName AS Country
    FROM Monasteries AS m
    JOIN Countries AS c ON m.CountryCode = c.CountryCode
   WHERE c.IsDeleted = 0
ORDER BY Monastery

--=====================================================--
--Problem 14. Monasteries by Continents and Countries--
--=====================================================--

UPDATE Countries
SET CountryName = 'Burma'
WHERE CountryName = 'Myanmar'

INSERT INTO Monasteries([Name], CountryCode) VALUES
	('Hanga Abbey', (SELECT CountryCode FROM Countries WHERE CountryName = 'Tanzania')),
	('Myin-Tin-Daik', (SELECT CountryCode FROM Countries WHERE CountryName = 'Myanmar'))

   SELECT co.ContinentName, c.CountryName, COUNT(m.Id) AS MonasteriesCount 
     FROM Continents AS co
     JOIN Countries AS c ON c.ContinentCode = co.ContinentCode
LEFT JOIN Monasteries AS m ON m.CountryCode = c.CountryCode
    WHERE c.IsDeleted = 0
 GROUP BY co.ContinentName, c.CountryName
 ORDER BY MonasteriesCount DESC, c.CountryName
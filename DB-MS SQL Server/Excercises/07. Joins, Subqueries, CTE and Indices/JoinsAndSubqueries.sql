--=====================================================--
--Problem 1. Employee Address--
--=====================================================--

USE SoftUni

  SELECT TOP(5) e.EmployeeID,
	     e.JobTitle,
	     e.AddressID,
	     a.AddressText	
    FROM Employees AS e
    JOIN Addresses AS a ON e.AddressID = a.AddressID
ORDER BY AddressID

--=====================================================--
--Problem 2. Addresses with Towns--
--=====================================================--

  SELECT TOP(50) e.FirstName,
			e.LastName,
			t.[Name] AS Town,
			a.AddressText
	FROM Employees AS e
	JOIN Addresses AS a ON e.AddressID = a.AddressID
	JOIN Towns AS t ON a.TownID = t.TownID
ORDER BY FirstName, LastName

--=====================================================--
--Problem 3. Sales Employee--
--=====================================================--

  SELECT e.EmployeeID,
		 e.FirstName,
		 e.LastName,
		 d.[Name] AS DepartmentName
    FROM Employees AS e
    JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
   WHERE d.Name = 'Sales'
ORDER BY EmployeeID

--=====================================================--
--Problem 4. Employee Departments--
--=====================================================--

  SELECT TOP(5) e.EmployeeID,
				e.FirstName,
				e.Salary,
				d.[Name] AS DepartmentName
	FROM Employees AS e
	JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
   WHERE Salary > 15000
ORDER BY d.DepartmentID

--=====================================================--
--Problem 5. Employees Without Project--
--=====================================================--

  SELECT TOP(3) e.EmployeeID, e.FirstName
    FROM Employees AS e
    FULL JOIN EmployeesProjects AS ep ON ep.EmployeeID = e.EmployeeID
   WHERE ep.ProjectID IS NULL
ORDER BY e.EmployeeID

--=====================================================--
--Problem 6. Employees Hired After--
--=====================================================--

  SELECT e.FirstName, 
		 e.LastName, 
		 e.HireDate, 
		 d.[Name] AS DeptName
	FROM Employees AS e
	JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
   WHERE e.HireDate > '01/01/1999' AND d.Name IN ('Sales', 'Finance')
ORDER BY e.HireDate

--=====================================================--
--Problem 7. Employees with Project--
--=====================================================--

  SELECT TOP(5) e.EmployeeID, 
				e.FirstName,
				p.[Name] AS ProjectName
	FROM Employees AS e
	JOIN EmployeesProjects AS ep ON e.EmployeeID = ep.EmployeeID
	JOIN Projects AS p ON ep.ProjectID = p.ProjectID
   WHERE p.StartDate > '08/13/2002' AND p.EndDate IS NULL
ORDER BY e.EmployeeID

--=====================================================--
--Problem 8. Employee 24--
--=====================================================--

SELECT e.EmployeeID, 
	   e.FirstName, 
	   IIF(p.StartDate <= '01/01/2005', p.[Name], NULL) AS ProjectName
 FROM Employees AS e
 JOIN EmployeesProjects AS ep ON e.EmployeeID = ep.EmployeeID
 JOIN Projects AS p ON ep.ProjectID = p.ProjectID 
WHERE e.EmployeeID = 24 

--=====================================================--
--Problem 9. Employee Manager--
--=====================================================--

  SELECT e.EmployeeID, 
		 e.FirstName, 
		 e.ManagerID, 
		 mg.FirstName AS ManagerName
	FROM Employees AS e
	JOIN Employees AS mg ON e.ManagerID = mg.EmployeeID 
   WHERE mg.EmployeeID IN (3, 7)
ORDER BY e.EmployeeID

--=====================================================--
--Problem 10. Employee Summary--
--=====================================================--

  SELECT TOP(50) e.EmployeeID,
		 CONCAT(e.FirstName, ' ', e.LastName) AS EmployeeName,
		 CONCAT(mg.FirstName, ' ', mg.LastName) AS ManagerName, 
		 d.[Name] AS DepartmentName
	FROM Employees AS e
	JOIN Employees AS mg ON mg.EmployeeID = e.ManagerID
	JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
ORDER BY e.EmployeeID

--=====================================================--
--Problem 11. Min Average Salary--
--=====================================================--

  SELECT TOP(1) AVG(e.Salary) AS MinAverageSalary
	FROM Employees AS e
	JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
GROUP BY d.Name
ORDER BY MinAverageSalary

--=====================================================--
--Problem 12. Highest Peaks in Bulgaria--
--=====================================================--

USE Geography

  SELECT c.CountryCode,
		 m.MountainRange,
		 p.PeakName, 
		 p.Elevation
	FROM Countries AS c
	JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
	JOIN Mountains AS m ON mc.MountainId = m.Id
	JOIN Peaks AS p ON mc.MountainId = p.MountainId
   WHERE p.Elevation > 2835 AND c.CountryCode = 'BG'
ORDER BY p.Elevation DESC

--=====================================================--
--Problem 13. Count Mountain Ranges--
--=====================================================--

  SELECT CountryCode, COUNT(CountryCode) AS MountainRanges
	FROM MountainsCountries
GROUP BY CountryCode
  HAVING CountryCode IN ('US', 'RU', 'BG')

--=====================================================--
--Problem 14. Countries with Rivers--
--=====================================================--

   SELECT TOP(5) c.CountryName, r.RiverName
	 FROM Countries AS c 
LEFT JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
LEFT JOIN Rivers AS r ON cr.RiverId = r.Id
    WHERE ContinentCode = 'AF'
 ORDER BY c.CountryName

--=====================================================--
--Problem 15. *Continents and Currencies--
--=====================================================--
  SELECT k.ContinentCode,
		 k.CurrencyCode, 
		 k.CurrencyUsage
	FROM
(
  SELECT c.ContinentCode,
		 co.CurrencyCode, 
		 COUNT(co.CurrencyCode) AS CurrencyUsage,
		 DENSE_RANK() OVER (PARTITION BY c.ContinentCode ORDER BY COUNT(co.CurrencyCode) DESC) AS [Rank]
	FROM Continents AS c
	JOIN Countries AS co ON c.ContinentCode = co.ContinentCode
GROUP BY c.ContinentCode, co.CurrencyCode
  HAVING COUNT(co.CurrencyCode) > 1) AS k
  WHERE k.[Rank] = 1
ORDER BY k.ContinentCode

--=====================================================--
--Problem 16. Countries without any Mountains--
--=====================================================--

   SELECT COUNT(c.CountryCode)
	 FROM Countries AS c
LEFT JOIN MountainsCountries AS m ON c.CountryCode = m.CountryCode
	WHERE m.CountryCode IS NULL

--=====================================================--
--Problem 17. Highest Peak and Longest River by Country--
--=====================================================--

   SELECT TOP(5) c.CountryName,
				 MAX(p.Elevation) AS HighestPeakElevation,
				 MAX(r.[Length]) AS LongestRiverLength
	 FROM Countries AS c
LEFT JOIN MountainsCountries AS mc ON  c.CountryCode = mc.CountryCode
LEFT JOIN Mountains AS m ON mc.MountainId = m.Id
LEFT JOIN Peaks AS p ON m.Id = p.MountainId
LEFT JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
LEFT JOIN Rivers AS r ON cr.RiverId = r.Id
 GROUP BY c.CountryName
 ORDER BY HighestPeakElevation DESC, LongestRiverLength DESC, c.CountryName

--=====================================================--
--Problem 18. * Highest Peak Name and Elevation by Country--
--=====================================================--

SELECT TOP(5) k.Country, 
	   ISNULL(k.[Highest Peak Name], '(no highest peak)'),
	   ISNULL(k.[Highest Peak Elevation], 0),
	   ISNULL(k.Mountain, '(no mountain)')
  FROM(
		   SELECT c.CountryName AS Country,
			      p.PeakName AS [Highest Peak Name],
			      MAX(p.Elevation) AS [Highest Peak Elevation],
			      m.MountainRange AS Mountain,
			      DENSE_RANK() OVER(PARTITION BY c.CountryName ORDER BY MAX(p.Elevation) DESC) AS [Rank]
			 FROM Countries AS c
		LEFT JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
		LEFT JOIN Mountains AS m ON mc.MountainId = m.Id
		LEFT JOIN Peaks AS p ON m.Id = p.MountainId
		 GROUP BY c.CountryName, m.MountainRange, p.PeakName) AS k
 WHERE k.[Rank] = 1
 ORDER BY k.Country, k.[Highest Peak Name]
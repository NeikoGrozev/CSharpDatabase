--=====================================================--
--Problem 1. Employees with Salary Above 35000--
--=====================================================--

USE SoftUni

GO

CREATE PROC usp_GetEmployeesSalaryAbove35000 
AS
BEGIN
	SELECT E.FirstName, E.LastName
	FROM Employees AS e
	WHERE e.Salary > 35000
END

EXEC usp_GetEmployeesSalaryAbove35000

--=====================================================--
--Problem 2. Employees with Salary Above Number--
--=====================================================--

GO

CREATE PROC usp_GetEmployeesSalaryAboveNumber @currentSalary DECIMAL(18, 4)
AS
BEGIN
	SELECT E.FirstName, e.LastName
	FROM Employees AS e
	WHERE E.Salary >= @currentSalary
END

EXEC usp_GetEmployeesSalaryAboveNumber 48100

--=====================================================--
--Problem 3. Town Names Starting With--
--=====================================================--

GO

CREATE PROC usp_GetTownsStartingWith @startSymbols VARCHAR(10)
AS
BEGIN
	SELECT t.[Name] AS Town
	FROM Towns AS t
	WHERE t.[Name] LIKE @startSymbols + '%'
END

EXEC usp_GetTownsStartingWith B

--=====================================================--
--Problem 4. Employees from Town--
--=====================================================--

GO

CREATE PROC usp_GetEmployeesFromTown @town NVARCHAR(30)
AS
BEGIN
	SELECT e.FirstName, e.LastName
	FROM Towns AS t
	JOIN Addresses AS a ON t.TownID = a.TownID
	JOIN Employees AS e ON a.AddressID = e.AddressID
	WHERE t.[Name] = @town
END

EXEC usp_GetEmployeesFromTown 'Sofia'

--=====================================================--
--Problem 5. Salary Level Function--
--=====================================================--

GO

CREATE FUNCTION ufn_GetSalaryLevel(@salary DECIMAL(18,4)) 
RETURNS VARCHAR(10)
AS
BEGIN

	DECLARE @result VARCHAR(10)

	IF(@salary < 30000)
		SET @result = 'Low'	
	ELSE IF(@salary BETWEEN 30000 AND 50000)
		SET @result ='Average'
	ELSE	
		SET @result = 'High'
	
	RETURN @result
END

GO

SELECT Salary, dbo.ufn_GetSalaryLevel(Salary) AS [Salary Level]
FROM Employees

--=====================================================--
--Problem 6. Employees by Salary Level--
--=====================================================--

GO

CREATE PROC usp_EmployeesBySalaryLevel(@level VARCHAR(10))
AS
BEGIN
	SELECT e.FirstName, e.LastName
	FROM Employees AS e
	WHERE dbo.ufn_GetSalaryLevel(Salary) = @level
END

EXEC usp_EmployeesBySalaryLevel 'High'

--=====================================================--
--Problem 7. Define Function--
--=====================================================--

GO

CREATE FUNCTION ufn_IsWordComprised(@setOfLetters VARCHAR(MAX), @word VARCHAR(100)) 
RETURNS BIT
AS
BEGIN

	DECLARE @result BIT = 1
	DECLARE @count INT = 1

	WHILE @count <= LEN(@word)
	BEGIN
		DECLARE @currentSymbol VARCHAR(1) = SUBSTRING(@word, @count, 1)

		IF(CHARINDEX(@currentSymbol, @setOfLetters) > 0)
		BEGIN
			SET @count += 1
		END
		ELSE
		BEGIN
			SET @result = 0
			BREAK
		END
	END

	RETURN @result
END

GO

SELECT dbo.ufn_IsWordComprised ('oistmiahf', 'Sofia') AS [Result]
SELECT dbo.ufn_IsWordComprised ('oistmiahf', 'halves') AS [Result]

--=====================================================--
--Problem 8. * Delete Employees and Departments--
--=====================================================--

GO

CREATE PROC usp_DeleteEmployeesFromDepartment (@departmentId INT)
AS
BEGIN
	DELETE FROM EmployeesProjects
	WHERE EmployeeID IN (
		SELECT EmployeeID
		FROM Employees
		WHERE DepartmentID = @departmentId
	)

	UPDATE Employees
	SET ManagerID = NULL
	WHERE ManagerID IN (
		SELECT EmployeeID
		FROM Employees
		WHERE DepartmentID = @departmentId
	)
	
	ALTER TABLE Departments
	ALTER COLUMN ManagerId INT

	UPDATE Departments
	SET ManagerID = NULL
	WHERE DepartmentID = @departmentId

	DELETE FROM Employees
	WHERE DepartmentID = @departmentId

	DELETE FROM Departments
	WHERE DepartmentID = @departmentId

	SELECT COUNT(*)
	FROM Employees
	WHERE DepartmentID = @departmentId
END

--=====================================================--
--Problem 9. Find Full Name--
--=====================================================--
GO

USE Bank

GO

CREATE PROC usp_GetHoldersFullName 
AS
BEGIN
	SELECT CONCAT(a.FirstName, ' ', a.LastName) AS [Full Name]
	FROM AccountHolders AS a
END

EXEC usp_GetHoldersFullName

--=====================================================--
--Problem 10. People with Balance Higher Than--
--=====================================================--

GO

CREATE PROC usp_GetHoldersWithBalanceHigherThan (@num DECIMAL(15, 2))
AS
BEGIN
	SELECT ah.FirstName, ah.LastName
	FROM AccountHolders AS ah
	JOIN Accounts AS a ON ah.Id = a.AccountHolderId
	GROUP BY ah.FirstName, ah.LastName
	HAVING SUM(a.Balance) > @num
	ORDER BY ah.FirstName, ah.LastName
END

EXEC usp_GetHoldersWithBalanceHigherThan 100.50

--=====================================================--
--Problem 11. Future Value Function--
--=====================================================--

GO

CREATE FUNCTION ufn_CalculateFutureValue (@sum DECIMAL(15, 2), @yearlyInterestRate FLOAT, @numberOfYears INT)
RETURNS DECIMAL(15, 4)
AS
BEGIN
	RETURN @sum * POWER(1 + @yearlyInterestRate, @numberOfYears)	
END

GO

SELECT dbo.ufn_CalculateFutureValue(1000, 0.1, 5) AS Output

--=====================================================--
--Problem 12. Calculating Interest--
--=====================================================--

GO

CREATE PROC usp_CalculateFutureValueForAccount(@accountId INT, @yearlyInterestRate FLOAT)
AS
BEGIN
	SELECT a.Id AS [Account Id],
			ah.FirstName AS [First Name],
			ah.LastName AS [Last Name],
			a.Balance AS [Current Balance],
			dbo.ufn_CalculateFutureValue(a.Balance, @yearlyInterestRate, 5) AS [Balance in 5 years]
	FROM Accounts AS a
	JOIN AccountHolders AS ah ON a.AccountHolderId = ah.Id
	WHERE a.Id = @accountId
END

EXEC dbo.usp_CalculateFutureValueForAccount 1, 0.1

--=====================================================--
--Problem 13. *Scalar Function: Cash in User Games Odd Rows--
--=====================================================--

GO

USE Diablo

GO

CREATE FUNCTION ufn_CashInUsersGames(@game VARCHAR(50))
RETURNS TABLE
AS
RETURN
	SELECT SUM(k.Cash) AS SumCash
	FROM (
		SELECT ug.Cash,
			   ROW_NUMBER() OVER(PARTITION BY g.[Name] ORDER BY ug.Cash DESC) AS [Row]
		FROM Games AS g
		JOIN UsersGames AS ug ON g.Id = ug.GameId
		WHERE g.[Name] = @game) AS k
	WHERE k.[Row] % 2 = 1

GO
	
--=====================================================--
--Problem 14. Create Table Logs--
--=====================================================--

USE Bank

GO

CREATE TABLE Logs
(
	LogId INT PRIMARY KEY IDENTITY,
	AccountId INT,
	OldSum DECIMAL(15, 2),
	NewSum DECIMAL(15, 2)
)

GO

CREATE TRIGGER tr_UpdateBalance ON Accounts FOR UPDATE
AS
BEGIN
	DECLARE @newSum DECIMAL(15, 2) = (SELECT i.[Balance] FROM [INSERTED] AS i)
	DECLARE @oldSum DECIMAL(15, 2) = (SELECT d.[Balance] FROM [DELETED] AS d)
	DECLARE @accountId INT = (SELECT i.[Id] FROM [INSERTED] AS i)

	INSERT INTO Logs
	(
	    AccountId,
	    OldSum,
	    NewSum
	)
	VALUES
	(
		@accountId,
		@oldSum,
		@newSum
	)
END

--=====================================================--
--Problem 15. Create Table Emails--
--=====================================================--

CREATE TABLE NotificationEmails
(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	Recipient INT,
	[Subject] VARCHAR(500),
	Body VARCHAR(500)
)

GO

CREATE TRIGGER tr_AddNewEmail ON Logs FOR INSERT
AS
BEGIN
	DECLARE @recipient INT = (SELECT i.AccountId FROM INSERTED AS i)
	DECLARE @oldSum DECIMAL(15, 2) = (SELECT i.OldSum FROM INSERTED AS i)
	DECLARE @newSum DECIMAL(15, 2) = (SELECT i.NewSum FROM INSERTED AS i)

	INSERT INTO NotificationEmails
	(
	    Recipient,
	    [Subject],
	    Body
	)
	VALUES
	(
	    @recipient,
	    'Balance change for account: ' + CAST(@recipient AS VARCHAR(15)),
	    'On ' + CAST(GETDATE() AS VARCHAR(50)) + ' your balance was changed from ' +
	    CAST(@oldSum AS VARCHAR(30)) + ' to ' +
	    CAST(@newSum AS VARCHAR(50)) + '.'
	)
END

--=====================================================--
--Problem 16. Deposit Money--
--=====================================================--

GO

CREATE PROCEDURE usp_DepositMoney (@accountId INT, @moneyAmount DECIMAL(15, 4))
AS
BEGIN
	DECLARE @targetAccountId INT = (SELECT a.Id FROM Accounts a 
									WHERE a.Id = @accountId)

	IF(@moneyAmount < 0 OR @moneyAmount IS NULL)
	BEGIN
		ROLLBACK
		RAISERROR('Invalid amount of money', 16, 1)
		RETURN
	END
	
	IF(@targetAccountId IS NULL)
	BEGIN
		ROLLBACK
		RAISERROR('Invalid Id Parameter', 16, 2)
		RETURN
	END

	UPDATE Accounts
	   SET
	       Accounts.Balance += @moneyAmount
	 WHERE Accounts.Id = @accountId
END

--=====================================================--
--Problem 17. Withdraw Money--
--=====================================================--

GO

CREATE PROCEDURE usp_WithdrawMoney (@accountId INT, @moneyAmount DECIMAL(15, 4))
AS
BEGIN
	DECLARE @targetAccountId INT = (SELECT Id FROM Accounts AS a 
									WHERE a.Id = @accountId)
	
	IF(@targetAccountId IS NULL)
	BEGIN
		ROLLBACK
		RAISERROR('Invalid Id Parameter', 16, 1)
		RETURN
	END
	
	DECLARE @targetBalance DECIMAL(15, 4) = (SELECT Balance FROM Accounts AS a 
											WHERE a.Id = @targetAccountId)
	
	IF(@moneyAmount < 0)
	BEGIN
		ROLLBACK
		RAISERROR('Invalid amount of money', 16, 2)
		RETURN
	END
	
	IF(@targetBalance - @moneyAmount < 0)
	BEGIN
		ROLLBACK
		RAISERROR('Invalid amount of money left', 16, 3)
		RETURN
	END
	
	UPDATE Accounts
	SET
	    Balance -= @moneyAmount
	WHERE Id = @accountId
END

--=====================================================--
--Problem 18. Money Transfer--
--=====================================================--

GO

CREATE PROCEDURE usp_TransferMoney(@senderId INT, @receiverId INT, @amount DECIMAL(15, 4))
AS
BEGIN
	DECLARE @targetSender INT = (SELECT Id FROM Accounts AS a 
								 WHERE a.Id = @senderId)
	DECLARE @targetReciver INT = (SELECT Id FROM Accounts AS a 
								  WHERE a.Id = @receiverId)
	
	IF(@targetReciver IS NULL OR @targetSender IS NULL)
	BEGIN
		ROLLBACK
		RAISERROR('Invalid Id Parameter', 16, 1)
		RETURN
	END
	
	IF(@amount < 0)
	BEGIN
		ROLLBACK
		RAISERROR('Invalid amount of money', 16, 2)
		RETURN
	END
	
	EXEC dbo.usp_WithdrawMoney @targetSender, @amount
	EXEC dbo.usp_DepositMoney @targetReciver, @amount
END

--=====================================================--
--Problem 20. *Massive Shopping--
--=====================================================--
GO

DECLARE @gameName NVARCHAR(50) = 'Safflower'
DECLARE @username NVARCHAR(50) = 'Stamat'

DECLARE @userGameId INT = (SELECT ug.Id
						   FROM UsersGames AS ug
						   JOIN Users AS u ON ug.UserId = u.Id
						   JOIN Games AS g ON ug.GameId = g.Id
						   WHERE u.Username = @username AND g.[Name] = @gameName
						   )

DECLARE @userGameLevel INT = (SELECT Level
                              FROM UsersGames
                              WHERE Id = @userGameId)

DECLARE @itemsCost DECIMAL(15, 2), @availableCash DECIMAL(15, 2), @minLevel INT, @maxLevel INT

SET @minLevel = 11
SET @maxLevel = 12
SET @availableCash = (SELECT Cash
                      FROM UsersGames
                      WHERE Id = @userGameId)

SET @itemsCost = (SELECT SUM(Price)
                  FROM Items
                  WHERE MinLevel BETWEEN @minLevel AND @maxLevel)

IF (@availableCash >= @itemsCost AND @userGameLevel >= @maxLevel)

  BEGIN
    BEGIN TRANSACTION
    UPDATE UsersGames
    SET Cash -= @itemsCost
    WHERE Id = @userGameId
    IF (@@ROWCOUNT <> 1)
      BEGIN
        ROLLBACK
        RAISERROR ('Could not make payment', 16, 1)
      END
    ELSE
      BEGIN
        INSERT INTO UserGameItems (ItemId, UserGameId)
          (SELECT
             Id,
             @userGameId
           FROM Items
           WHERE MinLevel BETWEEN @minLevel AND @maxLevel)

        IF ((SELECT COUNT(*)
             FROM Items
             WHERE MinLevel BETWEEN @minLevel AND @maxLevel) <> @@ROWCOUNT)
          BEGIN
            ROLLBACK;
            RAISERROR ('Could not buy items', 16, 1)
          END
        ELSE COMMIT;
      END
  END

SET @minLevel = 19
SET @maxLevel = 21
SET @availableCash = (SELECT Cash
                      FROM UsersGames
                      WHERE Id = @userGameId)

SET @itemsCost = (SELECT SUM(Price)
                  FROM Items
                  WHERE MinLevel BETWEEN @minLevel AND @maxLevel)

IF (@availableCash >= @itemsCost AND @userGameLevel >= @maxLevel)

  BEGIN
    BEGIN TRANSACTION
    UPDATE UsersGames
    SET Cash -= @itemsCost
    WHERE Id = @userGameId

    IF (@@ROWCOUNT <> 1)
      BEGIN
        ROLLBACK
        RAISERROR ('Could not make payment', 16, 1)
      END
    ELSE
      BEGIN
        INSERT INTO UserGameItems (ItemId, UserGameId)
          (SELECT
             Id,
             @userGameId
           FROM Items
           WHERE MinLevel BETWEEN @minLevel AND @maxLevel)

        IF ((SELECT COUNT(*)
             FROM Items
             WHERE MinLevel BETWEEN @minLevel AND @maxLevel) <> @@ROWCOUNT)
          BEGIN
            ROLLBACK
            RAISERROR ('Could not buy items', 16, 1)
          END
        ELSE COMMIT;
      END
  END

SELECT i.Name AS [Item Name]
FROM UserGameItems AS ugi
  JOIN Items AS i
    ON i.Id = ugi.ItemId
  JOIN UsersGames AS ug
    ON ug.Id = ugi.UserGameId
  JOIN Games AS g
    ON g.Id = ug.GameId
WHERE g.Name = @gameName
ORDER BY [Item Name]

--=====================================================--
--Problem 21. Employees with Three Projects--
--=====================================================--
GO

USE SoftUni

GO

CREATE PROCEDURE usp_AssignProject(@emloyeeId INT, @projectID INT)
AS
BEGIN TRANSACTION
DECLARE @projects INT = (SELECT COUNT(ep.ProjectID)
                           FROM Employees AS e
						   JOIN EmployeesProjects AS ep ON e.EmployeeID = ep.EmployeeID
						  WHERE e.EmployeeID = @emloyeeId)

IF(@projects >= 3)
BEGIN
	ROLLBACK
	RAISERROR('The employee has too many projects!', 16, 1)
	RETURN
END

INSERT INTO EmployeesProjects (EmployeeID, ProjectID) VALUES
	(@emloyeeId, @projectID)

COMMIT

--=====================================================--
--Problem 22. Delete Employees--
--=====================================================--

CREATE TABLE Deleted_Employees
(
	EmployeeId INT PRIMARY KEY,
	FirstName VARCHAR(50),
	LastName VARCHAR(50),
	MiddleName VARCHAR(50),
	JobTitle VARCHAR(50),
	DepartmentId INT,
	Salary DECIMAL(15, 2)
)

GO

CREATE TRIGGER tr_DeletedEmployees ON Employees FOR DELETE
AS
BEGIN
	INSERT INTO Deleted_Employees
	(
	    FirstName,
	    LastName,
	    MiddleName,
	    JobTitle,
	    DepartmentId,
	    Salary
	)
	SELECT d.FirstName,
		   d.LastName,
		   d.MiddleName,
		   d.JobTitle,
		   d.DepartmentID,
		   d.Salary
      FROM Deleted AS d
END
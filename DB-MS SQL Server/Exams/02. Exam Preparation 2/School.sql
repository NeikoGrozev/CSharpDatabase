--=====================================================--
--Problem 1. Database Design--
--=====================================================--

CREATE DATABASE School

USE School

CREATE TABLE Students
(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(30) NOT NULL,
	MiddleName NVARCHAR(25),
	LastName NVARCHAR(30) NOT NULL,
	Age INT CHECK(AGE BETWEEN 5 AND 100) NOT NULL,
	[Address] NVARCHAR(50),
	Phone NCHAR(10)
)

CREATE TABLE Subjects
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(20) NOT NULL,
	Lessons INT CHECK(Lessons > 0) 
)

CREATE TABLE StudentsSubjects
(
	Id INT PRIMARY KEY IDENTITY,
	StudentId INT FOREIGN KEY REFERENCES Students(Id) NOT NULL,
	SubjectId INT FOREIGN KEY REFERENCES Subjects(Id) NOT NULL,
	Grade DECIMAL(3, 2) CHECK(Grade BETWEEN 2 AND 6) NOT NULL
)

CREATE TABLE Exams
(
	Id INT PRIMARY KEY IDENTITY,
	[Date] DATETIME2,
	SubjectId INT FOREIGN KEY REFERENCES Subjects(Id) NOT NULL
)

CREATE TABLE StudentsExams
(
	StudentId INT FOREIGN KEY REFERENCES Students(Id) NOT NULL,
	ExamId INT FOREIGN KEY REFERENCES Exams(Id) NOT NULL,
	Grade DECIMAL(3, 2) CHECK(Grade BETWEEN 2 AND 6) NOT NULL
	CONSTRAINT PK_StudentsExams PRIMARY KEY(StudentId, ExamId)
)

CREATE TABLE Teachers
(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(20) NOT NULL,
	LastName NVARCHAR(20) NOT NULL,
	[Address] NVARCHAR(20) NOT NULL,
	Phone CHAR(10),
	SubjectId INT FOREIGN KEY REFERENCES Subjects(Id)
)

CREATE TABLE StudentsTeachers
(
	StudentId INT FOREIGN KEY REFERENCES Students(Id),
	TeacherId INT FOREIGN KEY REFERENCES Teachers(Id)
	CONSTRAINT PK_StudentsTeachers PRIMARY KEY(StudentId, TeacherId)
)

--=====================================================--
--Problem 2. Insert--
--=====================================================--

INSERT INTO Teachers(FirstName, LastName, [Address], Phone, SubjectId) VALUES
('Ruthanne', 'Bamb', '84948 Mesta Junction', '3105500146', 6),
('Gerrard',	'Lowin', '370 Talisman Plaza', '3324874824', 2),
('Merrile',	'Lambdin', '81 Dahle Plaza', '4373065154', 5),
('Bert', 'Ivie', '2 Gateway Circle', '4409584510', 4)

INSERT INTO Subjects([Name], Lessons) VALUES
('Geometry', 12),
('Health', 10),
('Drama', 7),
('Sports', 9)

--=====================================================--
--Problem 3. Update--
--=====================================================--

UPDATE StudentsSubjects
SET Grade = 6.00
WHERE Grade >= 5.50 AND SubjectId IN (1 ,2)

--=====================================================--
--Problem 4. Delete--
--=====================================================--

DELETE FROM StudentsTeachers
WHERE TeacherId IN (SELECT Id FROM Teachers
					WHERE Phone LIKE '%72%')

DELETE FROM Teachers
WHERE Phone LIKE '%72%'

--=====================================================--
--Problem 5. Teen Students--
--=====================================================--

SELECT FirstName, LastName, Age
FROM Students
WHERE Age >= 12
ORDER BY FirstName, LastName

--=====================================================--
--Problem 6. Students Teachers--
--=====================================================--

SELECT s.FirstName, s.LastName, COUNT(st.TeacherId) AS TeachersCount
FROM Students AS s
JOIN StudentsTeachers AS st ON st.StudentId = s.Id
GROUP BY s.FirstName, s.LastName

--=====================================================--
--Problem 7. Students to Go--
--=====================================================--

SELECT CONCAT(s.FirstName, ' ', s.LastName) AS [Full Name]
FROM Students AS s
LEFT JOIN StudentsExams AS sx ON s.Id = sx.StudentId
WHERE sx.ExamId IS NULL
ORDER BY [Full Name]

--=====================================================--
--Problem 8. Top Students--
--=====================================================--

SELECT TOP(10) k.[First Name], k.[Last Name], k.Grade 
FROM (SELECT s.FirstName AS [First Name], s.LastName AS [Last Name], FORMAT(AVG(sx.Grade), 'N2') AS Grade
	FROM Students AS s
	JOIN StudentsExams AS sx ON s.Id = sx.StudentId
	GROUP BY s.FirstName, s.LastName) AS k
ORDER BY k.Grade DESC, [First Name], [Last Name]

--=====================================================--
--Problem 9. Not So In The Studying--
--=====================================================--

SELECT CONCAT(FirstName, ' ', MiddleName  + ' ', LastName)  AS [Full Name]
FROM Students AS s
LEFT JOIN StudentsSubjects AS ss ON s.Id = ss.StudentId
WHERE ss.SubjectId IS NULL
ORDER BY [Full Name]

--=====================================================--
--Problem 10. Average Grade per Subject--
--=====================================================--

SELECT k.Name, k.AverageGrade
FROM (SELECT s.Id AS Id, s.[Name] AS [Name], AVG(ss.Grade) AS AverageGrade
		FROM Subjects AS s
		JOIN StudentsSubjects AS ss ON s.Id = ss.SubjectId
		GROUP BY s.Id, s.[Name]) AS k
ORDER BY k.Id

--=====================================================--
--Problem 11. Exam Grades--
--=====================================================--

GO

CREATE FUNCTION udf_ExamGradesToUpdate(@studentId INT, @grade DECIMAL(3, 2))
RETURNS VARCHAR(MAX)
AS
BEGIN
	IF(@grade > 6.00)
	BEGIN 
		RETURN 'Grade cannot be above 6.00!'
	END

	DECLARE @student VARCHAR(50) = (SELECT FirstName FROM Students
										WHERE Id = @studentId)

	IF(@student IS NULL)
	BEGIN
		RETURN 'The student with provided id does not exist in the school!'
	END
	
	DECLARE @gradeCount INT = (SELECT COUNT(sx.Grade) FROM Students AS s
								JOIN StudentsExams AS sx ON s.Id = sx.StudentId
								WHERE s.Id = @studentId AND sx.Grade BETWEEN @grade AND @grade + 0.50)
	RETURN CONCAT('You have to update ', @gradeCount, ' grades for the student ', @student)
END 

GO

SELECT dbo.udf_ExamGradesToUpdate(12, 6.20)
SELECT dbo.udf_ExamGradesToUpdate(12, 5.50)
SELECT dbo.udf_ExamGradesToUpdate(121, 5.50)

--=====================================================--
--Problem 12. Exclude from school--
--=====================================================--

GO

CREATE PROC usp_ExcludeFromSchool @StudentId INT
AS 
BEGIN
	DECLARE @student INT = (SELECT Id FROM Students
							WHERE Id = @StudentId)
	IF(@student IS NULL)
	BEGIN
		RAISERROR ('This school has no student with the provided id!', 16, 1)
		RETURN 
	END
	
	DELETE FROM StudentsTeachers
	WHERE StudentId = @StudentId
	
	DELETE FROM StudentsSubjects
	WHERE StudentId = @StudentId
	
	DELETE FROM StudentsExams
	WHERE StudentId = @StudentId
	
	DELETE FROM Students
	WHERE ID = @StudentId
END

EXEC usp_ExcludeFromSchool 1
SELECT COUNT(*) FROM Students

EXEC usp_ExcludeFromSchool 301
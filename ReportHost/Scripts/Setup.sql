CREATE DATABASE [DataTests];
GO
USE [DataTests];
GO

IF OBJECT_ID('[dbo].[Reports]', 'U') IS NOT NULL
BEGIN
	DROP TABLE [dbo].[Reports]
END
GO

CREATE TABLE [dbo].[Reports]
(
	[id] INT NOT NULL IDENTITY(1,1),
	[Name] VARCHAR(30) NOT NULL,
	[Query] VARCHAR(max),
	[CreatedBy] VARCHAR(50) NOT NULL,
	[Created] DATETIME NOT NULL CONSTRAINT [DF_Created_Reports] DEFAULT(GETDATE()),
);
GO

IF OBJECT_ID('[dbo].[Accounts]', 'U') IS NOT NULL
BEGIN
	DROP TABLE [dbo].[Accounts]
END
GO

CREATE TABLE [dbo].[Accounts]
(
	[Id] INT NOT NULL IDENTITY(1,1),
	[Name] varchar(100),
	[EmailAddress] varchar(250),
	[Created] DateTime NOT NULL CONSTRAINT [DF_Created_Accounts] DEFAULT(GETDATE()),
	CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	)
);
GO

/*	Create 1 Million Accounts */
SET NOCOUNT ON;
DECLARE @TotalAccounts INT = 1000000,
		@i INT = 1,
		@email varchar(250) = 'email_{0}@blnk.com',
		@name varchar(100) = 'name_{0}',
		@e varchar(250),
		@n varchar(100);


WHILE @i < @TotalAccounts
BEGIN
	SET @e = REPLACE(@email, '{0}', CONVERT(varchar, @i));
	SET @n = REPLACE(@name, '{0}', CONVERT(varchar, @i));

	INSERT INTO [dbo].[Accounts] ([Name], [EmailAddress])
	VALUES (@n, @e);

	SET @i += 1;
END
GO


/*	Create a Report for Accounts */
INSERT INTO [dbo].[Reports] ([Name], [Query], [CreatedBy])
VALUES ('Base Accounts Report', 'SELECT * FROM [dbo].[Accounts]', 'jcanady');
GO
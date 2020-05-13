CREATE DATABASE vulnerable
GO
USE [VULNERABLE]
GO 

CREATE TABLE Users (
    [LastName] [varchar](255) NOT NULL,
    [FirstName] [varchar](255) NOT NULL,
    [Username] [varchar](255) NOT NULL,
    [EncryptedPassword] [varchar](255) NOT NULL,
    [Role] [int] NOT NULL
);
GO

INSERT INTO Users (LastName, FirstName, Username, EncryptedPassword, Role) VALUES ('Chairman','The','The_Chairman','IRUCBQ8EVEtKVVFsYWNlcg==',0);
GO

CREATE TABLE Sessions (
    [Username] [varchar](255) NOT NULL,
    [ID] [uniqueidentifier] NOT NULL
);
GO

CREATE TABLE BetaBankBalances (
    [Username] [varchar](255) NOT NULL,
    [Balance] [money] NOT NULL,
    [CurrencySymbol] [varchar](1) NOT NULL
);
GO

INSERT INTO BetaBankBalances (Username, Balance, CurrencySymbol) VALUES ('The_Chairman', 2801274696878, '$');
GO
INSERT INTO BetaBankBalances (Username, Balance, CurrencySymbol) VALUES ('BusinessGuy3', 751300000, '$');
GO
INSERT INTO BetaBankBalances (Username, Balance, CurrencySymbol) VALUES ('The_British_Are_Banking', 982124089.78, '£');
GO

CREATE TABLE OffshoreAccounts (
    [Username] [varchar](255) NOT NULL,
    [Island] [varchar](255) NOT NULL,
    [Amount] [money] NOT NULL,
    [CurrencySymbol] [varchar](1) NOT NULL
);
GO

INSERT INTO OffshoreAccounts (Username, Island, Amount, CurrencySymbol) VALUES ('The_British_Are_Banking', 'Tuvalu', 6700000.00, '£');
GO
INSERT INTO OffshoreAccounts (Username, Island, Amount, CurrencySymbol) VALUES ('The_Chairman', 'Île Saint-Paul', 254000800.00, '$');
GO

CREATE TABLE Coordinates (
    [Location] [geography] NOT NULL
);
GO

INSERT INTO Coordinates SELECT geography::STGeomFromText('Point(82.837285 -71.721439)',4326);
GO
INSERT INTO Coordinates SELECT geography::STGeomFromText('Point(66.543597 25.847240)',4326);
GO
INSERT INTO Coordinates SELECT geography::STGeomFromText('Point(51.388751 30.098574)',4326);
GO
INSERT INTO Coordinates SELECT geography::STGeomFromText('Point(29.979058 31.134170)',4326);
GO

CREATE PROCEDURE CreateSession (
     @Username varchar (255),
     @id uniqueidentifier OUTPUT
   )
AS
BEGIN  
SET nocount ON;

    SET @id = NEWID()

    IF EXISTS (SELECT * FROM Sessions WHERE Username = @Username)
    BEGIN
        DELETE FROM Sessions WHERE Username = @Username
    END
    BEGIN
        INSERT INTO Sessions (Username, ID) VALUES (@Username, @id);
    END
END
GO

CREATE PROCEDURE Login (
    @Username varchar (255),
	@Password varchar (255),
	@guid uniqueidentifier OUTPUT
   )
AS
BEGIN  
SET nocount ON;

    DECLARE @pass varchar (255)
    SELECT @pass = EncryptedPassword FROM Users WHERE Username = @Username

    IF @pass is NULL
        RAISERROR('Invalid username or password',14,1)
    ELSE
    	IF @pass = @Password
    		EXEC CreateSession @Username, @guid output;
    	ELSE
    		RAISERROR('Invalid username or password',14,1)
END
GO

CREATE PROCEDURE DestroySession (
	@ID [uniqueidentifier]
)
AS
BEGIN  
SET nocount ON;

    IF EXISTS (SELECT * FROM Sessions WHERE ID = @ID)
    BEGIN
        DELETE FROM Sessions WHERE ID = @ID
    END
END
GO

CREATE PROCEDURE IsSessionActive (
	@ID [uniqueidentifier]
)
AS
BEGIN  
SET nocount ON;
    IF EXISTS (SELECT * FROM Sessions WHERE ID = @ID)
    	RETURN 1
    ELSE
    	RETURN 0
END
GO

CREATE PROCEDURE CreateUser (
	@LastName varchar (255),
	@FirstName varchar (255),
	@Username varchar (255),
	@Password varchar (255)
)
AS
BEGIN
SET nocount ON;
	IF ((SELECT COUNT(*)
	FROM Users
	WHERE Username = @Username) > 0)
		RAISERROR('Username is already registered',14,1)
	ELSE
		BEGIN
			INSERT INTO Users (LastName, FirstName, Username, EncryptedPassword, Role) VALUES (@LastName,@FirstName,@Username,@Password,1);
			INSERT INTO BetaBankBalances (Username, Balance, CurrencySymbol) VALUES (@Username, 0.00, '$');
		END
END
GO

CREATE PROCEDURE GetUsername (
	@ID [uniqueidentifier],
	@username VARCHAR(MAX) OUTPUT
)
AS
BEGIN
SET nocount ON;

	SELECT @username = Username FROM Sessions WHERE ID = @ID
END
GO

CREATE PROCEDURE GetBalance (
	@ID [uniqueidentifier],
	@totalbalance VARCHAR(MAX) OUTPUT
)
AS
BEGIN
SET nocount ON;
	BEGIN
		DECLARE @bool int; 
		EXEC @bool = IsSessionActive @ID IF @bool = 1
			BEGIN
				DECLARE @username varchar(255);
				EXEC GetUsername @ID, @username output;
				DECLARE @balance money;
				SELECT @balance = Balance FROM BetaBankBalances WHERE Username = @username;
				DECLARE @symbol varchar(1);
				SELECT @symbol = CurrencySymbol FROM BetaBankBalances WHERE Username = @username;
				SELECT @totalbalance = CONCAT(@symbol, convert(varchar(254), @balance, 1));
			END

		ELSE
			RAISERROR('Session is not active',14,1)
	END
END
GO

CREATE PROCEDURE WithdrawBalance (
	@Withdrawal money,
	@ID [uniqueidentifier]
)
AS
BEGIN
SET nocount ON;
	BEGIN
		DECLARE @bool int; 
		EXEC @bool = IsSessionActive @ID IF @bool = 1
			BEGIN
				DECLARE @username varchar(255);
				EXEC GetUsername @ID, @username output;
				UPDATE BetaBankBalances
				SET Balance = Balance - @Withdrawal
				WHERE Username = @username;
				RETURN 1
			END

		ELSE
			RAISERROR('Session is not active',14,1)
	END
END
GO

CREATE PROCEDURE IsAdmin (
	@ID [uniqueidentifier]
)
AS
BEGIN  
SET nocount ON;
    DECLARE @username varchar(255);
	EXEC GetUsername @ID, @username output;
	IF ((SELECT Role FROM Users WHERE Username = @username) = 0)
    	RETURN 1
    ELSE
    	RETURN 0
END
GO

CREATE PROCEDURE GetVaultLocation (
	@ID [uniqueidentifier],
	@address VARCHAR(MAX) OUTPUT
)
AS
BEGIN
SET nocount ON;
	BEGIN
		DECLARE @boolActive int; 
		EXEC @boolActive = IsSessionActive @ID IF @boolActive = 1
			BEGIN
				DECLARE @boolAdmin int; 
				EXEC @boolAdmin = IsSessionActive @ID IF @boolAdmin = 1
					BEGIN
						SELECT @address = "800 N Washington Ave, Minneapolis, MN 55401";
					END
				ELSE
					RAISERROR('Unauthorized',14,1)
			END

		ELSE
			RAISERROR('Session is not active',14,1)
	END
END
GO


exec sp_configure "remote access", 0          -- 0 on, 1 off
exec sp_configure "remote query timeout", 600 -- seconds
exec sp_configure "remote proc trans", 0      -- 0 on, 1 off
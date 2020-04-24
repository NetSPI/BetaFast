CREATE DATABASE vulnerable
GO
USE [VULNERABLE]
GO 
CREATE TABLE USERS (
    [UserID] [int] NOT NULL,
    [LastName] [varchar](255) NOT NULL,
    [FirstName] [varchar](255) NOT NULL,
    [Username] [varchar](255) NOT NULL,
    [Password] [varchar](255) NOT NULL,
    [Salt] [varchar](255) NOT NULL,
    [Role] [int] NOT NULL,
    [Active] [bit] NOT NULL 
);
GO
INSERT INTO USERS (UserID, LastName, FirstName, Username, Password, Salt, Role, Active) VALUES (0,'Fast','Beta','movieguy45','nRFosQ==','aFSYzZBF9Lti3lS/fJ+ugw==',0,1);
GO
CREATE TABLE MOVIES (
    [Title] [varchar](255) NOT NULL,
    [Description] [varchar](max) NOT NULL,
    [Director] [varchar](255) NOT NULL,
    [Year] [int] NOT NULL,
    [Price] [decimal](4,2) NOT NULL,
    [PosterFile] [varchar](255) NOT NULL
);
GO
INSERT INTO MOVIES (Title, Description, Director, Year, Price, PosterFile) VALUES ('Hell-E-Vator','"666th floor, please." Career man Jake Reynolds thought he was on the way to the top, but it turned out he was on the way to the bottom - the bottom of Hell. Hailed as one of the most compelling elevator-based horror films of the decade, Hell-E-Vator will delight and thrill.','Mike Larch',1978,3.20,'hell-e-vator.png');
GO
INSERT INTO MOVIES (Title, Description, Director, Year, Price, PosterFile) VALUES ('Hockey And Hand Grenades','Everyone was playing hockey with pucks. That is, until a crate of World War II bring back grenades accidentally made its way onto the ice.','Michael Larson',1980,2.50,'hockey-and-hand-grenades.png');
GO
INSERT INTO MOVIES (Title, Description, Director, Year, Price, PosterFile) VALUES ('Lunchtime With Kevin','Grab a bite to eat with Kevin on this emotional journey into adulthood.','Liam McGrath',1976,1.99,'lunchtime-with-kevin.png');
GO
INSERT INTO MOVIES (Title, Description, Director, Year, Price, PosterFile) VALUES ('Most Valued Player','Life isn''t easy as a foosball figure. And it sure isn''t easy on the losing team.','Dalin McClellan',1978,3.20,'most-valued-player.png');
GO
INSERT INTO MOVIES (Title, Description, Director, Year, Price, PosterFile) VALUES ('That Plant Over There','"That plant over there isn''t just that plant over there. That plant over there is the love of my life." In this heartwarming love story, go where love has never gone before - a terra-cotta pot.','Mubasser Kamal',1976,3.00,'that-plant-over-there.png');
GO
INSERT INTO MOVIES (Title, Description, Director, Year, Price, PosterFile) VALUES ('The Drying of Mud','A scathing commentary on soil classification. Everything you thought you knew about sediment-based water soil mixtures was wrong!','Dr. Marcus Tennant',1977,6.99,'the-drying-of-mud.png');
GO
INSERT INTO MOVIES (Title, Description, Director, Year, Price, PosterFile) VALUES ('Water For Two','There''s room for two. But now there''s three.','Grisha Kumar',1979,8.99,'water-for-two.png');
GO
INSERT INTO MOVIES (Title, Description, Director, Year, Price, PosterFile) VALUES ('Where Do Deer Come From?','Visionary artist and local third grader Cody Wass raises the important question of Where Do Deer Come From?','Cody Wass',1981,2.65,'where-do-deer-come-from.png');
GO
INSERT INTO MOVIES (Title, Description, Director, Year, Price, PosterFile) VALUES ('Where Jackson Used To Sit','Jackson used to sit at one desk. And then he moved to a different desk. But that first desk will always be . . . where Jackson used to sit.','Sam Horvath',1980,0.99,'where-jackson-used-to-sit.png');
GO
CREATE TABLE Rentals (
	[Username] [varchar](255) NOT NULL,
    [Title] [varchar](255) NOT NULL,
    [Quantity] [int] NOT NULL,
    [Price] [decimal](4,2) NOT NULL
);
GO
CREATE TABLE Salts (
    [Username] [varchar](255) NOT NULL,
    [Salt] [varchar](255) NOT NULL
);
GO

exec sp_configure "remote access", 0          -- 0 on, 1 off
exec sp_configure "remote query timeout", 600 -- seconds
exec sp_configure "remote proc trans", 0      -- 0 on, 1 off
CREATE TABLE [Shots] (
    [Id] INT IDENTITY (1,1) NOT NULL,
    [Game_Id] INT,
    [Player] NVARCHAR (50),
    [Position] NVARCHAR (7),
    [Hit] BIT,
    CONSTRAINT [PK_Game] PRIMARY KEY ([Id])
)

CREATE TABLE [Titanics] (
    [Id] INT IDENTITY (1,1) NOT NULL,
    [Pos1] NVARCHAR (7),
    [Pos2] NVARCHAR (7),
    [Pos3] NVARCHAR (7),
    [Pos4] NVARCHAR (7),
    [Pos5] NVARCHAR (7),
    [Sunk] BIT,
    CONSTRAINT [PK_Titanics] PRIMARY KEY ([Id])
)

CREATE TABLE [LongShips] (
    [Id] INT IDENTITY (1,1) NOT NULL,
    [Pos1] NVARCHAR (7),
    [Pos2] NVARCHAR (7),
    [Pos3] NVARCHAR (7),
    [Pos4] NVARCHAR (7),
    [Sunk] BIT,
    CONSTRAINT [PK_LongShips] PRIMARY KEY ([Id])
)

CREATE TABLE [TrippleShips] (
    [Id] INT IDENTITY (1,1) NOT NULL,
    [Pos1] NVARCHAR (7),
    [Pos2] NVARCHAR (7),
    [Pos3] NVARCHAR (7),
    [Sunk] BIT,
    CONSTRAINT [PK_TrippleShips] PRIMARY KEY ([Id])
)

CREATE TABLE [DoubleShips] (
    [Id] INT IDENTITY (1,1) NOT NULL,
    [Pos1] NVARCHAR (7),
    [Pos2] NVARCHAR (7),
    [Sunk] BIT,
    CONSTRAINT [PK_DoubleShips] PRIMARY KEY ([Id])
)

CREATE TABLE [Board1] (
    [Id] INT IDENTITY (1,1) NOT NULL,
    [Game_Id] INT,
    [Titanic] INT,
    [LongShip1] INT,
    [LongShip2] INT,
    [TrippleShip1] INT,
    [TrippleShip2] INT,
    [TrippleShip3] INT,
    [DoubleShip1] INT,
    [DoubleShip2] INT,
    [DoubleShip3] INT,
    [DoubleShip4] INT,
    CONSTRAINT [PK_Board1] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Board1_Titanic] FOREIGN KEY ([Titanic]) REFERENCES [Titanics] ([Id]),
    CONSTRAINT [FK_Board1_LongShip1] FOREIGN KEY ([LongShip1]) REFERENCES [LongShips] ([Id]),
    CONSTRAINT [FK_Board1_LongShip2] FOREIGN KEY ([LongShip2]) REFERENCES [LongShips] ([Id]),
    CONSTRAINT [FK_Board1_TrippleShip1] FOREIGN KEY ([TrippleShip1]) REFERENCES [TrippleShips] ([Id]),
    CONSTRAINT [FK_Board1_TrippleShip2] FOREIGN KEY ([TrippleShip2]) REFERENCES [TrippleShips] ([Id]),
    CONSTRAINT [FK_Board1_TrippleShip3] FOREIGN KEY ([TrippleShip3]) REFERENCES [TrippleShips] ([Id]),
    CONSTRAINT [FK_Board1_DoubleShip1] FOREIGN KEY ([DoubleShip1]) REFERENCES [DoubleShips] ([Id]),
	CONSTRAINT [FK_Board1_DoubleShip2] FOREIGN KEY ([DoubleShip2]) REFERENCES [DoubleShips] ([Id]),
    CONSTRAINT [FK_Board1_DoubleShip3] FOREIGN KEY ([DoubleShip3]) REFERENCES [DoubleShips] ([Id]),
    CONSTRAINT [FK_Board1_DoubleShip4] FOREIGN KEY ([DoubleShip4]) REFERENCES [DoubleShips] ([Id]),
)

CREATE TABLE [Board2] (
    [Id] INT IDENTITY (1,1) NOT NULL,
    [Game_Id] INT,
    [Titanic] INT,
    [LongShip1] INT,
    [LongShip2] INT,
    [TrippleShip1] INT,
    [TrippleShip2] INT,
    [TrippleShip3] INT,
    [DoubleShip1] INT,
    [DoubleShip2] INT,
    [DoubleShip3] INT,
    [DoubleShip4] INT,
    CONSTRAINT [PK_Board2] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Board2_Titanic] FOREIGN KEY ([Titanic]) REFERENCES [Titanics] ([Id]),
    CONSTRAINT [FK_Board2_LongShip1] FOREIGN KEY ([LongShip1]) REFERENCES [LongShips] ([Id]),
    CONSTRAINT [FK_Board2_LongShip2] FOREIGN KEY ([LongShip2]) REFERENCES [LongShips] ([Id]),
    CONSTRAINT [FK_Board2_TrippleShip1] FOREIGN KEY ([TrippleShip1]) REFERENCES [TrippleShips] ([Id]),
    CONSTRAINT [FK_Board2_TrippleShip2] FOREIGN KEY ([TrippleShip2]) REFERENCES [TrippleShips] ([Id]),
    CONSTRAINT [FK_Board2_TrippleShip3] FOREIGN KEY ([TrippleShip3]) REFERENCES [TrippleShips] ([Id]),
    CONSTRAINT [FK_Board2_DoubleShip1] FOREIGN KEY ([DoubleShip1]) REFERENCES [DoubleShips] ([Id]),
	CONSTRAINT [FK_Board2_DoubleShip2] FOREIGN KEY ([DoubleShip2]) REFERENCES [DoubleShips] ([Id]),
    CONSTRAINT [FK_Board2_DoubleShip3] FOREIGN KEY ([DoubleShip3]) REFERENCES [DoubleShips] ([Id]),
    CONSTRAINT [FK_Board2_DoubleShip4] FOREIGN KEY ([DoubleShip4]) REFERENCES [DoubleShips] ([Id]),
)

CREATE TABLE [Game] (
	[Id] INT IDENTITY (1,1) NOT NULL,
	[Player1] NVARCHAR (50) NOT NULL,
	[Player2] NVARCHAR (50) NOT NULL,
    [Board1] INT,
    [Board2] INT,
	[NextPlayer] INT,
	[GameFinished] BIT,
    [Winner] NVARCHAR (50),
    [Board1Ready] BIT,
    [Board2Ready] BIT,/*
	CONSTRAINT [PK_Game] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Game_Users1] FOREIGN KEY ([Player1]) REFERENCES [Users] ([Username]),
	CONSTRAINT [FK_Game_Users2] FOREIGN KEY ([Player2]) REFERENCES [Users] ([Username])*/
)
CREATE TABLE [Users] (
[Username] NVARCHAR (50) NOT NULL,
[Password] NVARCHAR (50) NOT NULL,
CONSTRAINT [Username] PRIMARY KEY ([Username])
)

INSERT INTO [Users]
([Username], [Password])
values
('TiZiZAAiT', 'ardbeg10'),
('Rasmus1337', '420blazeit'),
('rasmus', 'rasmus'),
('ivar','ivar')

/* Procedur för att skapa spel*/
CREATE PROCEDURE CreateGame @Player1 NVARCHAR (50), @Player2 NVARCHAR (50) AS
    BEGIN TRY
        BEGIN TRANSACTION GameCreation

            INSERT INTO [Game]
            (Player1, Player2, NextPlayer, GameFinished)
            values
            (@Player1, @Player2, 1, 0)

        COMMIT TRANSACTION GameCreation
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION GameCreation
    END CATCH

RETURN 0;


EXEC CreateGame @Player1 = 'TiZiZAAiT', @Player2 = 'Rasmus1337'
CREATE TABLE [dbo].[AspNetUsers]
(
    [Id]                    NVARCHAR (128)  NOT NULL,
    [Email]                 NVARCHAR (256)  NULL,
    [EmailConfirmed]        BIT             NOT NULL,
    [PasswordHash]          NVARCHAR (MAX)  NULL,
    [SecurityStamp]         NVARCHAR (MAX)  NULL,
    [PhoneNumber]           NVARCHAR (MAX)  NULL,
    [PhoneNumberConfirmed]  BIT             NOT NULL,
    [TwoFactorEnabled]      BIT             NOT NULL,
    [LockoutEndDateUtc]     DATETIME        NULL,
    [LockoutEnabled]        BIT             NOT NULL,
    [AccessFailedCount]     INT             NOT NULL,
    [UserName]              NVARCHAR (256)  NOT NULL,
	[DisplayName]		    NVARCHAR (16)   NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]([UserName] ASC);

CREATE TABLE [dbo].[Users]
(
    [ID]            INT IDENTITY (1,1)  NOT NULL,
	[CredentialsID] NVARCHAR(128)	    NOT NULL,
	[FirstName]		NVARCHAR(128)	    NOT NULL,
	[LastName]		NVARCHAR(128)	    NOT NULL,
	[DOB]			DATE			    NOT NULL,
	CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_dbo.Users_dbo.AspNetUsers_Id] FOREIGN KEY ([CredentialsID]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[Events](
	[ID]				INT IDENTITY (1,1)  NOT NULL,
	[EventName]			NVARCHAR(64)        NOT NULL,
    [IsPublic]          Bit					NOT NULL,
	[Date]				DATETIME            NOT NULL,
	[EventDescription]	NVARCHAR(256)       NOT NULL,
	[EventLocation]		NVARCHAR (MAX)      NOT NULL,
	[PlayerSlotsMin]	INT                 NOT NULL,
	[PlayerSlotsMax]	INT                 NOT NULL,
    [UnsupGames]        NVARCHAR (MAX),
	[HostID]			INT      NOT NULL,
	CONSTRAINT [PK_dbo.Events] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_dbo.Events_dbo.Users_ID] FOREIGN KEY ([HostID]) REFERENCES [dbo].[Users] ([ID]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[Games](
	[ID]			INT IDENTITY (1,1)  NOT NULL,
	[Name]			NVARCHAR (MAX)      NOT NULL,
	CONSTRAINT [PK_dbo.Games] PRIMARY KEY CLUSTERED ([ID] ASC),
);

CREATE TABLE [dbo].[EventGames](
	[ID]			INT IDENTITY (1,1)  NOT NULL,
	[EventID]		INT                 NOT NULL,
	[GameID]		INT                 NOT NULL,
	CONSTRAINT [PK_dbo.EventGames] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_dbo.EventGames_dbo.Games_ID] FOREIGN KEY ([GameID]) REFERENCES [dbo].[Games] ([ID]),
	CONSTRAINT [FK_dbo.EventGames_dbo.Events_ID] FOREIGN KEY ([EventID]) REFERENCES [dbo].[Events] ([ID])
);

CREATE TABLE [dbo].[EventPlayers](
	[ID]			INT IDENTITY (1,1)  NOT NULL,
	[PlayerID]		INT                 NOT NULL,
	[EventID]		INT                 NOT NULL,
	CONSTRAINT [PK_dbo.EventPlayers] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_dbo.EventPlayers_dbo.Events_ID] FOREIGN KEY ([EventID]) REFERENCES [dbo].[Events] ([ID]),
	CONSTRAINT [FK_dbo.EventPlayers_dbo.Users_ID] FOREIGN KEY ([PlayerID]) REFERENCES [dbo].[Users] ([ID]),
);

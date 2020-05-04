-- #######################################
-- #             Identity Tables         #
-- #######################################
-- ############# AspNetRoles #############
CREATE TABLE [dbo].[AspNetRoles]
(
    [Id]   NVARCHAR (128) NOT NULL,
    [Name] NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
    ON [dbo].[AspNetRoles]([Name] ASC);

-- ############# AspNetUsers #############
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
    CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]([UserName] ASC);

-- ############# AspNetUserClaims #############
CREATE TABLE [dbo].[AspNetUserClaims]
(
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [UserId]     NVARCHAR (128) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserClaims]([UserId] ASC);

-- ############# AspNetUserLogins #############
CREATE TABLE [dbo].[AspNetUserLogins]
(
    [LoginProvider] NVARCHAR (128) NOT NULL,
    [ProviderKey]   NVARCHAR (128) NOT NULL,
    [UserId]        NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC, [UserId] ASC),
    CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]([UserId] ASC);

-- ############# AspNetUserRoles #############
CREATE TABLE [dbo].[AspNetUserRoles]
(
    [UserId] NVARCHAR (128) NOT NULL,
    [RoleId] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]([UserId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]([RoleId] ASC);

CREATE TABLE [dbo].[Users]
(
    [ID]            NVARCHAR(128)		NOT NULL,
	[FirstName]		NVARCHAR(128)	    NOT NULL,
	[LastName]		NVARCHAR(128)	    NOT NULL,
	[DOB]			DATE			    NOT NULL,
	[DisplayName]	NVARCHAR (16)		NOT NULL,
	[Bio]			NVARCHAR(MAX)				,
	[ProfilePic]	VARBINARY(max)						,
	CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED ([ID] ASC),
	--CONSTRAINT [FK_dbo.Users_dbo.AspNetUsers_Id] FOREIGN KEY ([ID]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[Events](
	[ID]				NVARCHAR(128)	    NOT NULL,
	[EventName]			NVARCHAR(64)        NOT NULL,
    [IsPublic]          Bit					NOT NULL,
	[Date]				DATE				NOT NULL,
	[EventDescription]	NVARCHAR(MAX)       NOT NULL,
	[EventLocation]		NVARCHAR (MAX)      NOT NULL,
	[EventLat]          FLOAT,
	[EventLong]         FLOAT,
	[PlayerSlotsMin]	INT                 NOT NULL,
	[PlayerSlotsMax]	INT                 NOT NULL,
	[PlayersCount]		INT							,
    [UnsupGames]        NVARCHAR (MAX),
	[HostID]			NVARCHAR(128)		NOT NULL,
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
	[EventID]		NVARCHAR(128)       NOT NULL,
	[GameID]		INT                 NOT NULL,
	CONSTRAINT [PK_dbo.EventGames] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_dbo.EventGames_dbo.Games_ID] FOREIGN KEY ([GameID]) REFERENCES [dbo].[Games] ([ID]),
	CONSTRAINT [FK_dbo.EventGames_dbo.Events_ID] FOREIGN KEY ([EventID]) REFERENCES [dbo].[Events] ([ID])
);

CREATE TABLE [dbo].[EventPlayers](
	[ID]			INT IDENTITY (1,1)  NOT NULL,
	[PlayerID]		NVARCHAR(128)                 NOT NULL,
	[EventID]		NVARCHAR(128)                 NOT NULL,
	CONSTRAINT [PK_dbo.EventPlayers] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_dbo.EventPlayers_dbo.Events_ID] FOREIGN KEY ([EventID]) REFERENCES [dbo].[Events] ([ID]),
	CONSTRAINT [FK_dbo.EventPlayers_dbo.Users_ID] FOREIGN KEY ([PlayerID]) REFERENCES [dbo].[Users] ([ID]),
	);

    CREATE TABLE [dbo].[Reviews]
(
    [ID]   NVARCHAR (128) NOT NULL,
    [ReviewString] NVARCHAR (1000),
    [Reviewer_ID] NVARCHAR(128) NOT NULL,
    [Host_ID] NVARCHAR (128) NOT NULL

    CONSTRAINT [PK_dbo.Reviews] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_dbo.Reviews_dbo.Users_Host_ID] FOREIGN KEY ([Host_ID]) REFERENCES [dbo].[Users] ([ID]) ON DELETE CASCADE 
);

CREATE TABLE [dbo].[Relationship]
(
	[ID]			INT IDENTITY (1,1)	NOT NULL,
	[UserFirstID]	NVARCHAR(128) NOT NULL,
	[UserSecondID]	NVARCHAR(128) NOT NULL,
	[Type]			INT NOT NULL

	PRIMARY KEY (UserFirstID, UserSecondID)
	CONSTRAINT[FK_dbo.Relationship_dbo.RelationshipTypes_ID] FOREIGN KEY ([Type]) REFERENCES [dbo].[RelationshipTypes] (ID) ON DELETE CASCADE
);

CREATE TABLE [dbo].[APIEventGames](
	[ID]			INT IDENTITY (1,1)  NOT NULL,
	[EventID]		NVARCHAR(128)       NOT NULL,
	[GameID]		NVARCHAR(16)        NOT NULL,
	CONSTRAINT [PK_dbo.APIEventGames] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_dbo.APIEventGames_dbo.Events_ID] FOREIGN KEY ([EventID]) REFERENCES [dbo].[Events] ([ID])
);

CREATE TABLE [dbo].[RelationshipTypes]
(
	[ID]		INT IDENTITY (1,1) NOT NULL,
	[Type]		NVARCHAR(20) NOT NULL
	CONSTRAINT [PK_dbo.RelationshipTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);


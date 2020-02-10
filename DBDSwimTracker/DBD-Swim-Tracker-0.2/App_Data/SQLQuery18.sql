CREATE TABLE [dbo].[Coaches]
(
	[ID]	INT IDENTITY (1,1)	NOT NULL,
	[NAME]	NVARCHAR (MAX)		NOT NULL,
	CONSTRAINT [PK_dbo.Coaches]	PRIMARY KEY CLUSTERED ([ID] ASC)
);

CREATE TABLE [dbo].[Teams]
(
	[ID]		INT IDENTITY (1,1)	NOT NULL,
	[NAME]		NVARCHAR (MAX)		NOT NULL,
	[COACHID]	INT					NOT NULL
	CONSTRAINT [PK_dbo.Teams] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_dbo.Teams_dbo.Coaches_ID] FOREIGN KEY ([COACHID]) REFERENCES [dbo].[Coaches] ([ID])
);

CREATE TABLE [dbo].[Athletes]
(
	[ID]		INT IDENTITY (1,1)	NOT NULL,
	[NAME]		NVARCHAR (MAX)		NOT NULL,
	[GENDER]	NVARCHAR (MAX)		NOT NULL,
	[AGE]		INT					NOT NULL
	CONSTRAINT [PK_dbo.Athletes] PRIMARY KEY CLUSTERED ([ID] ASC)
);

CREATE TABLE [dbo].[TeamRosters]
(
	[ID]		INT IDENTITY (1,1)	NOT NULL,
	[TEAMID]	INT					NOT NULL,
	[ATHLETEID]	INT					NOT NULL
	CONSTRAINT [PK_dbo.TeamRosters] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_dbo.TeamRosters_dbo.Teams_ID] FOREIGN KEY ([TEAMID]) REFERENCES [dbo].[Teams] ([ID]),
	CONSTRAINT [FK_dbo.TeamRosters_dbo.Athletes_ID] FOREIGN KEY ([ATHLETEID]) REFERENCES [dbo].[Athletes] ([ID])
);

CREATE TABLE [dbo].[Meets] 
(
	[ID]		INT IDENTITY (1,1)	NOT NULL,
	[LOCATION]	NVARCHAR (MAX)		NOT NULL,
	[DATE]		DATETIME			NOT NULL
	CONSTRAINT [PK_dbo.Meets] PRIMARY KEY CLUSTERED ([ID] ASC)
);

CREATE TABLE [dbo].[TeamMeets]
(
	[ID]		INT IDENTITY (1,1)	NOT NULL,
	[TEAMID]	INT					NOT NULL,
	[MEETID]	INT					NOT NULL
	CONSTRAINT [PK_dbo.TeamMeets] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_dbo.TeamMeets_dbo.Teams_ID] FOREIGN KEY ([TEAMID]) REFERENCES [dbo].[Teams] ([ID]),
	CONSTRAINT [FK_dbo.TeamMeets_dbo.Meets_ID] FOREIGN KEY ([MEETID]) REFERENCES [dbo].[Meets] ([ID])
);

CREATE TABLE [dbo].[Events]
(
	[ID]		INT IDENTITY (1,1)	NOT NULL,
	[DISTANCE]	INT					NOT NULL,
	[STROKE]	NVARCHAR (MAX)		NOT NULL,
	[AGEGROUP]	NVARCHAR (MAX)		,
	[GENDER]	NVARCHAR (MAX)		NOT NULL,
	[MEETID]	INT					NOT NULL
	CONSTRAINT [PK_dbo.Events] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_dbo.Events_dbo.Meets_ID] FOREIGN KEY ([MEETID]) REFERENCES [dbo].[Meets] ([ID])
);

CREATE TABLE [dbo].[Results]
(
	[ID]		INT IDENTITY (1,1)	NOT NULL,
	[EVENTID]	INT					NOT NULL,
	[ATHLETEID]	INT					NOT NULL,
	[MEETID]	INT					NOT NULL,
	[TIME]		FLOAT				NOT NULL
	CONSTRAINT [PK_dbo.Results] PRIMARY KEY CLUSTERED ([ID] ASC),
	CONSTRAINT [FK_dbo.Results_dbo.Events_ID] FOREIGN KEY ([EVENTID]) REFERENCES [dbo].[Events] ([ID]),
	CONSTRAINT [FK_dbo.Results_dbo.Athletes_ID] FOREIGN KEY ([ATHLETEID]) REFERENCES [dbo].[Athletes] ([ID]),
	CONSTRAINT [FK_dbo.Results_dbo.Meet_ID] FOREIGN KEY ([MEETID]) REFERENCES [dbo].[Meets] ([ID])
);

-- ################### SEED DATA ######################
INSERT INTO [dbo].[Coaches] (NAME)
	VALUES('Aqua Man'),
			('Fredrickson'),
			('BEN FRANKLIN'),
			('Walker')


INSERT INTO [dbo].[Athletes](NAME, GENDER, AGE)
	VALUES('Aqua Kid', 'Male', '15'),
			('Kile', 'Male', '16'),
			('Chuck', 'Male', '15'),
			('Tami', 'Female', '17'),
			('Becka', 'Female', '17'),
			('Nick', 'Male', '16'),
			('Scott', 'Male', '14'),
			('Ron Burgandy','Male','18'),
			('Tina Fuchs','Female','14')

INSERT INTO [dbo].[Meets] (LOCATION, DATE)
	VALUES('Salem', '1/24/2019'),
			('Boring', '2/23/2019'),
			('Portland', '3/05/2019'),
			('Monmouth', '4/10/2019'),
			('Sandy', '5/26/2020'),
			('Grants Pass', '7/03/2019'),
			('Silverton', '10/10/19'),
			('Wilsonville', '1/24/2020')

INSERT INTO [dbo].[Teams] (NAME, COACHID)
	VALUES('Aqua Mans Swim Legends', '1'),
			('Team Swim', '2'),
			('Swimtacular', '3'),
			('Walkers Texas Rangers', '4')


INSERT INTO [dbo].[TeamRosters](TEAMID, ATHLETEID)
	VALUES  ('1', '1'),
			('2', '2'),
			('3','3'),
			('1','4'),
			('2','5'),
			('3','6'),
			('1','7'),
			('2','8'),
			('3','9')

INSERT INTO [dbo].[TeamMeets](TEAMID, MEETID)
	VALUES('1', '1'),
			('2', '2'),
			('3', '3'),
			('4','4'),
			('1','5'),
			('2','6'),
			('3','7'),
			('4','8'),
			('1','1'),
			('2','2'),
			('3','3'),
			('4','4')

INSERT INTO [dbo].[Events](DISTANCE, STROKE, AGEGROUP, GENDER, MEETID)
	VALUES('30', 'Back', '13-14','Men','1'),
			('25', 'Butterfly', '14-15','Female','2'),
			('50', 'Back', 'ALL','Men','3'),
			('30', 'Freestyle', '15-18','Female','4'),
			('40', 'Freestyle', '15-18','Mixed','5'),
			('35', 'Butterfly', '13-14','Men','6'),
			('30', 'Back', 'ALL','Men','7'),
			('30', 'Back', 'ALL','Men','8')

INSERT INTO [dbo].[Results](EVENTID, ATHLETEID, MEETID, TIME)
	VALUES('1', '1', '1', '15.5'),
			('2', '2','2', '10.4'),
			('3', '3','3', '24.2'),
			('4', '4', '4','18.3'),
			('5', '5','5', '13.4'),
			('6', '6', '6','17.3'),
			('7','7','7','15.6'),
			('8','1','8','20.5')


DROP TABLE [dbo].[Results] ;

DROP TABLE [dbo].[Events] ;

DROP TABLE [dbo].[TeamMeets] ;

DROP TABLE [dbo].[Meets] ;

DROP TABLE [dbo].[TeamRosters] ;

DROP TABLE [dbo].[Teams] ;

DROP TABLE [dbo].[Athletes] ;

DROP TABLE [dbo].[Coaches] ;
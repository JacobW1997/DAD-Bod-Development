-- ################### SEED DATA ######################
INSERT INTO [dbo].[Coaches] (NAME)
	VALUES('Aqua Man'),
			('Fredrickson'),
			('BEN FRANKLIN')


INSERT INTO [dbo].[Athletes](NAME, GENDER, AGE)
	VALUES('Aqua Son', 'Fish', '154'),
			('Fredrickson Jr II', 'Male', '87'),
			('Walker', 'Male', '97'),
			('Tami', 'Female', '87'),
			('Morgan', 'Male', '104'),
			('Nick', 'Male', '12')

INSERT INTO [dbo].[Meets] (LOCATION, DATE)
	VALUES('Salem', '1/24/2020'),
			('Boring', '1/24/2020'),
			('Portland', '1/24/2020'),
			('Monmouth', '1/24/2020'),
			('Sandy', '1/24/2020')

INSERT INTO [dbo].[Teams] (NAME, COACHID)
	VALUES('Aqua Mans Swim Legends', '1'),
			('Team Swim', '2'),
			('Swimtacular', '3')


INSERT INTO [dbo].[TeamRosters](TEAMID, ATHLETEID)
	VALUES  ('1', '1'),
			('2', '2'),
			('3','3'),
			('1','1'),
			('2','2'),
			('3','3')

INSERT INTO [dbo].[TeamMeets](TEAMID, MEETID)
	VALUES('1', '1'),
			('2', '2'),
			('3', '3'),
			('1','2'),
			('2','1'),
			('3','3')

INSERT INTO [dbo].[Events](DISTANCE, STROKE, AGEGROUP, GENDER, MEETID)
	VALUES('20', 'back stroke', '100-200','Men','1'),
			('25', 'butterfly stroke', '10-20','Men','2'),
			('26', 'back stroke', '10-20','Men','3'),
			('30', 'freestyle', '15-200','Men','1'),
			('40', 'freestyle relay', '20-100','Men','2'),
			('60', 'butterfly stroke', '10-12','Men','3'),
			('10', 'back stroke', '15-20','Men','1')

INSERT INTO [dbo].[Results](EVENTID, ATHLETEID, TIME)
	VALUES('2', '1', '20.5'),
			('3', '2', '103.4'),
			('4', '3', '24.2'),
			('2', '4', '85.3'),
			('3', '5', '10.4'),
			('4', '6', '27.3')

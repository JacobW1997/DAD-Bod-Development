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
	VALUES('6', '1', '1', '15.5'),
			('7', '2','2', '10.4'),
			('8', '3','3', '24.2'),
			('9', '4', '4','18.3'),
			('10', '5','5', '13.4'),
			('11', '6', '6','17.3'),
			('12','7','7','15.6'),
			('13','1','8','20.5')
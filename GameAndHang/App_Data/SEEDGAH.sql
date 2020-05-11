INSERT INTO dbo.AspNetUsers(Id, Email, EmailConfirmed,PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, UserName, DisplayName)
VALUES ('1414b1ae-2edc-4cab-bae5-5b7244ebc549' ,'aquakid@gmail.com' , 0 , 'AIN1qJFDaQML9VdKNWMempZtaQ4o/uMqyo24L8IAH8TEyIUxNlVIkJRu/u+Vj/0Pow==' , 'cca15767-6edf-4059-8556-2338437aba0b' , NULL , 0 , 0 , 1 , 0 , 'aquakid@gmail.com', 'Jacob'),
		 ('b500adf6-8dba-4ca0-8ae1-8e4f8835bb55' ,'nick@gmail.com' , 0 , 'AMulXW+XE/g5PLwGgGfb5O91ESSSUv5wmhuwe06wplRaOfqf6JJpSC9J5ksgGaM9iw==' , 'cca15767-6edf-4059-8556-2338437aba0b' , NULL , 0 , 0 , 1 , 0 , 'nick@gmail.com', 'Nick');

INSERT INTO [dbo].[Games] (Name) VALUES
  ('Dungeons & Dragons'),
  ('Magic: The Gathering'),
  ('Warhammer 40,000'),
  ('Settlers of Catan');

INSERT INTO [dbo].[Users] (CredentialsID, FirstName, LastName, DOB) VALUES
    ('1414b1ae-2edc-4cab-bae5-5b7244ebc549', 'Jacob', 'Slappy', '1995-01-01'),
    ('b500adf6-8dba-4ca0-8ae1-8e4f8835bb55', 'Nick', 'Herman', '1989-03-23');

INSERT INTO [dbo].[Events] (ID, EventName, IsPublic, Date, EventDescription, EventLocation, EventLat, EventLong, PlayerSlotsMin, PlayerSlotsMax, UnsupGames, HostID) VALUES
    ('aHCzTfLcPEqP9E7Shyng','The Hangout', '1', '12/05/2019 09:12:45', 'A chill event', 'WOU student center', 44.852370, -123.238240, 2, 4, '', '38e9c339-d57f-4ca7-a428-ccb2cb723717');


INSERT INTO [dbo].[EventPlayers] (PlayerID, EventID) VALUES
  (2, 1),
  (1, 2);

INSERT INTO [dbo].[EventGames] (EventID, GameID) VALUES
  (1, 2),
  (1, 4),
  (2, 1);

INSERT INTO dbo.RelationshipTypes VALUES 
('Friends'),
('PendingFirstUser'),
('PendingSecondUser'),
('BlockFirstUser'),
('BlockSecondUser'),
('BlockBoth'),
('Rejected');

INSERT INTO dbo.Relationship(UserFirstID, UserSecondID, Type)
VALUES ('38e9c339-d57f-4ca7-a428-ccb2cb723717', '43c7ee9d-6e67-4c9a-89ed-8e7da97286e0', 1),
('43c7ee9d-6e67-4c9a-89ed-8e7da97286e0','4bc34da2-9848-4ae4-aa72-f5e4542d4b13' , 2);
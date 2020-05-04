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

INSERT INTO [dbo].[Events] (EventName, IsPublic, Date, EventDescription, EventLocation, PlayerSlotsMin, PlayerSlotsMax, UnsupGames, HostID) VALUES
    ('The Hangout', '1', '12/05/2019 09:12:45', 'A chill event', 'WOU student center', 2, 4, '', 1),
    ('DnD Mega Event', '1', '12/05/2019 09:12:45', 'Come hang out at Joes and game', 'Joes Coffee Shop', 2, 6, '', 2);

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
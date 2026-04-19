USE MyDb;
GO

TRUNCATE TABLE dbo.Payments;
TRUNCATE TABLE dbo.Cards;
GO

DECLARE @CardId UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Cards (Id, CardNumber, ExpiryMonth, ExpiryYear, Cvv)
VALUES (@CardId, '************8871', 12, 2028, '666');

INSERT INTO dbo.Payments (Id, Status, Currency, Amount, AuthorizationCode, CardId)
VALUES
    (NEWID(), 0, 'GBP', 30000, '0bb07405-6d44-4b50-a14f-7ae0beff13ad', @CardId),
    (NEWID(), 1, 'GBP', 30000, NULL, @CardId);
GO
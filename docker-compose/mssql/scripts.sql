IF DB_ID(N'MyDb') IS NULL
BEGIN
    CREATE DATABASE MyDb;
END
GO

USE MyDb;
GO

IF OBJECT_ID(N'dbo.Cards', N'U') IS NULL
BEGIN
CREATE TABLE dbo.Cards
(
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Cards PRIMARY KEY
        CONSTRAINT DF_Cards_Id DEFAULT NEWID(),
    CardNumber VARCHAR(19) NOT NULL,
    ExpiryMonth TINYINT NOT NULL,
    ExpiryYear SMALLINT NOT NULL,
    Cvv VARCHAR(3) NULL
);
END
GO

IF OBJECT_ID(N'dbo.Payments', N'U') IS NULL
BEGIN
CREATE TABLE dbo.Payments
(
    Id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT PK_Payments PRIMARY KEY
        CONSTRAINT DF_Payments_Id DEFAULT NEWID(),
    Status TINYINT NOT NULL,
    Currency VARCHAR(3) NOT NULL,
    Amount INT NOT NULL,
    AuthorizationCode VARCHAR(36) NULL,
    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Payments_CreatedAt DEFAULT GETUTCDATE(),
    CardId UNIQUEIDENTIFIER NOT NULL
);
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = N'FK_Payments_Cards'
      AND parent_object_id = OBJECT_ID(N'dbo.Payments')
)
BEGIN
ALTER TABLE dbo.Payments
    ADD CONSTRAINT FK_Payments_Cards
        FOREIGN KEY (CardId) REFERENCES dbo.Cards(Id);
END
GO
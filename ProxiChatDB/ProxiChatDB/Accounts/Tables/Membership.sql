CREATE TABLE [Accounts].[Membership] (
    [Id]         INT           NOT NULL,
    [CreatedUTC] DATETIME2 (7) NOT NULL,
    [Password]   BINARY (32)   NOT NULL,
    [Salt]       BINARY (8)    NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


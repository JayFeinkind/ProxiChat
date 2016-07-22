CREATE TABLE [Accounts].[DeviceTokens] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [CreatedUTC]    DATETIME2 (7)  NOT NULL,
    [ModifiedUTC]   DATETIME2 (7)  NULL,
    [Version]       ROWVERSION     NOT NULL,
    [VersionNumber] AS             (isnull(CONVERT([bigint],[Version]),(0))) PERSISTED NOT NULL,
    [UserId]        INT            NOT NULL,
    [Token]         NVARCHAR (100) NOT NULL,
    [DeviceId]      INT            NOT NULL,
    CONSTRAINT [PK__DeviceTokens] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DeviceId_Devices] FOREIGN KEY ([DeviceId]) REFERENCES [Accounts].[Devices] ([Id]),
    CONSTRAINT [FK_DeviceTokens_Users] FOREIGN KEY ([UserId]) REFERENCES [Accounts].[Users] ([Id]),
    CONSTRAINT [IX_DeviceId] UNIQUE NONCLUSTERED ([DeviceId] ASC)
);






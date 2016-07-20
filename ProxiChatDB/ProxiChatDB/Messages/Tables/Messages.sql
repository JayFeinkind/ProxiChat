CREATE TABLE [Messages].[Messages] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [CreatedUTC]      DATETIME2 (7)  NOT NULL,
    [SentUTC]         DATETIME2 (7)  NOT NULL,
    [Version]         ROWVERSION     NOT NULL,
    [VersionNumber]   AS             (isnull(CONVERT([bigint],[Version]),(0))) PERSISTED NOT NULL,
    [Message]         NVARCHAR (200) NOT NULL,
    [SenderId]        INT            NOT NULL,
    [ReceiverId]      INT            NOT NULL,
    [SenderDeleted]   BIT            DEFAULT ((0)) NOT NULL,
    [ReceiverDeleted] BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK__Messages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK__Messages__ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [Accounts].[Users] ([Id]),
    CONSTRAINT [FK__Messages__SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Accounts].[Users] ([Id])
);


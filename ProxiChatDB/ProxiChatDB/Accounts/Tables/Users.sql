CREATE TABLE [Accounts].[Users] (
    [Id]                 INT              IDENTITY (1, 1) NOT NULL,
    [CreatedUTC]         DATETIME2 (7)    CONSTRAINT [DF__Users__CreatedUTC] DEFAULT (sysutcdatetime()) NOT NULL,
    [ModifiedUTC]        DATETIME2 (7)    NULL,
    [Version]            ROWVERSION       NOT NULL,
    [VersionNumber]      AS               (isnull(CONVERT([bigint],[Version]),(0))) PERSISTED NOT NULL,
    [FirstName]          NVARCHAR (50)    NOT NULL,
    [LastName]           NVARCHAR (50)    NOT NULL,
    [UserName]           NVARCHAR (50)    NULL,
    [EmailAddress]       NVARCHAR (100)   NULL,
    [ResetPasswordToken] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);




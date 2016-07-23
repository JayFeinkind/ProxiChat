CREATE TABLE [Devices].[Devices] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [CreatedUTC]       DATETIME2 (7)  NOT NULL,
    [ModifiedUTC]      DATETIME2 (7)  NULL,
    [Version]          ROWVERSION     NOT NULL,
    [VersionNumber]    AS             (isnull(CONVERT([bigint],[Version]),(0))) PERSISTED NOT NULL,
    [DeviceIdentifier] NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK__Devices__Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


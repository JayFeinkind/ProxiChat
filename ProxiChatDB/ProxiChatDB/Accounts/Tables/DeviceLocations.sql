CREATE TABLE [Accounts].[DeviceLocations] (
    [Id]            INT            NOT NULL,
    [CreatedUTC]    DATETIME2 (7)  NOT NULL,
    [ModifiedUtc]   DATETIME2 (7)  NULL,
    [Version]       ROWVERSION     NOT NULL,
    [VersionNumber] AS             (isnull(CONVERT([bigint],[Version]),(0))) PERSISTED NOT NULL,
    [Latitude]      DECIMAL (8, 5) NOT NULL,
    [Longitude]     DECIMAL (8, 5) NOT NULL,
    [DeviceId]      INT            NOT NULL,
    FOREIGN KEY ([DeviceId]) REFERENCES [Accounts].[Devices] ([Id])
);


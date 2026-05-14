PRINT N'Altering [dbo].[BvDialers]...';


GO
ALTER TABLE [dbo].[BvDialers]
    ADD [DialerInterfaceVersion] NVARCHAR (255) CONSTRAINT [DF_BvDialers_DialerInterfaceVersion] DEFAULT ('') NOT NULL,
        [DialerDriver]           NVARCHAR (255) CONSTRAINT [DF_BvDialers_DialerDriver] DEFAULT ('') NOT NULL,
        [DialerDriverVersion]    NVARCHAR (255) CONSTRAINT [DF_BvDialers_DialerDriverVersion] DEFAULT ('') NOT NULL;


GO
PRINT N'Update complete.';
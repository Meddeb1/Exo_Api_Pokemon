CREATE TABLE [dbo].[Pokemon]
(
	[Id] INT NOT NULL IDENTITY, 
    [NomFr] NVARCHAR(128) NULL, 
    [NomEn] NCHAR(128) NULL, 
    CONSTRAINT [PK_Pokemon] PRIMARY KEY ([Id]) 
)

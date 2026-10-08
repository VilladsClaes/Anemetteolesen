-- Database-schema til anemetteolesen.dk
-- Genereret ud fra Models/AnemetteModel.edmx. Koeres mod en TOM database (fx i SSMS).
-- Scriptet springer tabeller over, som allerede findes, saa det kan koeres flere gange.
SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.tblAdmin', N'U') IS NULL
CREATE TABLE [dbo].[tblAdmin] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Brugernavn] nvarchar(150) NULL,
    [Password] nvarchar(150) NULL,
    [Salt] nvarchar(150) NULL,
    [BrugerEmail] nvarchar(150) NULL,
    CONSTRAINT [PK_tblAdmin] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblArtikel', N'U') IS NULL
CREATE TABLE [dbo].[tblArtikel] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ArtikelOverskrift] nvarchar(150) NULL,
    [ArtikelTekst] nvarchar(max) NULL,
    [ArtikelBannerBillede] nvarchar(150) NULL,
    [ArtikelManchetTekst] nvarchar(150) NULL,
    CONSTRAINT [PK_tblArtikel] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblBestyrelse', N'U') IS NULL
CREATE TABLE [dbo].[tblBestyrelse] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [MedlemsBillede] nvarchar(150) NULL,
    [MedlemsNavn] nvarchar(150) NULL,
    [MedlemsTitel] nvarchar(150) NULL,
    [MedlemsBeskrivelse] nvarchar(550) NULL,
    [MedlemsMail] nvarchar(150) NULL,
    CONSTRAINT [PK_tblBestyrelse] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblBillede', N'U') IS NULL
CREATE TABLE [dbo].[tblBillede] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [BilledeFil] nvarchar(850) NULL,
    [BilledeAlternativTekst] nvarchar(250) NULL,
    [BilledeBeskrivelse] nvarchar(350) NULL,
    [BilledeOverskrift] nvarchar(250) NULL,
    [FK_Type] int NULL,
    [FK_Artikel] int NULL,
    [FK_Produkt] int NULL,
    [FK_Event] int NULL,
    [FK_Bestyrelse] int NULL,
    [FK_Person] int NULL,
    [FK_Sponsor] int NULL,
    [FK_Side] int NULL,
    CONSTRAINT [PK_tblBillede] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblBillet', N'U') IS NULL
CREATE TABLE [dbo].[tblBillet] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [BilletType] nvarchar(250) NULL,
    [BilletBeskrivelse] nvarchar(550) NULL,
    [BilletAntalGlas] int NULL,
    [BilletPris] int NULL,
    [BilletLink] nvarchar(250) NULL,
    CONSTRAINT [PK_tblBillet] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblEvent', N'U') IS NULL
CREATE TABLE [dbo].[tblEvent] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [EventOverskrift] nvarchar(150) NULL,
    [EventDato] datetime NULL,
    [EventBeskrivelse] nvarchar(750) NULL,
    [EventPris] int NULL,
    [EventPladser] int NULL,
    [EventDistance] int NULL,
    [FK_Region] int NULL,
    [FK_Sponsor] int NULL,
    [FK_Type] int NULL,
    CONSTRAINT [PK_tblEvent] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblEventRegion', N'U') IS NULL
CREATE TABLE [dbo].[tblEventRegion] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Region] nvarchar(50) NULL,
    CONSTRAINT [PK_tblEventRegion] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblNyhedsbrev', N'U') IS NULL
CREATE TABLE [dbo].[tblNyhedsbrev] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_Person] int NULL,
    [Emailadresse] nvarchar(250) NULL,
    CONSTRAINT [PK_tblNyhedsbrev] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblPerson', N'U') IS NULL
CREATE TABLE [dbo].[tblPerson] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [NavnTilPerson] nvarchar(250) NULL,
    [TelefonnummerTilPerson] nvarchar(50) NULL,
    [EmailTilPerson] nvarchar(50) NULL,
    [BilledeAfPerson] nvarchar(150) NULL,
    [RSVP] nvarchar(50) NULL,
    [SamtykkeTilDenneListe] nvarchar(50) NULL,
    [FK_HvorKenderViPersonenFra] int NULL,
    [MedlemsskabAfForetagsomheden] nvarchar(110) NULL,
    [BidragTilAuktionen] nvarchar(550) NULL,
    [BidragTilSneglebingo] nvarchar(550) NULL,
    [BidragTilForberedelse] nvarchar(550) NULL,
    [FK_TjansUnderEvent] int NULL,
    [Salt] nvarchar(550) NULL,
    [HashetLink] nvarchar(550) NULL,
    [FK_BilletType] int NULL,
    [AlderAfPerson] datetime NULL,
    [LoebeDistanceForPerson] int NULL,
    [FK_Region_Hjemstavn] int NULL,
    [AdresseTilPerson] nvarchar(550) NULL,
    [AntalGangePersonenHarBesogtSiden] int NULL,
    CONSTRAINT [PK_tblPerson] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblProdukt', N'U') IS NULL
CREATE TABLE [dbo].[tblProdukt] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Navn] nvarchar(150) NULL,
    [Beskrivelse] nvarchar(250) NULL,
    [Pris] int NULL,
    [Antal] int NULL,
    [FK_ProductCategoryID] int NULL,
    CONSTRAINT [PK_tblProdukt] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblProduktKategori', N'U') IS NULL
CREATE TABLE [dbo].[tblProduktKategori] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Kategori] nvarchar(150) NULL,
    CONSTRAINT [PK_tblProduktKategori] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblSide', N'U') IS NULL
CREATE TABLE [dbo].[tblSide] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [SideTitel] nvarchar(150) NULL,
    [SideBeskrivelse] nvarchar(450) NULL,
    [SideOprettelsesDato] date NULL,
    [SideLink] nvarchar(150) NULL,
    [SideBillede] nvarchar(150) NULL,
    [FK_Type] int NULL,
    CONSTRAINT [PK_tblSide] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblSpm', N'U') IS NULL
CREATE TABLE [dbo].[tblSpm] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Spm] nvarchar(500) NULL,
    [FK_Type] int NULL,
    CONSTRAINT [PK_tblSpm] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblSponsor', N'U') IS NULL
CREATE TABLE [dbo].[tblSponsor] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [SponsorNavn] nvarchar(150) NULL,
    [SponsorLogo] nvarchar(150) NULL,
    [FK_SponsorType] int NULL,
    [SponsorLogoAlternativtekst] nvarchar(150) NULL,
    CONSTRAINT [PK_tblSponsor] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblSponsorType', N'U') IS NULL
CREATE TABLE [dbo].[tblSponsorType] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [SponsorType] nvarchar(150) NULL,
    CONSTRAINT [PK_tblSponsorType] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblSted', N'U') IS NULL
CREATE TABLE [dbo].[tblSted] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [HvorKenderViPersonenFra] nvarchar(250) NULL,
    CONSTRAINT [PK_tblSted] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblSvar', N'U') IS NULL
CREATE TABLE [dbo].[tblSvar] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [FK_Person] int NULL,
    [FK_Spm] int NULL,
    [Svar] nvarchar(2500) NULL,
    CONSTRAINT [PK_tblSvar] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblTag', N'U') IS NULL
CREATE TABLE [dbo].[tblTag] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Tag] nvarchar(50) NULL,
    [FK_Side] int NULL,
    CONSTRAINT [PK_tblTag] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblTilmelding', N'U') IS NULL
CREATE TABLE [dbo].[tblTilmelding] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [TilmeldteEmail] nvarchar(250) NULL,
    [FK_Event] int NULL,
    [FK_Person] int NULL,
    CONSTRAINT [PK_tblTilmelding] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblTjans', N'U') IS NULL
CREATE TABLE [dbo].[tblTjans] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [TjansUnderEvent] nvarchar(210) NULL,
    CONSTRAINT [PK_tblTjans] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblType', N'U') IS NULL
CREATE TABLE [dbo].[tblType] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Type] nvarchar(50) NULL,
    CONSTRAINT [PK_tblType] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.tblValg', N'U') IS NULL
CREATE TABLE [dbo].[tblValg] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Valg] nvarchar(150) NULL,
    [FK_Spm] int NULL,
    [FK_Svar_DetKorrekteSvar] int NULL,
    CONSTRAINT [PK_tblValg] PRIMARY KEY CLUSTERED ([ID])
);
GO

IF OBJECT_ID(N'dbo.FK_Product_Category', N'F') IS NULL
ALTER TABLE [dbo].[tblProdukt] ADD CONSTRAINT [FK_Product_Category] FOREIGN KEY ([FK_ProductCategoryID]) REFERENCES [dbo].[tblProduktKategori] ([ID]) ON DELETE CASCADE;
GO
IF OBJECT_ID(N'dbo.FK_tblBillede_tblArtikel', N'F') IS NULL
ALTER TABLE [dbo].[tblBillede] ADD CONSTRAINT [FK_tblBillede_tblArtikel] FOREIGN KEY ([FK_Artikel]) REFERENCES [dbo].[tblArtikel] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblBillede_tblBestyrelse', N'F') IS NULL
ALTER TABLE [dbo].[tblBillede] ADD CONSTRAINT [FK_tblBillede_tblBestyrelse] FOREIGN KEY ([FK_Bestyrelse]) REFERENCES [dbo].[tblBestyrelse] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblBillede_tblEvent', N'F') IS NULL
ALTER TABLE [dbo].[tblBillede] ADD CONSTRAINT [FK_tblBillede_tblEvent] FOREIGN KEY ([FK_Event]) REFERENCES [dbo].[tblEvent] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblBillede_tblPerson', N'F') IS NULL
ALTER TABLE [dbo].[tblBillede] ADD CONSTRAINT [FK_tblBillede_tblPerson] FOREIGN KEY ([FK_Person]) REFERENCES [dbo].[tblPerson] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblBillede_tblProdukt', N'F') IS NULL
ALTER TABLE [dbo].[tblBillede] ADD CONSTRAINT [FK_tblBillede_tblProdukt] FOREIGN KEY ([FK_Produkt]) REFERENCES [dbo].[tblProdukt] ([ID]) ON DELETE CASCADE;
GO
IF OBJECT_ID(N'dbo.FK_tblBillede_tblSide', N'F') IS NULL
ALTER TABLE [dbo].[tblBillede] ADD CONSTRAINT [FK_tblBillede_tblSide] FOREIGN KEY ([FK_Side]) REFERENCES [dbo].[tblSide] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblBillede_tblSponsor', N'F') IS NULL
ALTER TABLE [dbo].[tblBillede] ADD CONSTRAINT [FK_tblBillede_tblSponsor] FOREIGN KEY ([FK_Sponsor]) REFERENCES [dbo].[tblSponsor] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblBillede_tblType', N'F') IS NULL
ALTER TABLE [dbo].[tblBillede] ADD CONSTRAINT [FK_tblBillede_tblType] FOREIGN KEY ([FK_Type]) REFERENCES [dbo].[tblType] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblEvent_tblEventRegion', N'F') IS NULL
ALTER TABLE [dbo].[tblEvent] ADD CONSTRAINT [FK_tblEvent_tblEventRegion] FOREIGN KEY ([FK_Region]) REFERENCES [dbo].[tblEventRegion] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblEvent_tblSponsor', N'F') IS NULL
ALTER TABLE [dbo].[tblEvent] ADD CONSTRAINT [FK_tblEvent_tblSponsor] FOREIGN KEY ([FK_Sponsor]) REFERENCES [dbo].[tblSponsor] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblEvent_tblType', N'F') IS NULL
ALTER TABLE [dbo].[tblEvent] ADD CONSTRAINT [FK_tblEvent_tblType] FOREIGN KEY ([FK_Type]) REFERENCES [dbo].[tblType] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblNyhedsbrev_tblPerson', N'F') IS NULL
ALTER TABLE [dbo].[tblNyhedsbrev] ADD CONSTRAINT [FK_tblNyhedsbrev_tblPerson] FOREIGN KEY ([FK_Person]) REFERENCES [dbo].[tblPerson] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblPerson_tblBillet', N'F') IS NULL
ALTER TABLE [dbo].[tblPerson] ADD CONSTRAINT [FK_tblPerson_tblBillet] FOREIGN KEY ([FK_BilletType]) REFERENCES [dbo].[tblBillet] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblPerson_tblEventRegion', N'F') IS NULL
ALTER TABLE [dbo].[tblPerson] ADD CONSTRAINT [FK_tblPerson_tblEventRegion] FOREIGN KEY ([FK_Region_Hjemstavn]) REFERENCES [dbo].[tblEventRegion] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblPerson_tblSted', N'F') IS NULL
ALTER TABLE [dbo].[tblPerson] ADD CONSTRAINT [FK_tblPerson_tblSted] FOREIGN KEY ([FK_HvorKenderViPersonenFra]) REFERENCES [dbo].[tblSted] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblPerson_tblTjans', N'F') IS NULL
ALTER TABLE [dbo].[tblPerson] ADD CONSTRAINT [FK_tblPerson_tblTjans] FOREIGN KEY ([FK_TjansUnderEvent]) REFERENCES [dbo].[tblTjans] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblSide_tblType', N'F') IS NULL
ALTER TABLE [dbo].[tblSide] ADD CONSTRAINT [FK_tblSide_tblType] FOREIGN KEY ([FK_Type]) REFERENCES [dbo].[tblType] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblSpm_tblType', N'F') IS NULL
ALTER TABLE [dbo].[tblSpm] ADD CONSTRAINT [FK_tblSpm_tblType] FOREIGN KEY ([FK_Type]) REFERENCES [dbo].[tblType] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblSponsor_tblSponsorType', N'F') IS NULL
ALTER TABLE [dbo].[tblSponsor] ADD CONSTRAINT [FK_tblSponsor_tblSponsorType] FOREIGN KEY ([FK_SponsorType]) REFERENCES [dbo].[tblSponsorType] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblSvar_tblPerson', N'F') IS NULL
ALTER TABLE [dbo].[tblSvar] ADD CONSTRAINT [FK_tblSvar_tblPerson] FOREIGN KEY ([FK_Person]) REFERENCES [dbo].[tblPerson] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblSvar_tblSpm', N'F') IS NULL
ALTER TABLE [dbo].[tblSvar] ADD CONSTRAINT [FK_tblSvar_tblSpm] FOREIGN KEY ([FK_Spm]) REFERENCES [dbo].[tblSpm] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblTag_tblSide', N'F') IS NULL
ALTER TABLE [dbo].[tblTag] ADD CONSTRAINT [FK_tblTag_tblSide] FOREIGN KEY ([FK_Side]) REFERENCES [dbo].[tblSide] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblTilmelding_tblEvent', N'F') IS NULL
ALTER TABLE [dbo].[tblTilmelding] ADD CONSTRAINT [FK_tblTilmelding_tblEvent] FOREIGN KEY ([FK_Event]) REFERENCES [dbo].[tblEvent] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblTilmelding_tblPerson', N'F') IS NULL
ALTER TABLE [dbo].[tblTilmelding] ADD CONSTRAINT [FK_tblTilmelding_tblPerson] FOREIGN KEY ([FK_Person]) REFERENCES [dbo].[tblPerson] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblValg_tblSpm', N'F') IS NULL
ALTER TABLE [dbo].[tblValg] ADD CONSTRAINT [FK_tblValg_tblSpm] FOREIGN KEY ([FK_Spm]) REFERENCES [dbo].[tblSpm] ([ID]);
GO
IF OBJECT_ID(N'dbo.FK_tblValg_tblSvar', N'F') IS NULL
ALTER TABLE [dbo].[tblValg] ADD CONSTRAINT [FK_tblValg_tblSvar] FOREIGN KEY ([FK_Svar_DetKorrekteSvar]) REFERENCES [dbo].[tblSvar] ([ID]);
GO

-- Tekst til "Om Anemette"-boksen paa forsiden (redigeres bagefter)
IF NOT EXISTS (SELECT 1 FROM dbo.tblArtikel WHERE ArtikelOverskrift = N'Om-Anemette')
    INSERT INTO dbo.tblArtikel (ArtikelOverskrift, ArtikelTekst) VALUES (N'Om-Anemette', N'');
GO

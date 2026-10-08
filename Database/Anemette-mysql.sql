-- Database-schema til anemetteolesen.dk (MySQL / MariaDB)
-- Genereret ud fra datamodellen. Koer det i phpMyAdmin (fanen SQL) paa den nye database.
-- Koeres een gang paa en tom database (fremmednoeglerne giver fejl, hvis de allerede findes).
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS `tblAdmin` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Brugernavn` varchar(150) NULL,
  `Password` varchar(150) NULL,
  `Salt` varchar(150) NULL,
  `BrugerEmail` varchar(150) NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblArtikel` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `ArtikelOverskrift` varchar(150) NULL,
  `ArtikelTekst` longtext NULL,
  `ArtikelBannerBillede` varchar(150) NULL,
  `ArtikelManchetTekst` varchar(150) NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblBestyrelse` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `MedlemsBillede` varchar(150) NULL,
  `MedlemsNavn` varchar(150) NULL,
  `MedlemsTitel` varchar(150) NULL,
  `MedlemsBeskrivelse` varchar(550) NULL,
  `MedlemsMail` varchar(150) NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblBillede` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `BilledeFil` varchar(850) NULL,
  `BilledeAlternativTekst` varchar(250) NULL,
  `BilledeBeskrivelse` varchar(350) NULL,
  `BilledeOverskrift` varchar(250) NULL,
  `FK_Type` int NULL,
  `FK_Artikel` int NULL,
  `FK_Produkt` int NULL,
  `FK_Event` int NULL,
  `FK_Bestyrelse` int NULL,
  `FK_Person` int NULL,
  `FK_Sponsor` int NULL,
  `FK_Side` int NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblBillet` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `BilletType` varchar(250) NULL,
  `BilletBeskrivelse` varchar(550) NULL,
  `BilletAntalGlas` int NULL,
  `BilletPris` int NULL,
  `BilletLink` varchar(250) NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblEvent` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `EventOverskrift` varchar(150) NULL,
  `EventDato` datetime NULL,
  `EventBeskrivelse` varchar(750) NULL,
  `EventPris` int NULL,
  `EventPladser` int NULL,
  `EventDistance` int NULL,
  `FK_Region` int NULL,
  `FK_Sponsor` int NULL,
  `FK_Type` int NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblEventRegion` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Region` varchar(50) NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblNyhedsbrev` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `FK_Person` int NULL,
  `Emailadresse` varchar(250) NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblPerson` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `NavnTilPerson` varchar(250) NULL,
  `TelefonnummerTilPerson` varchar(50) NULL,
  `EmailTilPerson` varchar(50) NULL,
  `BilledeAfPerson` varchar(150) NULL,
  `RSVP` varchar(50) NULL,
  `SamtykkeTilDenneListe` varchar(50) NULL,
  `FK_HvorKenderViPersonenFra` int NULL,
  `MedlemsskabAfForetagsomheden` varchar(110) NULL,
  `BidragTilAuktionen` varchar(550) NULL,
  `BidragTilSneglebingo` varchar(550) NULL,
  `BidragTilForberedelse` varchar(550) NULL,
  `FK_TjansUnderEvent` int NULL,
  `Salt` varchar(550) NULL,
  `HashetLink` varchar(550) NULL,
  `FK_BilletType` int NULL,
  `AlderAfPerson` datetime NULL,
  `LoebeDistanceForPerson` int NULL,
  `FK_Region_Hjemstavn` int NULL,
  `AdresseTilPerson` varchar(550) NULL,
  `AntalGangePersonenHarBesogtSiden` int NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblProdukt` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Navn` varchar(150) NULL,
  `Beskrivelse` varchar(250) NULL,
  `Pris` int NULL,
  `Antal` int NULL,
  `FK_ProductCategoryID` int NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblProduktKategori` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Kategori` varchar(150) NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblSide` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `SideTitel` varchar(150) NULL,
  `SideBeskrivelse` varchar(450) NULL,
  `SideOprettelsesDato` date NULL,
  `SideLink` varchar(150) NULL,
  `SideBillede` varchar(150) NULL,
  `FK_Type` int NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblSpm` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Spm` varchar(500) NULL,
  `FK_Type` int NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblSponsor` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `SponsorNavn` varchar(150) NULL,
  `SponsorLogo` varchar(150) NULL,
  `FK_SponsorType` int NULL,
  `SponsorLogoAlternativtekst` varchar(150) NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblSponsorType` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `SponsorType` varchar(150) NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblSted` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `HvorKenderViPersonenFra` varchar(250) NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblSvar` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `FK_Person` int NULL,
  `FK_Spm` int NULL,
  `Svar` varchar(2500) NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblTag` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Tag` varchar(50) NULL,
  `FK_Side` int NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblTilmelding` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `TilmeldteEmail` varchar(250) NULL,
  `FK_Event` int NULL,
  `FK_Person` int NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblTjans` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `TjansUnderEvent` varchar(210) NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblType` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Type` varchar(50) NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

CREATE TABLE IF NOT EXISTS `tblValg` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Valg` varchar(150) NULL,
  `FK_Spm` int NULL,
  `FK_Svar_DetKorrekteSvar` int NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_danish_ci;

-- Fremmednoegler (relationer)
ALTER TABLE `tblProdukt` ADD CONSTRAINT `FK_Product_Category` FOREIGN KEY (`FK_ProductCategoryID`) REFERENCES `tblProduktKategori` (`ID`) ON DELETE CASCADE;
ALTER TABLE `tblBillede` ADD CONSTRAINT `FK_tblBillede_tblArtikel` FOREIGN KEY (`FK_Artikel`) REFERENCES `tblArtikel` (`ID`);
ALTER TABLE `tblBillede` ADD CONSTRAINT `FK_tblBillede_tblBestyrelse` FOREIGN KEY (`FK_Bestyrelse`) REFERENCES `tblBestyrelse` (`ID`);
ALTER TABLE `tblBillede` ADD CONSTRAINT `FK_tblBillede_tblEvent` FOREIGN KEY (`FK_Event`) REFERENCES `tblEvent` (`ID`);
ALTER TABLE `tblBillede` ADD CONSTRAINT `FK_tblBillede_tblPerson` FOREIGN KEY (`FK_Person`) REFERENCES `tblPerson` (`ID`);
ALTER TABLE `tblBillede` ADD CONSTRAINT `FK_tblBillede_tblProdukt` FOREIGN KEY (`FK_Produkt`) REFERENCES `tblProdukt` (`ID`) ON DELETE CASCADE;
ALTER TABLE `tblBillede` ADD CONSTRAINT `FK_tblBillede_tblSide` FOREIGN KEY (`FK_Side`) REFERENCES `tblSide` (`ID`);
ALTER TABLE `tblBillede` ADD CONSTRAINT `FK_tblBillede_tblSponsor` FOREIGN KEY (`FK_Sponsor`) REFERENCES `tblSponsor` (`ID`);
ALTER TABLE `tblBillede` ADD CONSTRAINT `FK_tblBillede_tblType` FOREIGN KEY (`FK_Type`) REFERENCES `tblType` (`ID`);
ALTER TABLE `tblEvent` ADD CONSTRAINT `FK_tblEvent_tblEventRegion` FOREIGN KEY (`FK_Region`) REFERENCES `tblEventRegion` (`ID`);
ALTER TABLE `tblEvent` ADD CONSTRAINT `FK_tblEvent_tblSponsor` FOREIGN KEY (`FK_Sponsor`) REFERENCES `tblSponsor` (`ID`);
ALTER TABLE `tblEvent` ADD CONSTRAINT `FK_tblEvent_tblType` FOREIGN KEY (`FK_Type`) REFERENCES `tblType` (`ID`);
ALTER TABLE `tblNyhedsbrev` ADD CONSTRAINT `FK_tblNyhedsbrev_tblPerson` FOREIGN KEY (`FK_Person`) REFERENCES `tblPerson` (`ID`);
ALTER TABLE `tblPerson` ADD CONSTRAINT `FK_tblPerson_tblBillet` FOREIGN KEY (`FK_BilletType`) REFERENCES `tblBillet` (`ID`);
ALTER TABLE `tblPerson` ADD CONSTRAINT `FK_tblPerson_tblEventRegion` FOREIGN KEY (`FK_Region_Hjemstavn`) REFERENCES `tblEventRegion` (`ID`);
ALTER TABLE `tblPerson` ADD CONSTRAINT `FK_tblPerson_tblSted` FOREIGN KEY (`FK_HvorKenderViPersonenFra`) REFERENCES `tblSted` (`ID`);
ALTER TABLE `tblPerson` ADD CONSTRAINT `FK_tblPerson_tblTjans` FOREIGN KEY (`FK_TjansUnderEvent`) REFERENCES `tblTjans` (`ID`);
ALTER TABLE `tblSide` ADD CONSTRAINT `FK_tblSide_tblType` FOREIGN KEY (`FK_Type`) REFERENCES `tblType` (`ID`);
ALTER TABLE `tblSpm` ADD CONSTRAINT `FK_tblSpm_tblType` FOREIGN KEY (`FK_Type`) REFERENCES `tblType` (`ID`);
ALTER TABLE `tblSponsor` ADD CONSTRAINT `FK_tblSponsor_tblSponsorType` FOREIGN KEY (`FK_SponsorType`) REFERENCES `tblSponsorType` (`ID`);
ALTER TABLE `tblSvar` ADD CONSTRAINT `FK_tblSvar_tblPerson` FOREIGN KEY (`FK_Person`) REFERENCES `tblPerson` (`ID`);
ALTER TABLE `tblSvar` ADD CONSTRAINT `FK_tblSvar_tblSpm` FOREIGN KEY (`FK_Spm`) REFERENCES `tblSpm` (`ID`);
ALTER TABLE `tblTag` ADD CONSTRAINT `FK_tblTag_tblSide` FOREIGN KEY (`FK_Side`) REFERENCES `tblSide` (`ID`);
ALTER TABLE `tblTilmelding` ADD CONSTRAINT `FK_tblTilmelding_tblEvent` FOREIGN KEY (`FK_Event`) REFERENCES `tblEvent` (`ID`);
ALTER TABLE `tblTilmelding` ADD CONSTRAINT `FK_tblTilmelding_tblPerson` FOREIGN KEY (`FK_Person`) REFERENCES `tblPerson` (`ID`);
ALTER TABLE `tblValg` ADD CONSTRAINT `FK_tblValg_tblSpm` FOREIGN KEY (`FK_Spm`) REFERENCES `tblSpm` (`ID`);
ALTER TABLE `tblValg` ADD CONSTRAINT `FK_tblValg_tblSvar` FOREIGN KEY (`FK_Svar_DetKorrekteSvar`) REFERENCES `tblSvar` (`ID`);

-- Tekst til "Om Anemette"-boksen paa forsiden (redigeres bagefter)
INSERT INTO `tblArtikel` (`ArtikelOverskrift`, `ArtikelTekst`)
SELECT 'Om-Anemette', '' FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM `tblArtikel` WHERE `ArtikelOverskrift` = 'Om-Anemette');

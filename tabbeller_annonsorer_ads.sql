CREATE TABLE tbl_annonsorer (
    annonsor_id INT IDENTITY(1,1) PRIMARY KEY,
    annonsor_namn NVARCHAR(100) NOT NULL,
    annonsor_telefon NVARCHAR(20) NULL,
    annonsor_adress NVARCHAR(255) NULL,
    annonsor_postnummer NVARCHAR(10) NULL,
    annonsor_ort NVARCHAR(100) NULL,
    annonsor_typ NVARCHAR(20) NOT NULL,
    prenumerant_id INT NULL,
    ar_prenumerant BIT NOT NULL DEFAULT 0 
);

CREATE TABLE tbl_ads (
    ad_id INT IDENTITY(1,1) PRIMARY KEY,
    ad_rubrik NVARCHAR(100) NOT NULL,
    ad_innehall NVARCHAR(1000) NOT NULL,
    ad_pris DECIMAL(10,2) NOT NULL,
    annonsor_id INT NOT NULL,
    FOREIGN KEY (annonsor_id) REFERENCES tbl_annonsorer(annonsor_id) ON DELETE CASCADE
);

CREATE TABLE tbl_foretag_faktura (
    foretag_id INT IDENTITY(1,1) PRIMARY KEY,
    foretag_namn NVARCHAR(100) NOT NULL,
    organisationsnummer NVARCHAR(20) NOT NULL,
    foretag_telefon NVARCHAR(20) NULL,
    foretag_adress NVARCHAR(255) NULL,
    foretag_postnummer NVARCHAR(10) NULL,
    foretag_ort NVARCHAR(100) NULL,
    faktura_adress NVARCHAR(255) NULL,
    faktura_postnummer NVARCHAR(10) NULL,
    faktura_ort NVARCHAR(100) NULL
);

-- Lägg till en privatperson
INSERT INTO tbl_annonsorer (annonsor_namn, annonsor_telefon, annonsor_adress, annonsor_postnummer, annonsor_ort, annonsor_typ, prenumerant_id)
VALUES ('Anna Svensson', '0701234567', 'Storgatan 12', '11122', 'Stockholm', 'Privatperson', 1);

-- Lägg till ett företag
INSERT INTO tbl_annonsorer (annonsor_namn, annonsor_telefon, annonsor_adress, annonsor_postnummer, annonsor_ort, annonsor_typ)
VALUES ('ABC Reklam AB', '086543210', 'Industrivägen 45', '21432', 'Malmö', 'Företag');

-- Lägg till en annons
INSERT INTO tbl_ads (ad_rubrik, ad_innehall, ad_pris, annonsor_id)
VALUES ('Säljer begagnad cykel', 'En röd damcykel i bra skick.', 1500, 1);

-- Lägg till fler privatpersoner
INSERT INTO tbl_annonsorer (annonsor_namn, annonsor_telefon, annonsor_adress, annonsor_postnummer, annonsor_ort, annonsor_typ, prenumerant_id)
VALUES 
('Erik Andersson', '0701112233', 'Kungsgatan 10', '41122', 'Göteborg', 'Privatperson', 2),
('Sofia Karlsson', '0734445566', 'Vasagatan 5', '11345', 'Stockholm', 'Privatperson', 3),
('Lars Pettersson', '0767778899', 'Bäckvägen 22', '21454', 'Malmö', 'Privatperson', 4),
('Emma Lindberg', '0709876543', 'Drottninggatan 15', '75321', 'Uppsala', 'Privatperson', 5),
('Oskar Johansson', '0723456789', 'Torggatan 8', '22222', 'Lund', 'Privatperson', 6);

-- Lägg till fler företag
INSERT INTO tbl_annonsorer (annonsor_namn, annonsor_telefon, annonsor_adress, annonsor_postnummer, annonsor_ort, annonsor_typ)
VALUES 
('TechVision AB', '0855123456', 'Teknikvägen 18', '11455', 'Stockholm', 'Företag'),
('Malmö Bilcenter', '040223344', 'Motorvägen 7', '21544', 'Malmö', 'Företag'),
('Guld & Smycken AB', '031334455', 'Hamngatan 12', '40010', 'Göteborg', 'Företag'),
('ByggExperten AB', '046556677', 'Hantverksgatan 5', '22220', 'Lund', 'Företag'),
('DataPro IT', '019998877', 'Cybervägen 9', '70374', 'Örebro', 'Företag');

-- Lägg till fakturaadress för ett företag
INSERT INTO tbl_foretag_faktura (foretag_namn, organisationsnummer, foretag_telefon, foretag_adress, foretag_postnummer, foretag_ort, faktura_adress, faktura_postnummer, faktura_ort)
VALUES 
('ABC Reklam AB', '556677-8899', '086543210', 'Industrivägen 45', '21432', 'Malmö', 'Fakturavägen 12', '21450', 'Malmö');

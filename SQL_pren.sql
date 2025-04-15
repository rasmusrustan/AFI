CREATE TABLE tbl_prenumeranter (
    prn_id INT IDENTITY(1,1) PRIMARY KEY, 
    prn_namn VARCHAR(100) NOT NULL,
    prn_telefon VARCHAR(20) NOT NULL,
    prn_adress VARCHAR(255) NOT NULL,
    prn_postnummer VARCHAR(10) NOT NULL,
    prn_ort VARCHAR(100) NOT NULL
);

INSERT INTO tbl_prenumeranter (prn_namn, prn_telefon, prn_adress, prn_postnummer, prn_ort)
VALUES 
('Erik Andersson', '0701112233', 'Kungsgatan 10', '41122', 'Göteborg'),
('Sofia Karlsson', '0734445566', 'Vasagatan 5', '11345', 'Stockholm'),
('Lars Pettersson', '0767778899', 'Bäckvägen 22', '21454', 'Malmö'),
('Emma Lindberg', '0709876543', 'Drottninggatan 15', '75321', 'Uppsala'),
('Oskar Johansson', '0723456789', 'Torggatan 8', '22222', 'Lund');
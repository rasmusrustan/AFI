CREATE TABLE tbl_annonsorer (
    ann_id INT IDENTITY(1,1) PRIMARY KEY, 
    ann_typ VARCHAR(20) CHECK (ann_typ IN ('prenumerant', 'foretag')) NOT NULL, 
    ann_namn VARCHAR(100) NOT NULL,
    ann_telefon VARCHAR(20) NOT NULL,
    ann_adress VARCHAR(255) NOT NULL,
    ann_postnummer VARCHAR(10) NOT NULL,
    ann_ort VARCHAR(100) NOT NULL,
    ann_organisationsnummer VARCHAR(20) NULL, 
    ann_faktura_adress VARCHAR(255) NULL,
    ann_faktura_postnummer VARCHAR(10) NULL,
    ann_faktura_ort VARCHAR(100) NULL
);

CREATE TABLE tbl_ads (
    ad_id INT IDENTITY(1,1) PRIMARY KEY,  
    ann_id INT NOT NULL,  
    ad_rubrik VARCHAR(255) NOT NULL,
    ad_innehall TEXT NOT NULL,
    ad_pris DECIMAL(10,2) NOT NULL,
    ad_annonspris DECIMAL(10,2) NOT NULL DEFAULT 0, 
    FOREIGN KEY (ann_id) REFERENCES tbl_annonsorer(ann_id)
);
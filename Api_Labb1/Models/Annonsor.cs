namespace Api_Labb1.Models
{
    public class Annonsor
    {
        public Annonsor() { }

        public int ID { get; set; }                          // ann_id
        public string Typ { get; set; }                      // ann_typ 
        public string Namn { get; set; }                     // ann_namn
        public string Telefonnummer { get; set; }            // ann_telefon
        public string Adress { get; set; }                   // ann_adress
        public string Postnummer { get; set; }               // ann_postnummer
        public string Ort { get; set; }                      // ann_ort

        public string? Organisationsnummer { get; set; }
        public string? FakturaAdress { get; set; }
        public string? FakturaPostnummer { get; set; }
        public string? FakturaOrt { get; set; }             
    }
}
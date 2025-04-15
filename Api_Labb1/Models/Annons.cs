namespace Api_Labb1.Models
{
    public class Annons
    {
        public Annons() { }
        public int ID { get; set; }              // Motsvarar ad_id
        public int AnnonsorID { get; set; }      // Motsvarar ann_id
        public string Rubrik { get; set; }
        public string Innehall { get; set; }
        public decimal Pris { get; set; }
        public decimal Annonspris { get; set; }
    }

}

namespace Api_Labb1.Models
{
    public class Ad
    {
        public Ad() { }

        public int ID { get; set; }              // ad_id
        public int AnnonsorID { get; set; }      // ann_id
        public string Rubrik { get; set; }       // ad_rubrik
        public string Innehall { get; set; }     // ad_innehall
        public decimal Pris { get; set; }        // ad_pris
        public decimal AnnonsPris { get; set; }  // ad_annonspris

    }
}

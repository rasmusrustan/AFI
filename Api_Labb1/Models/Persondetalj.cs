using Microsoft.AspNetCore.Mvc;

namespace Api_Labb1.Models
{
    public class Persondetalj
    {
        public Persondetalj() { }

        public int PrenumerantID { get; set; }
        public string Namn { get; set; }
        public string Telefonnummer { get; set; }
        public string Utdelningsadress { get; set; }
        public string Postnummer { get; set; }
        public string Ort { get; set; }



    }
}

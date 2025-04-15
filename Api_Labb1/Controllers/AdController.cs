using Microsoft.AspNetCore.Mvc;
using Api_Labb1.Models;
using System.Collections.Generic;

namespace Api_Labb1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdController : Controller
    {
        private readonly AdMetoder adMetoder;
        public AdController()
        {
            adMetoder = new AdMetoder();
        }
        /*-------------------------------READ--------------------------------*/

        [HttpGet(Name = "GetAllAds")]
        public List<Ad> GetAllAds()
        {
            List<Ad> Adlist = new List<Ad>();
            AdMetoder ad = new AdMetoder();
            Adlist = ad.GetAllAds(out string error);
            return Adlist;
        }
        /*-------------------------------READ--------------------------------*/

        [HttpGet("{id}", Name = "GetAdById")]
        public Ad GetAdById(int id)
        {
            Ad ad = new Ad();
            AdMetoder adm = new AdMetoder();
            ad = adm.GetAdById(id, out string error);
            return ad;
        }
        /*------------------------------------CREATE-------------------------------------------*/
        [HttpPost(Name = "InsertAd")]
        public string InsertAd([FromBody] Ad ad)
        {
            AdMetoder adm = new AdMetoder();
            int i = adm.InsertAd(ad, out string error);

            return string.IsNullOrEmpty(error)
                ? "Ad inserted successfully. ID: " + i.ToString()
                : "Error inserting: " + error;
        }

        /*---------------------------------- UPDATE -----------------------------------------------*/
        [HttpPut("{id}", Name = "UpdateAd")]
        public IActionResult UpdateAd(int id, [FromBody] Ad ad)
        {
            ad.ID = id;
            Ad updatedAd = adMetoder.UpdateAd(ad, out string error);
            if (!string.IsNullOrEmpty(error))
                return BadRequest(error);
            return Ok(updatedAd);
        }
        /*---------------------------------- DELETE -----------------------------------------------*/
        [HttpDelete("{id}", Name = "DeleteAd")]
        public IActionResult DeleteAd(int id)
        {
            bool success = adMetoder.DeleteAd(id, out string error);
            if (!success)
            return BadRequest(error);

            return NoContent();
        }
    }
}

using System;
using System.Collections.Generic;
using Api_Labb1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api_Labb1.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class AnnonsorController : Controller
    {
        private readonly Annonsormetoder annonsormetoder;
        public AnnonsorController()
        {
            annonsormetoder = new Annonsormetoder();
        }
        /*-------------------------------READ--------------------------------*/

        [HttpGet(Name = "GetAllAnnonsor")]
        public List<Annonsor> GetAllAnnonsor()
        {
            List<Annonsor> Annonsorlist = new List<Annonsor>();
            Annonsormetoder anm = new Annonsormetoder();
            Annonsorlist = anm.GetAllAnnonsor(out string error);
            return Annonsorlist;
        }
        /*-------------------------------READ--------------------------------*/

        [HttpGet("{id}", Name = "GetAnnonsorById")]
        public Annonsor GetAnnonsorById(int id)
        {
            Annonsor annonsor = new Annonsor();
            Annonsormetoder anm = new Annonsormetoder();
            annonsor = anm.GetAnnonsorById(id, out string error);
            return annonsor;
        }
        [HttpPost(Name = "InsertAnnonsor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult InsertAnnonsor([FromBody] Annonsor annonsor)
        {
            int id = annonsormetoder.InsertAnnonsor(annonsor, out string error);

            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            annonsor.ID = id; // sätt ID som returnerades
            return CreatedAtRoute("GetAnnonsorById", new { id = annonsor.ID }, annonsor);
        }



        /*---------------------------------- UPDATE -----------------------------------------------*/
        [HttpPut("{id}", Name = "UpdateAnnonsor")]
        public IActionResult UpdateAnnonsor(int id, [FromBody] Annonsor annonsor)
        {
            string errormsg;
            annonsor.ID = id;
            Annonsor updatedannonsor = annonsormetoder.UpdateAnnonsor(annonsor, out errormsg);

            if (!string.IsNullOrEmpty(errormsg))
            {
                return BadRequest(errormsg);
            }

            return Ok(updatedannonsor);
        }
        /*---------------------------------- DELETE -----------------------------------------------*/

        [HttpDelete("{id}", Name = "DeleteAnnonsor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult DeleteAnnonsor(int id)
        {
            string errormsg;
            bool success = annonsormetoder.DeleteAnnonsor(id, out errormsg);

            if (!string.IsNullOrEmpty(errormsg))
            {
                return BadRequest(errormsg);
            }

            return NoContent();
        }



    }
}

using System.Collections.Generic;
using Api_Labb1.Models;
using Microsoft.AspNetCore.Mvc;
namespace Api_Labb1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Route("api/[controller]")]

    public class PersonController : Controller
    {
        private readonly Personmetoder personmetoder;

        // Konstruktor som använder PersonMetoder
        public PersonController()
        {
            personmetoder = new Personmetoder(); 
        }

        [HttpGet(Name = "GetAllPersons")]
        public List<Persondetalj> GetAllPersons()
        {
            List<Persondetalj> Personlist = new List<Persondetalj>();
            Personmetoder pm = new Personmetoder();
            Personlist = pm.GetAllPersons(out string error);
            return Personlist;
        }

        [HttpGet("{id}", Name = "GetPersonById")]
        public Persondetalj GetPersonById(int id)
        {
            Persondetalj Person = new Persondetalj();
            Personmetoder pm = new Personmetoder();
            Person = pm.GetPrenumerant(id, out string error);
            return Person;

        }
        /*------------------------------------CREATE-------------------------------------------*/
        [HttpPost(Name = "InsertPrenumerant")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public string InsertPersonDetalj([FromBody] Persondetalj person)
        {
            Personmetoder pm = new Personmetoder();
            string errormsg = "";
            int i = pm.InsertPrenumerant(person, out string error);
            if (errormsg == null)
            {
                return i.ToString();
            }
            else return errormsg;
                
        }
        /*---------------------------------- UPDATE -----------------------------------------------*/
        [HttpPut("{id}", Name = "UpdatePrenumerant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult UpdatePrenumerant(int id, [FromBody] Persondetalj person)
        {
            string errormsg;
            person.PrenumerantID = id; 
            Persondetalj updatedPerson = personmetoder.UpdatePrenumerant(person, out errormsg);

            if (!string.IsNullOrEmpty(errormsg))
            {
                return BadRequest(errormsg);
            }

            return Ok(updatedPerson);
        }
        /*---------------------------------- DELETE -----------------------------------------------*/
        [HttpDelete("{id}", Name = "DeletePrenumerant")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult DeletePrenumerant(int id)
        {
            string errormsg;
            bool success = personmetoder.DeletePrenumerant(id, out errormsg);

            if (!string.IsNullOrEmpty(errormsg))
            {
                return BadRequest(errormsg);
            }

            return NoContent();
        }

    }
}

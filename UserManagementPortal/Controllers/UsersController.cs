using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UserManagementPortal.Data;
using UserManagementPortal.Models;

namespace UserManagementPortal.Controllers
{
    [Route("/api/persons/")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        public readonly PersonContext _personContext;

        public UsersController(PersonContext personContext)
        {
            _personContext = personContext;
            //_personContext.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<ActionResult<List<Person>>> GetPersons()
        {
            return Ok(await _personContext.Persons.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = await _personContext.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return Ok(person);
        }

        [HttpPost]
        public async Task<ActionResult<Person>> AddPerson(Person person)
        {
            _personContext.Persons.Add(person);
            await _personContext.SaveChangesAsync();

            return CreatedAtAction(
                "GetPerson",
                new { id = person.Id },
                person
                );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> EditPerson(int id, Person person)
        {
            if (id != person.Id)
            {
                return BadRequest();
            }
            _personContext.Entry(person).State = EntityState.Modified;
            try
            {
                await _personContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_personContext.Persons.Any(p => p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Person>> DeletePerson(int id)
        {
            var person = await _personContext.Persons.FindAsync(id);
            if(person == null)
            {
                return NotFound();
            }

            _personContext.Persons.Remove(person);
            await _personContext.SaveChangesAsync();

            return Ok(person);
        }

        [HttpPost]
        [Route("delete")]
        public async Task<ActionResult<List<Person>>> DeleteMultiplePersons([FromQuery]int[] ids)
        {
            var persons = new List<Person>();
            foreach (var id in ids)
            {
                var person = await _personContext.Persons.FindAsync(id);
                if(person == null)
                {
                    return NotFound();
                }

                persons.Add(person);
            }

            _personContext.Persons.RemoveRange(persons);
            await _personContext.SaveChangesAsync();

            return Ok(persons);
        }
    }
}

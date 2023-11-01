using ActorRepositoryLib;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ActorsREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private IActorsRepository _actorsRepository;

        public ActorsController(IActorsRepository actorsRepository)
        {
            _actorsRepository = actorsRepository;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // GET: api/Actors?nameFilter=vin
        [HttpGet]
        public ActionResult<IEnumerable<Actor>> Get(
            [FromHeader] string? amount,
            [FromQuery] string? nameFilter,
            [FromQuery] int? minBirthYear,
            [FromQuery] int? maxBirthYear)
        {
            int? parsedAmount = null;
            if (amount != null)
            {
                if (int.TryParse(amount, out var tempParsedAmount))
                {
                    parsedAmount = tempParsedAmount;
                }
                else
                {
                    return BadRequest("Amount must be a integer");
                }
            }

            IEnumerable<Actor> result = _actorsRepository.Get(nameFilter,
                minBirthYear, maxBirthYear, parsedAmount);
            if (result.Any())
            {
                return Ok(result);
            }
            else
            {
                return NoContent();
            }
        }

        // GET api/<ActorsController>/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public ActionResult<Actor> GetById(int id)
        {
            Actor? actor = _actorsRepository.GetById(id);
            if (actor == null) return NotFound();
            return Ok(actor);
        }

        // POST api/<ActorsController>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public ActionResult<Actor> Post([FromBody] Actor newActor)
        {
            try
            {
                Actor addedActor = _actorsRepository.Add(newActor);
                return Created("/" + addedActor.Id, addedActor);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<ActorsController>/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}")]
        public ActionResult<Actor> Put(int id, [FromBody] Actor value)
        {
            try
            {
                Actor? updatedActor = _actorsRepository.Update(id, value);
                if (updatedActor == null) return NotFound();
                else return Ok(updatedActor);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<ActorsController>/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public ActionResult<Actor> Delete(int id)
        {
            Actor? deletedActor = _actorsRepository.Delete(id);
            if (deletedActor == null) return NotFound();
            else return Ok(deletedActor);
        }
    }
}

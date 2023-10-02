using ActorRepositoryLib;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ActorsREST.Controllers
{
    [Route("api/[controller]")]
    [EnableCors(PolicyNames.OnlyZealand)]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private IActorsRepository _actorsRepository;

        public ActorsController(IActorsRepository actorsRepository)
        {
            _actorsRepository = actorsRepository;
        }

        // GET: api/Actors
        [HttpGet]
        public ActionResult<IEnumerable<Actor>> Get(
            [FromHeader] string? amount,
            [FromQuery] string? nameFilter
            )
        {
            int? parsedAmount = null;
            if (amount != null)
            {
                if (int.TryParse(amount, out var tempParsedAmount))
                {
                    parsedAmount = tempParsedAmount;
                } else
                {
                    return BadRequest("Amount must be a integer");
                }
            }

            return Ok(_actorsRepository.Get(nameFilter,
                null, null, parsedAmount));
        }

        // GET api/<ActorsController>/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status418ImATeapot)]
        [EnableCors(PolicyNames.AllowAllPolicy)]
        [HttpGet]
        [Route("{id}")]
        public ActionResult<Actor> GetById([FromHeader] string color
            , string id)
        {
            if (!int.TryParse(id, out int actorId))
            {

                Response.Headers.Add("Volume", "1l");
                return StatusCode(StatusCodes.Status418ImATeapot);
            }
            Actor? actor = _actorsRepository.GetById(actorId);
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
        [HttpPut("{id}")]
        public Actor? Put(int id, [FromBody] Actor value)
        {
            return _actorsRepository.Update(id, value);
        }

        // DELETE api/<ActorsController>/5
        [HttpDelete("{id}")]
        public Actor? Delete(int id)
        {
            return _actorsRepository.Delete(id);
        }
    }
}

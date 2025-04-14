using HandleEvents.Generator;
using HandleEvents.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HandleEvents.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeneratorController : ControllerBase
    {
        private readonly IEventGenerator _generator;
        public GeneratorController(IEventGenerator generator)
        {
            _generator = generator;
        }

        [HttpGet]


        public Task<IActionResult> GenerateEvent()
        {
            var ev = _generator.Generate();

            return Task.FromResult<IActionResult>(Ok(ev));
        }


    }
}

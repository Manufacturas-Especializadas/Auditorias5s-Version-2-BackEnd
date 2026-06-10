using Application.Areas.Queries.GetAreasByModule;
using Application.Areas.Commands.CreateArea;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AreasController(IMediator mediator) => _mediator = mediator;

        [HttpGet("areas/{id:int}")]
        public async Task<IActionResult> GetByModule(int id)
        {
            var response = await _mediator.Send(new GetAreasByModuleQuery(id));
            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateAreaCommand command)
        {
            var response = await _mediator.Send(command);
            return response.Success ? Ok(response) : BadRequest(new { Error = response.ErrorMessage });
        }
    }
}
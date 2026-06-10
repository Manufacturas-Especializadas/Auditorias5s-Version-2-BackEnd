using Application.Areas.Commands.CreateArea;
using Application.Areas.Commands.DeleteArea;
using Application.Areas.Commands.UpdateArea;
using Application.Areas.Queries.GetAreasByModule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateAreaCommand command)
        {
            var response = await _mediator.Send(command);
            return response.Success ? Ok(response) : BadRequest(new { Error = response.ErrorMessage });
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _mediator.Send(new DeleteAreaCommand(id));

            return response.Success ? Ok(new { Message = "Área desactivada con éxito." }) : BadRequest(new { Error = response.ErrorMessage });
        }    
    }
}
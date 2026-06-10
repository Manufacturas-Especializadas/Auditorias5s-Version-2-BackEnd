using Application.Audits.Queries.GetActiveModules;
using Application.Modules.Commands.CreateModule;
using Application.Modules.Commands.DeleteModule;
using Application.Modules.Commands.UpdateModule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ModulesController(IMediator mediator) => _mediator = mediator;

        [HttpGet("all")]
        public async Task<IActionResult> GetAllActive()
        {
            var response = await _mediator.Send(new GetActiveModulesQuery());
            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateModuleCommand command)
        {
            var response = await _mediator.Send(command);
            return response.Success ? Ok(response) : BadRequest(new { Error = response.ErrorMessage });
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateModuleCommand command)
        {
            var response = await _mediator.Send(command);
            return response.Success ? Ok(response) : BadRequest(new { Error = response.ErrorMessage });
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _mediator.Send(new DeleteModuleCommand(id));
            return response.Success ? Ok(new { Message = "Módulo desactivado con éxito." }) : BadRequest(new { Error = response.ErrorMessage });
        }
    }
}
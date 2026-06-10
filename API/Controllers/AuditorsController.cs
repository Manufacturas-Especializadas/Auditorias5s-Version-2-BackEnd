using Application.Auditors.Commands.CreateAuditor;
using Application.Auditors.Commands.DeleteAuditor;
using Application.Auditors.Queries.GetActiveAuditors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditorsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuditorsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllActive()
        {
            var response = await _mediator.Send(new GetActiveAuditorsQuery());
            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateAuditorCommand command)
        {
            var response = await _mediator.Send(command);
            return response.Success ? Ok(response) : BadRequest(new { Error = response.ErrorMessage });
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _mediator.Send(new DeleteAuditorCommand(id));
            return response.Success
                ? Ok(new { Message = "Auditor desactivado con éxito de las listas activas." })
                : BadRequest(new { Error = response.ErrorMessage });
        }
    }
}
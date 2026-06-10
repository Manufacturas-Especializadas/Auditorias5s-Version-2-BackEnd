using Application.Questions.Commands.CreateQuestion;
using Application.Questions.Commands.DeleteQuestion;
using Application.Questions.Commands.GetQuestionsByModule;
using Application.Questions.Commands.UpdateQuestion;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("module/{moduleId:int}")]
        public async Task<IActionResult> GetByModule(int moduleId)
        {
            var response = await _mediator.Send(new GetQuestionsByModuleQuery(moduleId));

            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateQuestionCommand commnad)
        {
            var response = await _mediator.Send(commnad);

            if (!response.Success)
            {
                return BadRequest(new
                {
                    Error = response.ErrorMessage
                });
            }

            return Ok(response);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateQuestionCommand command)
        {
            var response = await _mediator.Send(command);

            return response.Success ? Ok(response) : BadRequest(new { Error = response.ErrorMessage });
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _mediator.Send(new DeleteQuestionCommand(id));

            return response.Success ? Ok(new { Message = "Pregunta desactivada con éxito." }) : BadRequest(new { Error = response.ErrorMessage });
        }
    }
}
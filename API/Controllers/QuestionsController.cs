using Application.Questions.Commands.CreateQuestion;
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
    }
}
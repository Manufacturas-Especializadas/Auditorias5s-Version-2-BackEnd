using Application.Auditors.Commands.UpdateAuditor;
using MesaCore.Audits.Application.Audits.Queries.GetAuditExcelReport;
using Application.Audits.Commands;
using Application.Audits.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly GetAuditExcelReportQueryHandler _excelHandler;

        public AuditsController(IMediator mediator, GetAuditExcelReportQueryHandler excelHandler)
        {
            _mediator = mediator;
            _excelHandler = excelHandler;
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var response = await _mediator.Send(new GetAuditsHistoryQuery());
            return Ok(response);
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var audit = await _mediator.Send(new GetAuditByIdQuery(id));

            if (audit == null) return NotFound("La auditoría no existe.");

            return Ok(audit);
        }

        [HttpGet("download-excel/{id}")]
        public async Task<IActionResult> DownloadExcel([FromRoute] int id)
        {
            var reportData = await _mediator.Send(new GetAuditExcelReportQuery(id));
            if (reportData == null)
                return NotFound(new { message = $"No se encontró la auditoría con ID {id}" });

            byte[] fileBytes = _excelHandler.GenerateExcelByteArray(reportData);
            string fileName = $"Auditoria_5S_{reportData.AreaName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.xlsx";

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateAuditCommand command)
        {
            var response = await _mediator.Send(command);

            if (!response.Success)
            {
                return BadRequest(new { Error = response.ErrorMessage });
            }

            return Ok(response);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAudit([FromRoute] int id, [FromBody] UpdateAuditCommand commnad)
        {
            if (id != commnad.AuditId)
            {
                return BadRequest("El ID de la ruta no coincide con el ID proporcionado");
            }

            var success = await _mediator.Send(commnad);

            if(!success)
            {
                return BadRequest("No se pudo actualizar la auditoría");
            }

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAudit(int id)
        {
            var success = await _mediator.Send(new DeleteAuditCommand(id));

            if (!success) return NotFound("La auditoría no existe");

            return Ok();
        }
    }
}
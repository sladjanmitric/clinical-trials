using ClinicalTrials.API.Common;
using ClinicalTrials.Application.ClinicalTrial.Commands.UploadFile;
using ClinicalTrials.Application.ClinicalTrial.Queries.GetClimateTrial;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalTrials.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicalTrialsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ClinicalTrialsController(IMediator mediator, ILogger<ErrorHandlingMiddleware> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(UploadFileRequest request)
        {
            _logger.LogWarning("Upload clinical trial information started.");

            var command = new UploadFileCommand
            {
                File = request.File
            };

            var result = await _mediator.Send(command);

            _logger.LogWarning("Upload clinical trial information completed.");

            return Ok(result);
        }

        [HttpGet("{trialId}")]
        public async Task<IActionResult> GetClinicalTrialById(Guid trialId)
        {
            _logger.LogWarning("Get clinical trial information by id started.");

            var query = new GetClinicalTrialByIdQuery
            {
                TrialId = trialId
            };

            var result = await _mediator.Send(query);

            _logger.LogWarning("Get clinical trial information by id finished.");

            return Ok(result);
            
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllClinicalTrials([FromQuery] int? status, [FromQuery] string? title)
        {
            _logger.LogWarning("Get filtered clinical trial information started.");

            var query = new GetAllClinicalTrialQuery
            {
                Status = status,
                Title = title
            };

            var result = await _mediator.Send(query);

            _logger.LogWarning("Get filtered clinical trial information started.");

            return Ok(result);
        }

    }
}

using ClinicalTrials.Application.Common.Interfaces.Persistence;
using ClinicalTrials.Domain.Entity;
using MediatR;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace ClinicalTrials.Application.ClinicalTrial.Commands.UploadFile
{
    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, Trial>
    {
        private readonly IClinicalTrialRepository _clinicalTrialRepository;
        private readonly ILogger<UploadFileCommandHandler> _logger;

        public UploadFileCommandHandler(IClinicalTrialRepository clinicalTrialRepository, ILogger<UploadFileCommandHandler> logger)
        {
            _clinicalTrialRepository = clinicalTrialRepository;
            _logger = logger;
        }

        public async Task<Trial> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogWarning("File validation succesfully.");

            var file = request.File;

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var trialFileContent = JsonSerializer.Deserialize<Trial>(content, options);

            if (trialFileContent == null)
            {
                throw new InvalidOperationException("Deserialization failed.");
            }

            trialFileContent.TrialId = Guid.NewGuid();

            if (trialFileContent.EndDate == null)
            {
                trialFileContent.EndDate = trialFileContent.StartDate.AddMonths(1);
            }

            trialFileContent.Duration = (int)(trialFileContent.EndDate - trialFileContent.StartDate).Value.TotalDays;

            _logger.LogWarning("Clinical trial information saving to db.");

            var trial = await _clinicalTrialRepository.AddClinicalTrialAsync(trialFileContent);

            if (trial == null)
            {
                throw new InvalidOperationException("Saving trial in the database was not successful.");
            }

            _logger.LogWarning("Clinical trial information saved to db successfuly.");

            return trial;
        }
    }
}

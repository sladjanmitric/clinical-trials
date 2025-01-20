using ClinicalTrials.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ClinicalTrials.Application.ClinicalTrial.Commands.UploadFile
{
    public class UploadFileCommand : IRequest<Trial>
    {
        public IFormFile File { get; set; }
    }
}

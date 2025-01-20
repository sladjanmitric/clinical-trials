using Microsoft.AspNetCore.Http;

namespace ClinicalTrials.Application.ClinicalTrial.Commands.UploadFile
{
    public class UploadFileRequest
    {
        public IFormFile File { get; set; }
    }
}

using FluentValidation;
using Microsoft.AspNetCore.Http;
using NJsonSchema;
using System.Reflection;

namespace ClinicalTrials.Application.ClinicalTrial.Commands.UploadFile
{
    public class UploadFileValidator : AbstractValidator<UploadFileCommand>
    {
        private const int MaxFileSizeInMb = 10;
        private const long MaxFileSizeInBytes = MaxFileSizeInMb * 1024 * 1024;
        private JsonSchema _schema;

        public UploadFileValidator()
        {
            RuleFor(x => x.File)
                .NotNull()
                .WithMessage("File is required")
                .DependentRules(() =>
                {
                    RuleFor(x => x.File)
                        .Must(file => file != null && file.Length > 0)
                        .WithMessage("File cannot be empty")
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.File)
                                .Must(file => file != null && file.Length <= MaxFileSizeInBytes)
                                .WithMessage($"File size must be less than {MaxFileSizeInMb}MB")
                                .DependentRules(() =>
                                {
                                    RuleFor(x => x.File)
                                        .Must(HaveValidExtension)
                                        .WithMessage("Only .json files are allowed")
                                        .DependentRules(() =>
                                        {
                                            RuleFor(x => x.File)
                                                .MustAsync(BeValidJson)
                                                .WithMessage("File content is not valid according to the schema");
                                        });
                                });
                        });
                });
        }

        private bool HaveValidExtension(IFormFile file)
        {
            if (file == null) return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return extension == ".json";
        }

        private async Task<bool> BeValidJson(IFormFile file, CancellationToken cancellationToken)
        {
            _schema = LoadSchema().GetAwaiter().GetResult();

            if (file == null) return false;

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();

            var errors = _schema.Validate(content);
            return errors.Count == 0;
        }

        private async Task<JsonSchema> LoadSchema()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var allresourcess = assembly.GetManifestResourceNames();
            var resourceName = "ClinicalTrials.Application.clinicalTrialSchema.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Resource '{resourceName}' not found.");
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    var schemaJson = await reader.ReadToEndAsync();
                    return await JsonSchema.FromJsonAsync(schemaJson);
                }
            }
        }
    }
}

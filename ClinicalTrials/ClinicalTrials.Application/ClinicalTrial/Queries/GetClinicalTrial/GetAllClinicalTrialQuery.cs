using ClinicalTrials.Domain.Entity;
using MediatR;

namespace ClinicalTrials.Application.ClinicalTrial.Queries.GetClinicalTrial
{
    public class GetAllClinicalTrialQuery : IRequest<IEnumerable<Trial>>
    {
        public int? Status { get; set; }
        public string? Title { get; set; }
    }
}

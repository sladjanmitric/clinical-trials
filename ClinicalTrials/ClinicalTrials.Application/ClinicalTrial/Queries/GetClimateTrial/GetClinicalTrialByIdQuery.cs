using ClinicalTrials.Domain.Entity;
using MediatR;

namespace ClinicalTrials.Application.ClinicalTrial.Queries.GetClimateTrial
{
    public class GetClinicalTrialByIdQuery : IRequest<Trial>
    {
        public Guid TrialId { get; set; }
    }


}

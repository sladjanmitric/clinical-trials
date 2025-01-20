using ClinicalTrials.Domain.Entity;
using MediatR;

namespace ClinicalTrials.Application.ClinicalTrial.Queries.GetClinicalTrial
{
    public class GetClinicalTrialByIdQuery : IRequest<Trial>
    {
        public Guid TrialId { get; set; }
    }


}

using ClinicalTrials.Application.Common.Interfaces.Persistence;
using ClinicalTrials.Domain.Common.Enums;
using ClinicalTrials.Domain.Entity;
using MediatR;

namespace ClinicalTrials.Application.ClinicalTrial.Queries.GetClinicalTrial
{
    public class GetAllClinicalTrialQueryHandler : IRequestHandler<GetAllClinicalTrialQuery, IEnumerable<Trial>>
    {
        private readonly IClinicalTrialRepository _clinicalTrialRepository;

        public GetAllClinicalTrialQueryHandler(IClinicalTrialRepository clinicalTrialRepository)
        {
            _clinicalTrialRepository = clinicalTrialRepository;
        }

        public async Task<IEnumerable<Trial>> Handle(GetAllClinicalTrialQuery query, CancellationToken cancellationToken)
        {
            return await _clinicalTrialRepository.GetAllClinicalTrialsAsync(
                query.Status.HasValue ? (TrialStatus)query.Status.Value : (TrialStatus?)null,
                query.Title);
        }
    }
}

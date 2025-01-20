using System;
using ClinicalTrials.Application.Common.Interfaces.Persistence;
using ClinicalTrials.Domain.Entity;
using MediatR;

namespace ClinicalTrials.Application.ClinicalTrial.Queries.GetClinicalTrial
{
    internal class GetClinicalTrialByIdQueryHandler : IRequestHandler<GetClinicalTrialByIdQuery, Trial>
    {
        private readonly IClinicalTrialRepository _clinicalTrialRepository;

        public GetClinicalTrialByIdQueryHandler(IClinicalTrialRepository clinicalTrialRepository)
        {
            _clinicalTrialRepository = clinicalTrialRepository;
        }

        public async Task<Trial> Handle(GetClinicalTrialByIdQuery query, CancellationToken cancellationToken)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var clinicalTrial = await _clinicalTrialRepository.GetClinicalTrialByIdAsync(query.TrialId);

            if (clinicalTrial == null)
            {
                throw new KeyNotFoundException($"Clinical trial with ID {query.TrialId} was not found.");
            }

            return clinicalTrial;
        }
    }
}

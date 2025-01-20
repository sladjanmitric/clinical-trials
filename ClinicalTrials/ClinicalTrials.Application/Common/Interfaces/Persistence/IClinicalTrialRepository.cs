using ClinicalTrials.Domain.Common.Enums;
using ClinicalTrials.Domain.Entity;

namespace ClinicalTrials.Application.Common.Interfaces.Persistence
{
    public interface IClinicalTrialRepository
    {
        Task<Trial> GetClinicalTrialByIdAsync(Guid id);
        Task<Trial> AddClinicalTrialAsync(Trial clinicalTrial);
        Task<IEnumerable<Trial>> GetAllClinicalTrialsAsync(TrialStatus? status = null, string? title = null);
    }
}

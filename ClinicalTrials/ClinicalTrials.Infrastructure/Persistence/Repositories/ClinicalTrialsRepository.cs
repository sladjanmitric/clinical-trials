using ClinicalTrials.Application.Common.Interfaces.Persistence;
using ClinicalTrials.Domain.Common.Enums;
using ClinicalTrials.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrials.Infrastructure.Persistence.Repositories
{
    public class ClinicalTrialsRepository : IClinicalTrialRepository
    {
        private readonly ClientTrialsDbContext _dbContext;

        public ClinicalTrialsRepository(ClientTrialsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Trial> AddClinicalTrialAsync(Trial clinicalTrial)
        {
            await _dbContext.AddAsync(clinicalTrial);

            _dbContext.SaveChanges();
            return clinicalTrial;
        }

        public async Task<Trial> GetClinicalTrialByIdAsync(Guid id)
        {
            var query = _dbContext.ClinicalTrials.Where(u => u.TrialId.Equals(id));

            var trial = await query.SingleOrDefaultAsync();

            if (trial == null)
            {
                throw new KeyNotFoundException($"Trial with ID {id} not found.");
            }

            return trial;
        }

        public async Task<IEnumerable<Trial>> GetAllClinicalTrialsAsync(TrialStatus? status = null, string? title = null)
        {
            var query = _dbContext.ClinicalTrials.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(t => t.Title.Contains(title));
            }

            return await query.ToListAsync();
        }
    }
}

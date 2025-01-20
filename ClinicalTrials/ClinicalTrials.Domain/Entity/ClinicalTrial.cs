using ClinicalTrials.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicalTrials.Domain.Entity
{
    public class Trial
    {
        [Key]
        public Guid TrialId { get; set; }

        [Required]
        [StringLength(25)]
        public string Title { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Range(1, 5)]
        public int? Participants { get; set; }

        [Required]
        public TrialStatus Status { get; set; }
        public int Duration { get; set; }
    }
}

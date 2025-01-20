using ClinicalTrials.Domain.Common.Enums;
using ClinicalTrials.Domain.Entity;
using ClinicalTrials.Infrastructure.Persistence;
using ClinicalTrials.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MockQueryable.Moq;
using Moq;

namespace ClinicalTrials.UnitTests
{
    [TestFixture]
    public class ClinicalTrialsRepositoryTests
    {
        private Mock<ClientTrialsDbContext> _mockDbContext;
        private Mock<DbSet<Trial>> _mockTrialDbSet;
        private ClinicalTrialsRepository _repository;
        private List<Trial> _trials;

        [SetUp]
        public void Setup()
        {
            _trials = new List<Trial>();
            _mockTrialDbSet = new Mock<DbSet<Trial>>();
            _mockDbContext = new Mock<ClientTrialsDbContext>();

            // Setup the DbSet mock  
            var queryable = _trials.AsQueryable();
            _mockTrialDbSet = queryable.BuildMockDbSet();

            _mockDbContext.Setup(c => c.ClinicalTrials).Returns(_mockTrialDbSet.Object);

            _repository = new ClinicalTrialsRepository(_mockDbContext.Object);
        }

        [Test]
        public async Task AddClinicalTrialAsync_ShouldAddTrialAndReturnIt()
        {
            // Arrange
            var trial = new Trial { TrialId = Guid.NewGuid(), Title = "Test Trial" };
            _mockDbContext.Setup(x => x.AddAsync(It.IsAny<Trial>(), It.IsAny<CancellationToken>()))
                  .Returns(ValueTask.FromResult((EntityEntry<Trial>)null));

            // Act
            var result = await _repository.AddClinicalTrialAsync(trial);

            // Assert
            Assert.That(result, Is.EqualTo(trial));
            _mockDbContext.Verify(x => x.AddAsync(trial, It.IsAny<CancellationToken>()), Times.Once);
            _mockDbContext.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Test]
        public async Task GetClinicalTrialByIdAsync_WhenTrialExists_ShouldReturnTrial()
        {
            // Arrange
            var trialId = Guid.NewGuid();
            var trial = new Trial { TrialId = trialId, Title = "Test Trial" };
            _trials.Add(trial);

            // Act
            var result = await _repository.GetClinicalTrialByIdAsync(trialId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TrialId, Is.EqualTo(trialId));
        }

        [Test]
        public void GetClinicalTrialByIdAsync_WhenTrialDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _repository.GetClinicalTrialByIdAsync(nonExistentId));
        }

        [Test]
        public async Task GetAllClinicalTrialsAsync_WithNoFilters_ShouldReturnAllTrials()
        {
            // Arrange
            _trials.AddRange(new[]
            {
                new Trial { TrialId = Guid.NewGuid(), Title = "Trial 1" },
                new Trial { TrialId = Guid.NewGuid(), Title = "Trial 2" }
            });

            // Act
            var result = await _repository.GetAllClinicalTrialsAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllClinicalTrialsAsync_WithStatusFilter_ShouldReturnFilteredTrials()
        {
            // Arrange
            _trials.AddRange(new[]
            {
                new Trial { TrialId = Guid.NewGuid(), Title = "Trial 1", Status = TrialStatus.OnGoing },
                new Trial { TrialId = Guid.NewGuid(), Title = "Trial 2", Status = TrialStatus.Completed }
            });

            // Act
            var result = await _repository.GetAllClinicalTrialsAsync(status: TrialStatus.OnGoing);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Status, Is.EqualTo(TrialStatus.OnGoing));
        }

        [Test]
        public async Task GetAllClinicalTrialsAsync_WithTitleFilter_ShouldReturnFilteredTrials()
        {
            // Arrange
            _trials.AddRange(new[]
            {
                new Trial { TrialId = Guid.NewGuid(), Title = "Test Trial 1" },
                new Trial { TrialId = Guid.NewGuid(), Title = "Test Trial 2" }
            });

            // Act
            var result = await _repository.GetAllClinicalTrialsAsync(title: "Test Trial 1");

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Does.Contain("Test Trial 1"));
        }
    }
}

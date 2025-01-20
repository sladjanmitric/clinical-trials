using ClinicalTrials.Application.ClinicalTrial.Commands.UploadFile;
using ClinicalTrials.Application.Common.Interfaces.Persistence;
using ClinicalTrials.Domain.Common.Enums;
using ClinicalTrials.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using System.Text.Json;

namespace ClinicalTrials.UnitTests
{
    [TestFixture]
    internal class UploadFileCommandHandlerTests
    {
        private Mock<IClinicalTrialRepository> _clinicalTrialRepositoryMock;
        private Mock<ILogger<UploadFileCommandHandler>> _loggerMock;
        private UploadFileCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _clinicalTrialRepositoryMock = new Mock<IClinicalTrialRepository>();
            _loggerMock = new Mock<ILogger<UploadFileCommandHandler>>();
            _handler = new UploadFileCommandHandler(_clinicalTrialRepositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnTrial_WhenFileIsValid()
        {
            // Arrange
            var trial = new Trial
            {
                TrialId = Guid.NewGuid(),
                Title = "Test Trial",
                StartDate = new DateTime(2025, 01, 20),
                EndDate = new DateTime(2025, 01, 22),
                Duration = 2,
                Participants = 3,
                Status = TrialStatus.OnGoing
            };

            var fileContent = JsonSerializer.Serialize(trial);
            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);

            var command = new UploadFileCommand { File = fileMock.Object };

            _clinicalTrialRepositoryMock.Setup(repo => repo.AddClinicalTrialAsync(It.IsAny<Trial>()))
                                        .ReturnsAsync(trial);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.TrialId, Is.EqualTo(trial.TrialId));
            Assert.That(result.Title, Is.EqualTo(trial.Title));
            Assert.That(result.StartDate, Is.EqualTo(trial.StartDate));
            Assert.That(result.EndDate, Is.EqualTo(trial.EndDate));
            Assert.That(result.Duration, Is.EqualTo(trial.Duration));
            Assert.That(result.Participants, Is.EqualTo(trial.Participants));
            Assert.That(result.Status, Is.EqualTo(trial.Status));
        }

        [Test]
        public void Handle_ShouldThrowArgumentException_WhenFileIsInvalid()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Invalid Content"));
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);

            var command = new UploadFileCommand { File = fileMock.Object };

            // Act & Assert
            Assert.ThrowsAsync<JsonException>(async () => await _handler.Handle(command, CancellationToken.None));
        }

        [Test]
        public void Handle_ShouldThrowInvalidOperationException_WhenSavingTrialFails()
        {
            // Arrange
            var trial = new Trial
            {
                TrialId = Guid.NewGuid(),
                Title = "Test Trial",
                StartDate = new DateTime(2025, 01, 20),
                EndDate = new DateTime(2025, 01, 22),
                Duration = 2,
                Participants = 3,
                Status = TrialStatus.OnGoing
            };

            var fileContent = JsonSerializer.Serialize(trial);
            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);

            var command = new UploadFileCommand { File = fileMock.Object };

            _clinicalTrialRepositoryMock.Setup(repo => repo.AddClinicalTrialAsync(It.IsAny<Trial>()))
                                        .ReturnsAsync((Trial)null);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(command, CancellationToken.None));
        }
    }
}

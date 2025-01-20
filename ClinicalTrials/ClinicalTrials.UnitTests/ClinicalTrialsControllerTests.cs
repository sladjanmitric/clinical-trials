using ClinicalTrials.API.Common;
using ClinicalTrials.API.Controllers;
using ClinicalTrials.Application.ClinicalTrial.Commands.UploadFile;
using ClinicalTrials.Application.ClinicalTrial.Queries.GetClinicalTrial;
using ClinicalTrials.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ClinicalTrials.UnitTests
{
    [TestFixture]
    public class ClinicalTrialsControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<ILogger<ErrorHandlingMiddleware>> _loggerMock;
        private ClinicalTrialsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            _controller = new ClinicalTrialsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task UploadFile_ShouldReturnOkResult()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var request = new UploadFileRequest { File = fileMock.Object };

            var commandResult = new Trial() 
            { TrialId = Guid.NewGuid(),
              Title = "testTitle", 
              StartDate = new DateTime(2025,01,20), 
              EndDate = new DateTime(2025, 01, 22),
              Duration = 2,
              Participants = 3,
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UploadFileCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.UploadFile(request);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(commandResult));
        }

        [Test]
        public async Task GetClinicalTrialById_ShouldReturnOkResult()
        {
            // Arrange
            var trialId = Guid.NewGuid();
            var queryResult = new Trial()
            {
                TrialId = Guid.NewGuid(),
                Title = "testTitle",
                StartDate = new DateTime(2025, 01, 20),
                EndDate = new DateTime(2025, 01, 22),
                Duration = 2,
                Participants = 3,
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetClinicalTrialByIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(queryResult);

            // Act
            var result = await _controller.GetClinicalTrialById(trialId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(queryResult));
        }

        [Test]
        public async Task GetAllClinicalTrials_ShouldReturnOkResult()
        {
            // Arrange
            var queryResult = new List<Trial>()
            {
                new Trial()
                {
                    TrialId = Guid.NewGuid(),
                    Title = "testTitle",
                    StartDate = new DateTime(2025, 01, 20),
                    EndDate = new DateTime(2025, 01, 22),
                    Duration = 2,
                    Participants = 3
                },
                new Trial()
                {
                    TrialId = Guid.NewGuid(),
                    Title = "testTitle2",
                    StartDate = new DateTime(2025, 01, 21),
                    EndDate = new DateTime(2025, 01, 23),
                    Duration = 2,
                    Participants = 3
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllClinicalTrialQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(queryResult);

            // Act
            var result = await _controller.GetAllClinicalTrials(null, null);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(queryResult));
        }
    }
}
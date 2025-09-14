using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using search_flight_backend.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework.Legacy;
using search_flight_backend.Services;

namespace search_flight_backend.Tests.Controllers
{
    [TestFixture]
    public class SearchFlightControllerTests
    {
        private Mock<ILogger<SearchFlightController>> _loggerMock;
        private Mock<IAirportService> _airportServiceMock; // Changed from IHttpClientFactory to IAirportService
        private SearchFlightController _controller;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<SearchFlightController>>();
            _airportServiceMock = new Mock<IAirportService>(); // Changed from IHttpClientFactory to IAirportService
            _controller = new SearchFlightController(_loggerMock.Object, _airportServiceMock.Object); // Updated to use IAirportService
        }

        [Test]
        public async Task GetOriginAirports_ReturnsOkWithAirports()
        {
            // Act
            var result = await _controller.GetOriginAirports();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var airports = okResult.Value as List<string>;
            Assert.That(airports, Is.Not.Null);
            CollectionAssert.AreEqual(
                new List<string> { "Chennai", "Banglore", "Delhi", "Kolkata", "Mumbai" },
                airports
            );
        }

        [Test]
        public async Task GetDestinationAirports_ReturnsBadRequest_WhenOriginIsNull()
        {
            // Act
            var result = await _controller.GetDestinationAirports(null);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null);
            Assert.That(badRequest.Value, Is.EqualTo("Origin airport is needed"));
        }

        [Test]
        public async Task GetDestinationAirports_ReturnsNotFound_WhenOriginIsInvalid()
        {
            // Act
            var result = await _controller.GetDestinationAirports("InvalidAirport");

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFound = result as NotFoundObjectResult;
            Assert.That(notFound, Is.Not.Null);
            Assert.That(notFound.Value, Is.EqualTo("There is no destination pair for origin InvalidAirport"));
        }

        [Test]
        public async Task GetDestinationAirports_ReturnsOkWithDestinations()
        {
            // Act
            var result = await _controller.GetDestinationAirports("Mumbai");

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf<List<string>>());
            var destinations = okResult.Value as List<string>;
            Assert.That(destinations, Is.Not.Null);
            CollectionAssert.AreEqual(new[] { "Chennai", "Banglore", "Delhi", "Kolkata" }, destinations);
        }
    }
}

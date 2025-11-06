using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using Registrations.Classes;
using Registrations.Controllers;
using Registrations.Models;

namespace RegistrationAPI.tests
{
	public class UnitTest1
	{
		private readonly Mock<ILogger<RegistrationController>> _mockLogger;
		private readonly Mock<IDbContextFactory<PostgresContext>> _mockDbContextFactory;
		private readonly RegistrationController _controller;

		public UnitTest1()
		{
			_mockLogger = new Mock<ILogger<RegistrationController>>();
			_mockDbContextFactory = new Mock<IDbContextFactory<PostgresContext>>();
			_controller = new RegistrationController(_mockLogger.Object, _mockDbContextFactory.Object);
		}

		[Fact]
		public void Post_ValidRegistration_ReturnsCreatedResult()
		{
			// Arrange
			var data = new RegistrationData { Id = "CAR123", Region = "US" };
			var mockContext = new Mock<PostgresContext>(new DbContextOptions<PostgresContext>());

			var lookupRegistrations = new List<LookupRegistration>
			{
				new LookupRegistration { Id = 1, Region = "US" }
			};

			var registrations = new List<Registration>();

			mockContext.Setup(x => x.LookupRegistrations).ReturnsDbSet(lookupRegistrations);
			mockContext.Setup(x => x.Registrations).ReturnsDbSet(registrations);
			mockContext.Setup(x => x.SaveChanges()).Returns(1);

			_mockDbContextFactory.Setup(f => f.CreateDbContext()).Returns(mockContext.Object);

			// Act
			var result = _controller.Post(data);

			// Assert
			var createdResult = Assert.IsType<CreatedResult>(result);
			Assert.Equal(201, createdResult.StatusCode);

			mockContext.Verify(c => c.Registrations.Add(It.IsAny<Registration>()), Times.Once);
			mockContext.Verify(c => c.SaveChanges(), Times.Once);
		}
	}
}




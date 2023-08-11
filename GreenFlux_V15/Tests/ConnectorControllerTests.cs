using GreenFlux_V15.Controllers;
using GreenFlux_V15.Data;
using GreenFlux_V15.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GreenFlux_V15.Tests.ConnectorTests
{
    public class ConnectorControllerTests
    {
        private readonly Mock<ChargingDbContext> _mockContext;

        public ConnectorControllerTests()
        {
            _mockContext = new Mock<ChargingDbContext>();
        }

        [Fact]
        public async Task Test_Index_ReturnsViewResultWithListOfConnectors()
        {
            // Arrange
            var connectors = new List<Connector>
            {
                new Connector { Id = 1, MaxCurrentInAmps = 50, ChargeStationId = 1 },
                new Connector { Id = 2, MaxCurrentInAmps = 100, ChargeStationId = 2 }
            }.AsQueryable();

            var mockSet = GetMockDbSet(connectors);

            _mockContext.Setup(c => c.Connectors).Returns(mockSet.Object);

            var controller = new ConnectorController(_mockContext.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Connector>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Test_Create_Get_ReturnsViewResult()
        {
            // Arrange
            var controller = new ConnectorController(_mockContext.Object);

            // Act
            var result = controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Test_Create_Post_ValidModelState_RedirectsToIndex()
        {
            // Arrange
            var controller = new ConnectorController(_mockContext.Object);
            var newConnector = new Connector { MaxCurrentInAmps = 150, ChargeStationId = 1 };
            var chargeStation = new ChargeStation { Id = 1 };

            _mockContext.Setup(c => c.ChargeStations.FindAsync(newConnector.ChargeStationId))
                        .ReturnsAsync(chargeStation);

            // Act
            var result = await controller.Create(newConnector);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Test_Create_Post_InvalidChargeStation_ReturnsViewWithModel()
        {
            // Arrange
            var controller = new ConnectorController(_mockContext.Object);
            var newConnector = new Connector { MaxCurrentInAmps = 150, ChargeStationId = 1 };

            _mockContext.Setup(c => c.ChargeStations.FindAsync(newConnector.ChargeStationId))
                        .ReturnsAsync((ChargeStation)null);

            // Act
            var result = await controller.Create(newConnector);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Connector>(viewResult.Model);
            Assert.Equal(newConnector.ChargeStationId, model.ChargeStationId);
            Assert.Equal(newConnector.MaxCurrentInAmps, model.MaxCurrentInAmps);
            Assert.True(controller.ModelState.ContainsKey("ChargeStationId"));
        }

        [Fact]
        public async Task Test_Edit_Get_ReturnsViewResult()
        {
            // Arrange
            var connector = new Connector { Id = 1, MaxCurrentInAmps = 50, ChargeStationId = 1 };
            _mockContext.Setup(c => c.Connectors.FindAsync(connector.Id)).ReturnsAsync(connector);

            var controller = new ConnectorController(_mockContext.Object);

            // Act
            var result = await controller.Edit(connector.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Connector>(viewResult.Model);
            Assert.Equal(connector.Id, model.Id);
            Assert.Equal(connector.MaxCurrentInAmps, model.MaxCurrentInAmps);
            Assert.Equal(connector.ChargeStationId, model.ChargeStationId);
        }

        [Fact]
        public async Task Test_Edit_Post_ValidModelState_RedirectsToIndex()
        {
            // Arrange
            var controller = new ConnectorController(_mockContext.Object);
            var connector = new Connector { Id = 1, MaxCurrentInAmps = 150, ChargeStationId = 1 };
            var chargeStation = new ChargeStation { Id = 1 };

            _mockContext.Setup(c => c.ChargeStations.FindAsync(connector.ChargeStationId))
                        .ReturnsAsync(chargeStation);
            _mockContext.Setup(c => c.Connectors.FindAsync(connector.Id)).ReturnsAsync(connector);

            // Act
            var result = await controller.Edit(connector.Id, connector);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Test_Delete_Get_ReturnsViewResult()
        {
            // Arrange
            var connector = new Connector { Id = 1, MaxCurrentInAmps = 50, ChargeStationId = 1 };
            _mockContext.Setup(c => c.Connectors.FindAsync(connector.Id)).ReturnsAsync(connector);

            var controller = new ConnectorController(_mockContext.Object);

            // Act
            var result = await controller.Delete(connector.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Connector>(viewResult.Model);
            Assert.Equal(connector.Id, model.Id);
            Assert.Equal(connector.MaxCurrentInAmps, model.MaxCurrentInAmps);
            Assert.Equal(connector.ChargeStationId, model.ChargeStationId);
        }

        [Fact]
        public async Task Test_DeleteConfirmed_DeletesAndRedirectsToIndex()
        {
            // Arrange
            var connector = new Connector { Id = 1, MaxCurrentInAmps = 50, ChargeStationId = 1 };
            _mockContext.Setup(c => c.Connectors.FindAsync(It.IsAny<int>())).ReturnsAsync(connector);

            var controller = new ConnectorController(_mockContext.Object);

            // Act
            var result = await controller.DeleteConfirmed(connector.Id);

            // Assert
            _mockContext.Verify(c => c.Connectors.Remove(connector), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }



        [Fact]
        public async Task Test_Create_Post_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var controller = new ConnectorController(_mockContext.Object);
            var connector = new Connector { MaxCurrentInAmps = 150, ChargeStationId = 1 };
            controller.ModelState.AddModelError("MaxCurrentInAmps", "Invalid MaxCurrentInAmps");

            // Act
            var result = await controller.Create(connector);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Connector>(viewResult.Model);
            Assert.Equal(connector.MaxCurrentInAmps, model.MaxCurrentInAmps);
            Assert.Equal(connector.ChargeStationId, model.ChargeStationId);
        }

        [Fact]
        public async Task Test_Create_Post_InvalidChargeStationId_ReturnsViewWithModelError()
        {
            // Arrange
            var controller = new ConnectorController(_mockContext.Object);
            var connector = new Connector { MaxCurrentInAmps = 150, ChargeStationId = 1 };

            // Act
            var result = await controller.Create(connector);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(viewResult.ViewData.ModelState.ContainsKey("ChargeStationId"));
        }

        [Fact]
        public async Task Test_Edit_Get_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = -1;
            _mockContext.Setup(c => c.Connectors.FindAsync(invalidId)).ReturnsAsync((Connector)null);

            var controller = new ConnectorController(_mockContext.Object);

            // Act
            var result = await controller.Edit(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Test_Edit_Post_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = -1;
            var connector = new Connector { Id = 1, MaxCurrentInAmps = 150, ChargeStationId = 1 };

            _mockContext.Setup(c => c.Connectors.FindAsync(invalidId)).ReturnsAsync((Connector)null);

            var controller = new ConnectorController(_mockContext.Object);

            // Act
            var result = await controller.Edit(invalidId, connector);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Test_Delete_Get_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = -1;
            _mockContext.Setup(c => c.Connectors.FindAsync(invalidId)).ReturnsAsync((Connector)null);

            var controller = new ConnectorController(_mockContext.Object);

            // Act
            var result = await controller.Delete(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Test_DeleteConfirmed_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = -1;
            _mockContext.Setup(c => c.Connectors.FindAsync(invalidId)).ReturnsAsync((Connector)null);

            var controller = new ConnectorController(_mockContext.Object);

            // Act
            var result = await controller.DeleteConfirmed(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // ... (other test methods)


        private static Mock<DbSet<T>> GetMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }
    }
}

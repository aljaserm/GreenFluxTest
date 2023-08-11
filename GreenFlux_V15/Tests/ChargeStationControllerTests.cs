using GreenFlux_V15.Controllers;
using GreenFlux_V15.Data;
using GreenFlux_V15.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GreenFlux_V15.Tests.ChargeStationTests
{
    public class ChargeStationControllerTests
    {
        private readonly Mock<ChargingDbContext> _mockContext;

        public ChargeStationControllerTests()
        {
            _mockContext = new Mock<ChargingDbContext>();
        }

        [Fact]
        public async Task Test_Index_ReturnsViewResultWithListOfChargeStations()
        {
            // Arrange
            var chargeStations = new List<ChargeStation>
            {
                new ChargeStation { Id = 1, Name = "CS 1", GroupId = 1 },
                new ChargeStation { Id = 2, Name = "CS 2", GroupId = 2 }
            }.AsQueryable();

            var mockSet = GetMockDbSet(chargeStations);

            _mockContext.Setup(c => c.ChargeStations).Returns(mockSet.Object);

            var controller = new ChargeStationController(_mockContext.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ChargeStation>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Test_Create_Get_ReturnsViewResultWithGroups()
        {
            // Arrange
            var groups = new List<Group>
            {
                new Group { Id = 1, Name = "Group 1" },
                new Group { Id = 2, Name = "Group 2" }
            }.AsQueryable();

            var mockGroupSet = GetMockDbSet(groups);
            _mockContext.Setup(c => c.Groups).Returns(mockGroupSet.Object);

            var controller = new ChargeStationController(_mockContext.Object);

            // Act
            var result = controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewGroups = Assert.IsAssignableFrom<IEnumerable<Group>>(viewResult.ViewData["Groups"]);
            Assert.Equal(2, viewGroups.Count());
        }

        [Fact]
        public async Task Test_Create_ValidModelState_RedirectsToIndex()
        {
            // Arrange
            var groups = new List<Group>
            {
                new Group { Id = 1, Name = "Group 1" },
                new Group { Id = 2, Name = "Group 2" }
            }.AsQueryable();

            var mockGroupSet = GetMockDbSet(groups);
            _mockContext.Setup(c => c.Groups).Returns(mockGroupSet.Object);

            var chargeStation = new ChargeStation { Name = "CS New", GroupId = 1 };
            _mockContext.Setup(c => c.ChargeStations.Add(chargeStation)).Callback(() => chargeStations.Add(chargeStation));

            var controller = new ChargeStationController(_mockContext.Object);

            // Act
            var result = await controller.Create(chargeStation);

            // Assert
            _mockContext.Verify(c => c.ChargeStations.Add(chargeStation), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Test_Create_InvalidModelState_ReturnsViewWithModelAndGroups()
        {
            // Arrange
            var groups = new List<Group>
            {
                new Group { Id = 1, Name = "Group 1" },
                new Group { Id = 2, Name = "Group 2" }
            }.AsQueryable();

            var mockGroupSet = GetMockDbSet(groups);
            _mockContext.Setup(c => c.Groups).Returns(mockGroupSet.Object);

            var controller = new ChargeStationController(_mockContext.Object);
            var chargeStation = new ChargeStation { Name = "CS Invalid", GroupId = 1 };
            controller.ModelState.AddModelError("Name", "Invalid Name");

            // Act
            var result = await controller.Create(chargeStation);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ChargeStation>(viewResult.Model);
            Assert.Equal(chargeStation.Name, model.Name);

            var viewGroups = Assert.IsAssignableFrom<IEnumerable<Group>>(viewResult.ViewData["Groups"]);
            Assert.Equal(2, viewGroups.Count());
        }

        [Fact]
        public async Task Test_Edit_Get_ReturnsViewResultWithModelAndGroups()
        {
            // Arrange
            var groups = new List<Group>
            {
                new Group { Id = 1, Name = "Group 1" },
                new Group { Id = 2, Name = "Group 2" }
            }.AsQueryable();

            var chargeStation = new ChargeStation { Id = 1, Name = "CS 1", GroupId = 1 };
            _mockContext.Setup(c => c.ChargeStations.FindAsync(chargeStation.Id)).ReturnsAsync(chargeStation);

            var mockGroupSet = GetMockDbSet(groups);
            _mockContext.Setup(c => c.Groups).Returns(mockGroupSet.Object);

            var controller = new ChargeStationController(_mockContext.Object);

            // Act
            var result = await controller.Edit(chargeStation.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ChargeStation>(viewResult.Model);
            Assert.Equal(chargeStation.Name, model.Name);

            var viewGroups = Assert.IsAssignableFrom<IEnumerable<Group>>(viewResult.ViewData["Groups"]);
            Assert.Equal(2, viewGroups.Count());
        }

        [Fact]
        public async Task Test_Edit_ValidModelState_RedirectsToIndex()
        {
            // Arrange
            var groups = new List<Group>
            {
                new Group { Id = 1, Name = "Group 1" },
                new Group { Id = 2, Name = "Group 2" }
            }.AsQueryable();

            var mockGroupSet = GetMockDbSet(groups);
            _mockContext.Setup(c => c.Groups).Returns(mockGroupSet.Object);

            var chargeStation = new ChargeStation { Id = 1, Name = "CS 1", GroupId = 1 };
            _mockContext.Setup(c => c.ChargeStations.FindAsync(chargeStation.Id)).ReturnsAsync(chargeStation);

            var controller = new ChargeStationController(_mockContext.Object);

            // Act
            var result = await controller.Edit(chargeStation.Id, chargeStation);

            // Assert
            _mockContext.Verify(c => c.Update(chargeStation), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }


        [Fact]
        public async Task Test_DeleteConfirmed_DeletesAndRedirectsToIndex()
        {
            // Arrange
            var chargeStation = new ChargeStation { Id = 1, Name = "CS 1", GroupId = 1 };
            _mockContext.Setup(c => c.ChargeStations.FindAsync(chargeStation.Id)).ReturnsAsync(chargeStation);

            var controller = new ChargeStationController(_mockContext.Object);

            // Act
            var result = await controller.DeleteConfirmed(chargeStation.Id);

            // Assert
            _mockContext.Verify(c => c.ChargeStations.Remove(chargeStation), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        // ... (other test methods)

        private List<ChargeStation> chargeStations = new List<ChargeStation>();

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

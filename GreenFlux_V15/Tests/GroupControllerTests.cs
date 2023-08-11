using System.Collections.Generic;
using System.Linq;
using GreenFlux_V15.Controllers;
using GreenFlux_V15.Data;
using GreenFlux_V15.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace GreenFlux_V15.Tests.GroupTests
{
    public class GroupControllerTests
    {
        private readonly Mock<ChargingDbContext> _mockContext;

        public GroupControllerTests()
        {
            _mockContext = new Mock<ChargingDbContext>();
        }

        [Fact]
        public async void Test_Index_ReturnsViewResultWithListOfGroups()
        {
            // Arrange
            var groups = new List<Group>
            {
                new Group { Id = 1, Name = "Group 1" },
                new Group { Id = 2, Name = "Group 2" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Group>>();
            mockSet.As<IQueryable<Group>>().Setup(m => m.Provider).Returns(groups.Provider);
            mockSet.As<IQueryable<Group>>().Setup(m => m.Expression).Returns(groups.Expression);
            mockSet.As<IQueryable<Group>>().Setup(m => m.ElementType).Returns(groups.ElementType);
            mockSet.As<IQueryable<Group>>().Setup(m => m.GetEnumerator()).Returns(groups.GetEnumerator());

            _mockContext.Setup(c => c.Groups).Returns(mockSet.Object);

            var controller = new GroupController(_mockContext.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Group>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async void Test_Create_ValidModelState_RedirectsToIndex()
        {
            // Arrange
            var controller = new GroupController(_mockContext.Object);
            var newGroup = new Group { Name = "New Group", CapacityInAmps = 100 };

            // Act
            var result = await controller.Create(newGroup);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async void Test_Create_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var controller = new GroupController(_mockContext.Object);
            var newGroup = new Group { Name = "Invalid Group", CapacityInAmps = -50 };
            controller.ModelState.AddModelError("CapacityInAmps", "Invalid Capacity");

            // Act
            var result = await controller.Create(newGroup);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Group>(viewResult.Model);
            Assert.Equal(newGroup.Name, model.Name);
            Assert.Equal(newGroup.CapacityInAmps, model.CapacityInAmps);
        }

        [Fact]
        public async void Test_Edit_ReturnsViewWithGroupModel()
        {
            // Arrange
            var groupId = 1;
            var group = new Group { Id = groupId, Name = "Group 1" };
            _mockContext.Setup(c => c.Groups.FindAsync(groupId)).ReturnsAsync(group);

            var controller = new GroupController(_mockContext.Object);

            // Act
            var result = await controller.Edit(groupId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Group>(viewResult.ViewData.Model);
            Assert.Equal(group, model);
        }

        [Fact]
        public async void Test_Edit_InvalidGroupId_ReturnsNotFound()
        {
            // Arrange
            var groupId = 999; // Invalid group ID
            _mockContext.Setup(c => c.Groups.FindAsync(groupId)).ReturnsAsync((Group)null);

            var controller = new GroupController(_mockContext.Object);

            // Act
            var result = await controller.Edit(groupId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Test_Edit_ValidModelState_RedirectsToIndex()
        {
            // Arrange
            var controller = new GroupController(_mockContext.Object);
            var group = new Group { Id = 1, Name = "Updated Group", CapacityInAmps = 200 };

            // Act
            var result = await controller.Edit(group.Id, group);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async void Test_Edit_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var controller = new GroupController(_mockContext.Object);
            var group = new Group { Id = 1, Name = "Invalid Group", CapacityInAmps = -50 };
            controller.ModelState.AddModelError("CapacityInAmps", "Invalid Capacity");

            // Act
            var result = await controller.Edit(group.Id, group);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Group>(viewResult.Model);
            Assert.Equal(group.Name, model.Name);
            Assert.Equal(group.CapacityInAmps, model.CapacityInAmps);
        }


        [Fact]
        public async void Test_Delete_ReturnsViewWithGroupModel()
        {
            // Arrange
            var groupId = 1;
            var group = new Group { Id = groupId, Name = "Group 1" };
            _mockContext.Setup(c => c.Groups.FindAsync(groupId)).ReturnsAsync(group);

            var controller = new GroupController(_mockContext.Object);

            // Act
            var result = await controller.Delete(groupId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Group>(viewResult.ViewData.Model);
            Assert.Equal(group, model);
        }

        [Fact]
        public async void Test_Delete_InvalidGroupId_ReturnsNotFound()
        {
            // Arrange
            var groupId = 999; // Invalid group ID
            _mockContext.Setup(c => c.Groups.FindAsync(groupId)).ReturnsAsync((Group)null);

            var controller = new GroupController(_mockContext.Object);

            // Act
            var result = await controller.Delete(groupId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Test_DeleteConfirmed_DeletesGroupAndRedirectsToIndex()
        {
            // Arrange
            var groupId = 1;
            var group = new Group { Id = groupId, Name = "Group 1" };
            _mockContext.Setup(c => c.Groups.FindAsync(groupId)).ReturnsAsync(group);

            var controller = new GroupController(_mockContext.Object);

            // Act
            var result = await controller.DeleteConfirmed(groupId);

            // Assert
            _mockContext.Verify(c => c.Groups.Remove(group), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async void Test_Details_ReturnsViewWithGroupModel()
        {
            // Arrange
            var groupId = 1;
            var group = new Group { Id = groupId, Name = "Group 1" };
            _mockContext.Setup(c => c.Groups.FindAsync(groupId)).ReturnsAsync(group);

            var controller = new GroupController(_mockContext.Object);

            // Act
            var result = await controller.Details(groupId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Group>(viewResult.ViewData.Model);
            Assert.Equal(group, model);
        }

        // Add more test methods for other actions, edge cases, and validations...

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

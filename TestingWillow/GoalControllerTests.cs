using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WillowV2.Controllers;
using WillowV2.Models.ViewModels;

namespace TestingWillow
{
    public class GoalControllerTests
    {
        [Fact]
        public async Task Add_ValidGoal_SavesToDatabase()
        {
            using var context = TestHelpers.CreateContext();
            var controller = new GoalController(context);
            controller.SetFakeUser();

            var viewModel = new AddGoalViewModel
            {
                Name = "Emergency Fund",
                AmountToReach = 1000,
                CurrentAmount = 100,
                CurrentDate = DateTime.Today,
                Deadline = DateTime.Today.AddMonths(6)
            };

            var result = await controller.Add(viewModel);

            Assert.IsType<OkResult>(result);
            var saved = await context.Goals.FirstOrDefaultAsync();
            Assert.NotNull(saved);
            Assert.Equal("Emergency Fund", saved!.Name);
        }

        [Fact]
        public async Task Add_MissingName_ReturnsBadRequest()
        {
            using var context = TestHelpers.CreateContext();
            var controller = new GoalController(context);
            controller.SetFakeUser();

            var viewModel = new AddGoalViewModel { Name = "", AmountToReach = 500, Deadline = DateTime.Today.AddMonths(3) };
            controller.ModelState.AddModelError("Name", "Name is required");

            var result = await controller.Add(viewModel);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}

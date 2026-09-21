using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WillowV2.Controllers;
using WillowV2.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace TestingWillow
{
    public class TransactionControllerTests
    {
        [Fact]
        public async Task Add_MissingName_ReturnsBadRequest()
        {
            using var context = TestHelpers.CreateContext();
            var controller = new TransactionController(context);
            controller.SetFakeUser();

            var viewModel = new AddTransactionViewModel { Name = "", Amount = 20, CategoryId = Guid.NewGuid(), Date = DateTime.Today };
            controller.ModelState.AddModelError("Name", "Name is required");

            var result = await controller.Add(viewModel);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Add_ValidTransaction_SavesToDatabase()
        {
            using var context = TestHelpers.CreateContext();
            var controller = new TransactionController(context);
            controller.SetFakeUser();

            var viewModel = new AddTransactionViewModel
            {
                Name = "Groceries",
                Amount = 45.5f,
                CategoryId = Guid.NewGuid(),
                Date = DateTime.Today
            };

            var result = await controller.Add(viewModel);

            Assert.IsType<OkResult>(result);
            var saved = await context.FinTransactions.FirstOrDefaultAsync();
            Assert.NotNull(saved);
            Assert.Equal("Groceries", saved!.Name);
            Assert.Equal(TestHelpers.TestUserId, saved.UserId);
        }
    }
}

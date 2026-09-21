using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WillowProject.Controllers;
using WillowProject.Models;

namespace TestingWillow
{
    public class SplitControllerTests
    {
        [Fact]
        public async Task Add_MissingParticipants_ReturnsBadRequest()
        {
            using var context = TestHelpers.CreateContext();
            var controller = new SplitController(context);
            controller.SetFakeUser();

            var viewModel = new AddSplitViewModel
            {
                Name = "Dinner",
                TotalAmount = 60,
                Date = DateTime.Today,
                Participants = new List<AddSplitViewModel.ParticipantViewModel>()
            };

            // simulatinm what [MinLength(1)] would produce during real model binding
            controller.ModelState.AddModelError("Participants", "At least one participant is required!");

            var result = await controller.Add(viewModel);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Add_ValidSplit_SavesWithParticipants()
        {
            using var context = TestHelpers.CreateContext();
            var controller = new SplitController(context);
            controller.SetFakeUser();

            var viewModel = new AddSplitViewModel
            {
                Name = "Dinner-restorant",
                TotalAmount = 60,
                Date = DateTime.Today,
                Participants = new List<AddSplitViewModel.ParticipantViewModel>
                {
                    new AddSplitViewModel.ParticipantViewModel { Name = "Petar", AmountOwed = 30, AmountPaid = 0 },
                    new AddSplitViewModel.ParticipantViewModel { Name = "Misho", AmountOwed = 30, AmountPaid = 0 }
                }
            };

            var result = await controller.Add(viewModel);

            Assert.IsType<OkResult>(result);
            var saved = await context.Splits.Include(s => s.Participants).FirstOrDefaultAsync();
            Assert.NotNull(saved);
            Assert.Equal(2, saved!.Participants.Count);
        }
    }
}

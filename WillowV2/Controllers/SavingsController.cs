using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WillowV2.Data;
using WillowV2.Models;
using WillowV2.Models.ViewModels;

namespace WillowV2.Controllers
{
    [Authorize]
    public class SavingsController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public SavingsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddSavingsViewModel viewModel)
        {
            //check if data is valid
            if (!ModelState.IsValid)
            {
                return BadRequest("Please fill in all required fields.");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ← from session
            var savings = new Savings
            {
                CurrentAmount = viewModel.AmountToAdd,
                UserId = userId
            };
            //save to dbContext
            await dbContext.Savings.AddAsync(savings);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Withdraw(AddSavingsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please fill in all required fields.");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var savings = new Savings
            {
                CurrentAmount = -viewModel.AmountToRemove,
                UserId = userId
            };
            await dbContext.Savings.AddAsync(savings);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WillowProject.Models;
using WillowV2.Data;
using WillowV2.Models;
using WillowV2.Models.ViewModels;
namespace WillowProject.Controllers
{
    [Authorize]
    public class SplitController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public SplitController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IActionResult> Split()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var splits = await dbContext.Splits
                .Include(s => s.Participants)
                .Where(s => s.UserId == userId)
                .ToListAsync();

            return View(splits);
        }
        [HttpGet]
        public async Task<IActionResult> GetSplit(Guid id)
        {
            var split = await dbContext.Splits
                .Include(s => s.Participants)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (split == null) return NotFound();

            return Json(new
            {
                id = split.Id,
                name = split.Name,
                totalAmount = split.TotalAmount,
                date = split.Date.ToString("yyyy-MM-dd"),
                notes = split.Notes,
                participants = split.Participants.Select(p => new { id = p.Id, name = p.Name, amountOwed = p.AmountOwed, amountPaid = p.AmountPaid })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, AddSplitViewModel viewModel)
        {
            var split = await dbContext.Splits
                .Include(s => s.Participants)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (split == null) return NotFound();

            split.Name = viewModel.Name;
            split.TotalAmount = viewModel.TotalAmount;
            split.Date = viewModel.Date;
            split.Notes = viewModel.Notes;

            dbContext.SplitParticipants.RemoveRange(split.Participants);
            split.Participants = viewModel.Participants?.Select(p => new SplitParticipant
            {
                Name = p.Name,
                AmountOwed = p.AmountOwed,
                AmountPaid = p.AmountPaid
            }).ToList() ?? new List<SplitParticipant>();

            await dbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddSplitViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("At least one participant is required.");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var split = new Split
            {
                Name = viewModel.Name,
                TotalAmount = viewModel.TotalAmount,
                Date = viewModel.Date,
                Notes = viewModel.Notes,
                UserId = userId,
                Participants = viewModel.Participants?.Select(p => new SplitParticipant
                {
                    Name = p.Name,
                    AmountOwed = p.AmountOwed,
                    AmountPaid = p.AmountPaid
                }).ToList()
            };

            await dbContext.Splits.AddAsync(split);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> UpdatePayment(Guid id, decimal amountPaid)
        {
            var participant = await dbContext.SplitParticipants.FindAsync(id);
            if (participant == null) return NotFound();

            if (amountPaid < 0) return BadRequest("Amount can't be negative!");
            if (amountPaid > participant.AmountOwed) return BadRequest("Can't pay more than what's owed!");

            participant.AmountPaid = amountPaid;
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var split = await dbContext.Splits
                .Include(s => s.Participants)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (split != null)
            {
                dbContext.SplitParticipants.RemoveRange(split.Participants);
                dbContext.Splits.Remove(split);
                await dbContext.SaveChangesAsync();
            }
            return Ok();
        }
    }
}

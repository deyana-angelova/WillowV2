using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WillowV2.Data;
using WillowV2.Models;
using WillowV2.Models.Enums;

namespace WillowV2.Controllers
{
    [Authorize]
    public class RecurringTransactionsController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public RecurringTransactionsController(ApplicationDbContext db) { 
            this.dbContext = db;
        }
        //get current user
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await dbContext.FinCategories.Where(c => c.UserId == UserId).OrderBy(c => c.Name).ToListAsync();
            return View(await dbContext.RecurringTransactions
                .Include(r => r.Category)
                .Where(r => r.UserId == UserId)
                .OrderBy(r => r.DayOfMonth)
                .ToListAsync());
        }
        //add reccuring transaction
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(string name, float amount, int dayOfMonth, Guid categoryId
            )
        {
            //check if data is valid
            if (!ModelState.IsValid)
            {
                return BadRequest("Please fill in all required fields.");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ← from session
            var rec = new RecurringTransaction
            {
                Id = Guid.NewGuid(),
                Name = name,
                Amount = amount,            
                DayOfMonth = Math.Clamp(dayOfMonth, 1, 31), 
                CategoryId = categoryId,
                //Type = type,
                UserId = userId
            };
            await dbContext.RecurringTransactions.AddAsync(rec);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //find reccuring tr. to edit
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id) 
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var foundReccuring = await dbContext.RecurringTransactions.FindAsync(Id);
            if (foundReccuring == null) return NotFound();
            ViewBag.Categories = await dbContext.FinCategories
                .Where(c => c.UserId == UserId)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(foundReccuring);
        }
        //actually edit the reccuring tr.
        [HttpPost]
        public async Task<IActionResult> Edit (RecurringTransaction reccuring)
        {
            var editedReccuring = await dbContext.RecurringTransactions.FindAsync(reccuring.Id);
            if(editedReccuring is not null)
            {
                editedReccuring.Name = reccuring.Name;
                editedReccuring.Amount = reccuring.Amount;
                editedReccuring.DayOfMonth = reccuring.DayOfMonth;
                editedReccuring.CategoryId = reccuring.CategoryId;
                //editedReccuring.Category = reccuring.Category;
                //editedReccuring.Type = reccuring.Type;
                //editedReccuring.UserId = reccuring.UserId;
                editedReccuring.LastProcessed = reccuring.LastProcessed;

                await dbContext.SaveChangesAsync();
            }
            else
            {
                // if save fails validation later, view needs Categories populated too
                ViewBag.Categories = await dbContext.FinCategories.Where(c => c.UserId == UserId).OrderBy(c => c.Name).ToListAsync();
            }
            //syntax RedirectToAction(actionName, controllerName[only prexix]);
            return RedirectToAction("Index", "RecurringTransactions");
        }
        //remove reccuring tr. from db

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var rec = await dbContext.RecurringTransactions.FirstOrDefaultAsync(r => r.Id == id && r.UserId == UserId);
            if (rec != null)
            {
                dbContext.Remove(rec);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
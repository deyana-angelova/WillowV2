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
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public CategoryController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        public async Task<IActionResult> Index()
        {
            var categories = await dbContext.FinCategories
                .Where(c => c.UserId == UserId)
                .OrderBy(c => c.Type)
                .ThenBy(c => c.Name)
                .ToListAsync();
            return View(categories);
        }
        [HttpGet]
        //find the id for the category to edit
        public async Task<IActionResult> Edit(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var foundCategory = await dbContext.FinCategories.FindAsync(id);
            return View(foundCategory);
        }
        [HttpPost]
        //edit the transaction
        public async Task<IActionResult> Edit(FinCategory categoryToEdit)
        {
            var editedCategory = await dbContext.FinCategories.FindAsync(categoryToEdit.Id);
            if (editedCategory is not null)
            {
                editedCategory.Name = categoryToEdit.Name;
                editedCategory.Type = categoryToEdit.Type;

                await dbContext.SaveChangesAsync();
            }
            //syntax RedirectToAction(actionName, controllerName[only prexix]);
            return RedirectToAction("Index", "Category");
        }
        public async Task<IActionResult> Add(string name, TransactionType type)
        {
            //check if data is valid
            if (!ModelState.IsValid)
            {
                return BadRequest("Please fill in all required fields.");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ← from session
            var cat = new FinCategory
            {
                Name = name,
                Type = type,
                UserId = userId
            };
            await dbContext.FinCategories.AddAsync(cat);
            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(FinCategory category)
        {
            var categoryToDelete = await dbContext.FinCategories.FindAsync(category.Id);
            if (categoryToDelete is not null)
            {
                dbContext.Remove(categoryToDelete);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Category");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WillowV2.Data;
using WillowV2.Models;
using WillowV2.Models.ViewModels;
namespace WillowV2.Controllers
{
    [Authorize]
    public class GoalController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public GoalController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        //add goal to the dbContext
        public async Task<IActionResult> Add(AddGoalViewModel viewModel)
        {
            //check if data is valid
            if (!ModelState.IsValid)
            {
                return BadRequest("Please fill in all required fields.");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ← from session
            var goal = new Goal
            {
                Name = viewModel.Name,
                AmountToReach = viewModel.AmountToReach,
                CurrentAmount = viewModel.CurrentAmount,
                CurrentDate = viewModel.CurrentDate,
                Deadline = viewModel.Deadline,
                Notes = viewModel.Notes,
                UserId = userId
            };
            //save in dbContext
            await dbContext.Goals.AddAsync(goal);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpGet]
        //list all goals
        public async Task<IActionResult> ListGoals(float? neededFrom, float? neededTo, 
            DateTime? deadlineStart, DateTime? deadlineEnd)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = dbContext.Goals
                 .Where(t => t.UserId == userId);

            if (neededFrom.HasValue) { 
                query = query.Where(t => t.AmountToReach >= neededFrom);
            }
            if (neededTo.HasValue)
            {
                query = query.Where(t => t.AmountToReach <= neededTo);
            }
            if (deadlineStart.HasValue)
            {
                query = query.Where(t => t.Deadline >= deadlineStart);
            }
            if (deadlineEnd.HasValue)
            {
                query = query.Where(t => t.Deadline <= deadlineEnd);
            }

            var goals = await query.ToListAsync();
            ViewBag.Goals = await dbContext.Goals
                .ToListAsync();
            //filter
            ViewBag.SelectedNeededAmountFrom = neededFrom;
            ViewBag.SelectedNeededAmountTo = neededTo;
            ViewBag.SelectedDeadlineFrom = deadlineStart;
            ViewBag.SelectedDeadlineTo = deadlineEnd;
            return View(goals);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var foundGoal = await dbContext.Goals.FindAsync(Id);
            return View(foundGoal);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Goal goalToEdit)
        {
            var editedGoal = await dbContext.Goals.FindAsync(goalToEdit.Id);

            if(editedGoal is not null)
            {
                editedGoal.Name = goalToEdit.Name;
                editedGoal.AmountToReach = goalToEdit.AmountToReach;
                editedGoal.CurrentAmount = goalToEdit.CurrentAmount;
                editedGoal.CurrentDate = goalToEdit.CurrentDate;
                editedGoal.Deadline = goalToEdit.Deadline;

                await dbContext.SaveChangesAsync();
            }

            //syntax RedirectToAction(actionName, controllerName[only prexix]);
            return RedirectToAction("ListGoals", "Goal");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Goal goal)
        {
            var goalToDelete = await dbContext.Goals.FindAsync(goal.Id);

            if (goalToDelete is not null)
            {
                dbContext.Remove(goalToDelete);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("ListGoals", "Goal");
        }
        //update goal's current amount
        [HttpPost]
        public async Task<IActionResult> AddToGoal(Guid goalId, float amount)
        {
            var goal = await dbContext.Goals.FindAsync(goalId);

            if (goal != null)
            {
                goal.CurrentAmount += amount;
            }
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

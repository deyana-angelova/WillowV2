using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WillowV2.Data;
using WillowV2.Models;
using WillowV2.Models.Enums;
using WillowV2.Models.ViewModels;

namespace WillowV2.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        public TransactionController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        //method for adding a transaction
        public async Task<IActionResult> Add(AddTransactionViewModel viewModel)
        {
            //check if data is valid
            if (!ModelState.IsValid)
            {
                return BadRequest("Please fill in all required fields.");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ← from session
            var finTransaction = new FinTransaction
            {
                Name = viewModel.Name,
                Amount = viewModel.Amount,
                Date = viewModel.Date,
                CategoryId = viewModel.CategoryId,
                Notes = viewModel.Notes,
                UserId = userId
            };

            //save to dbContext
            await dbContext.FinTransactions.AddAsync(finTransaction);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpGet]
        //find the id for the transaction to edit
        public async Task<IActionResult> Edit (Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.Categories = await dbContext.FinCategories
            .Where(c => c.UserId == userId)
            .ToListAsync();
            var foundTransaction = await dbContext.FinTransactions.FindAsync(id);
            return View(foundTransaction);
        }
        [HttpPost]
        //edit the transaction
        public async Task<IActionResult> Edit(FinTransaction transactionToEdit) 
        {
            var editedTransaction = await dbContext.FinTransactions.FindAsync(transactionToEdit.Id);
            if (editedTransaction is not null) {
                editedTransaction.Name = transactionToEdit.Name;
                editedTransaction.Amount = transactionToEdit.Amount;
                editedTransaction.Date = transactionToEdit.Date;
                editedTransaction.CategoryId = transactionToEdit.CategoryId;
                editedTransaction.Notes = transactionToEdit.Notes;

                await dbContext.SaveChangesAsync();
            }
            //syntax RedirectToAction(actionName, controllerName[only prexix]);
            return RedirectToAction("ListTransactions", "Transaction");
        }
        [HttpGet]
        //list all transactions
        public async Task<IActionResult> ListTransactions(TransactionType? type, string? category,
            DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = dbContext.FinTransactions
                .Where(t => t.UserId == userId);
                //.Include(t => t.Category); //load the related category           

            if (type.HasValue)
            {
                query = query.Where(t => t.Category.Type == type.Value);
            }
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(t => t.Category.Id.ToString() == category);
            }
            if (startDate.HasValue)
            {
                query = query.Where(t => t.Date >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(t => t.Date <= endDate.Value);
            }
            var transactions = await query
                .Include(t => t.Category)
                .ToListAsync();
            ViewBag.Categories = await dbContext.FinCategories
                .Where(c => c.UserId == userId)
                .ToListAsync();
            //filter
            ViewBag.SelectedType = type;
            ViewBag.StartDate = startDate; 
            ViewBag.EndDate = endDate;
            return View(transactions);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(FinTransaction transaction)
        {
            var transactionToDelete = await dbContext.FinTransactions.FindAsync(transaction.Id);
            if(transactionToDelete is not null)
            {
                dbContext.Remove(transactionToDelete);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("ListTransactions", "Transaction");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using WillowV2.Data;
using WillowV2.Models;
using WillowV2.Models.Enums;

namespace WillowV2.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<User> _userManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            _logger = logger;
            this.dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.DisplayName = currentUser?.DisplayName;
            //all transactions of logged user
            var transactions = await dbContext.FinTransactions
                .Where(t => t.UserId == userId)
                .Include(t => t.Category) // joins category table 
                .ToListAsync();

            //all goals of logged user
            var goals = await dbContext.Goals
                 .Where(g => g.UserId == userId)
                .ToListAsync();

            //sum of all transactions - income
            ViewBag.Totalncome = transactions
                .Where(t => t.Category.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            //sum of all transactions - expense
            ViewBag.TotalExpenses = transactions
                .Where(t => t.Category.Type == TransactionType.Expense)
                .Sum(t => t.Amount);
            var savings = await dbContext.Savings
               .Where(s => s.UserId == userId)
               .ToListAsync();

            ViewBag.Totalsavings = savings
                .Sum(s => s.CurrentAmount);
            //current balance: income - expenses - savings
            ViewBag.CurrentBalance = (decimal)ViewBag.Totalncome - (decimal)ViewBag.TotalExpenses - (decimal)ViewBag.Totalsavings;

            //current savings 
            ViewBag.Savings = (decimal)ViewBag.Totalsavings;

            ViewBag.Goals = goals;

            ViewBag.Categories = await dbContext.FinCategories
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var predictions = goals.ToDictionary(g => g.Id, g => GoalPrediction.Calculate(g));
            ViewBag.Predictions = predictions;


            var expensesByCategory = transactions
               .Where(t => t.Category != null &&
                           t.Category.Type == TransactionType.Expense)
               .GroupBy(t => t.Category.Name)
               .Select(g => new
               {
                   Category = g.Key,
                   Amount = g.Sum(t => t.Amount)
               })
               .OrderByDescending(x => x.Amount)
               .ToList();

            ViewBag.ExpenseCategories = expensesByCategory
                .Select(x => x.Category)
                .ToList();

            ViewBag.ExpenseAmounts = expensesByCategory
                .Select(x => x.Amount)
                .ToList();
            // Income by category
            var incomeByCategory = transactions
                .Where(t => t.Category != null &&
                            t.Category.Type == TransactionType.Income)
                .GroupBy(t => t.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Amount = g.Sum(t => t.Amount)
                })
                .OrderByDescending(x => x.Amount)
                .ToList();

            ViewBag.IncomeCategories = incomeByCategory
                .Select(x => x.Category)
                .ToList();

            ViewBag.IncomeAmounts = incomeByCategory
                .Select(x => x.Amount)
                .ToList();
            // Expenses by month
            var expensesByMonth = transactions
                .Where(t => t.Category != null &&
                            t.Category.Type == TransactionType.Expense)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Amount = g.Sum(t => t.Amount)
                })
                .OrderBy(x => x.Month)
                .ToList();

            ViewBag.ExpenseMonths = expensesByMonth
                .Select(x => x.Month.ToString("MMM yyyy"))
                .ToList();

            ViewBag.ExpenseMonthAmounts = expensesByMonth
                .Select(x => x.Amount)
                .ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> Analytics()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var transactions = await dbContext.FinTransactions
                .Where(t => t.UserId == userId)
                .Include(t => t.Category)
                .ToListAsync();

            var expensesByCategory = transactions
                .Where(t => t.Category != null &&
                            t.Category.Type == TransactionType.Expense)
                .GroupBy(t => t.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Amount = g.Sum(t => t.Amount)
                })
                .OrderByDescending(x => x.Amount)
                .ToList();

            ViewBag.ExpenseCategories = expensesByCategory
                .Select(x => x.Category)
                .ToList();

            ViewBag.ExpenseAmounts = expensesByCategory
                .Select(x => x.Amount)
                .ToList();
            // Income by category
            var incomeByCategory = transactions
                .Where(t => t.Category != null &&
                            t.Category.Type == TransactionType.Income)
                .GroupBy(t => t.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Amount = g.Sum(t => t.Amount)
                })
                .OrderByDescending(x => x.Amount)
                .ToList();

            ViewBag.IncomeCategories = incomeByCategory
                .Select(x => x.Category)
                .ToList();

            ViewBag.IncomeAmounts = incomeByCategory
                .Select(x => x.Amount)
                .ToList();
            // Expenses by month
            var expensesByMonth = transactions
                .Where(t => t.Category != null &&
                            t.Category.Type == TransactionType.Expense)
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Amount = g.Sum(t => t.Amount)
                })
                .OrderBy(x => x.Month)
                .ToList();

            ViewBag.ExpenseMonths = expensesByMonth
                .Select(x => x.Month.ToString("MMM yyyy"))
                .ToList();

            ViewBag.ExpenseMonthAmounts = expensesByMonth
                .Select(x => x.Amount)
                .ToList();
            return View("~/Views/Home/Analytics.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private async Task<List<string>> ProcessReccuringTransactions(string userId)
        {
            var applied = new List<string>();
            var today = DateTime.Today;

            var recurring = await dbContext.RecurringTransactions
                .Where(t => t.UserId == userId)
                .ToListAsync();
            foreach (var r in recurring) {
                //check if its time for reccuring transaction
                var isDue = today.Day >= r.DayOfMonth;
            }
            return applied;
        } 
    }
}


using Microsoft.EntityFrameworkCore;
using WillowV2.Data;
using WillowV2.Models;
using WillowV2.Models.Enums;

namespace WillowV2.Services
{
    public class RecurringTransactionService : IRecurringTransactionService
    {
        private readonly ApplicationDbContext dbContext;
        public RecurringTransactionService(ApplicationDbContext _dbContext)
        {
            this.dbContext = _dbContext;
        }
        //this method is used to process the transactions
        public async Task<int> ProcessRecurringTransactionsAsync(string userId)
        {
            //gets current date
            var today = DateTime.Today;
            //counts created transactions
            int createdCount = 0;
            //getting user's rec. transactions
            var recurring = await dbContext.RecurringTransactions
                .Include(r => r.Category)
                .Where(r => r.UserId == userId)
                .ToListAsync();
            //looping through all rec. transactions
            foreach(var r in recurring)
            {
                //get due date for transaction
                var dueDates = GetDueOccurences(r, today);

                //loop through the due dates of all r.tr.
                foreach (var dueDate in dueDates)
                {
                    //create a transaction
                    var transaction = CreateTransactionFrom(r, dueDate);
                    //add to db
                    dbContext.FinTransactions.Add(transaction);
                    createdCount++;
                }
                if (dueDates.Any())
                {
                    r.LastProcessed = today;
                }              
            }
            await dbContext.SaveChangesAsync();

            return createdCount;
        }
        //method to return a FinTransaction
        private FinTransaction CreateTransactionFrom(RecurringTransaction r, DateTime date) 
        {
            //positive or negative amnt depending on tr. type
            var signedAmount = r.Category?.Type == TransactionType.Expense
            ? -Math.Abs(r.Amount)
            : Math.Abs(r.Amount);

            var transaction = new FinTransaction()
            {
                Id = Guid.NewGuid(),
                Name = r.Name,
                Amount = r.Amount,
                Date = date,
                Notes = "Auto-generated from recurring transaction",
                CategoryId = r.CategoryId,
                UserId = r.UserId
            };
            return transaction;
        }
        public List<DateTime> GetDueOccurences(RecurringTransaction r, DateTime today)
        {
            //create new list made of due dates
            var due = new List<DateTime>();

            //determine which month to start from to check for rec. tr.
            DateTime month = r.LastProcessed.HasValue
                ? new DateTime(r.LastProcessed.Value.Year, r.LastProcessed.Value.Month, 1).AddMonths(1)
                : new DateTime(today.Year, today.Month, 1);

            DateTime currentMonth = new DateTime(today.Year, today.Month, 1);

            //loop through months
            while (month <= currentMonth)
            {
                //handle months with different number of dates
                int lastDayOfMonth = DateTime.DaysInMonth(month.Year, month.Month);
                int day = Math.Min(r.DayOfMonth, lastDayOfMonth);

                DateTime dueDate = new DateTime(month.Year, month.Month, day);
                //if date has happened add to list
                if (dueDate <= today)
                {
                    due.Add(dueDate);
                }
                //move to next month
                month = month.AddMonths(1);
            }

            return due;
        }
    }
}   

using WillowV2.Models;
using WillowV2.Services;
using Xunit;
namespace TestingWillow
{
    public class GetDueOccurencesTests
    {
        private readonly RecurringTransactionService _service = new(null!);
        [Fact]
        public void ReturnsOneDate_WhenDayAlreadyPassedThisMonth()
        {
            var recurring = new RecurringTransaction { DayOfMonth = 5, LastProcessed = null };
            var today = new DateTime(2026, 9, 13);

            var result = _service.GetDueOccurences(recurring, today);

            Assert.Single(result);
            Assert.Equal(new DateTime(2026, 9, 5), result[0]);
        }
    }
}

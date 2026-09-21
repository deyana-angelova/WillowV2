namespace WillowV2.Services
{
    public interface IRecurringTransactionService
    {
        Task<int> ProcessRecurringTransactionsAsync(string userId);
    }
}

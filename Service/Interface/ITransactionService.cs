using BankingSystemweb.Models;
using BankingSystemweb.ViewModels;

namespace BankingSystemweb.Service.Interface
{
    public interface ITransactionService
    {
        Task<bool> CreateTransaction(TransactionVM model);
        Task<List<Transaction>> GetAllTransactions();
        // Returns total transaction count
        Task<int> GetTotalTransactionsAsync();

        Task<List<RecentActivityVM>> GetRecentActivitiesAsync(string adminId);

        Task<(decimal deposits, int depositCount, decimal withdrawals, int withdrawalCount)> GetTodaysTransactionsTotalsAsync();
    };
}
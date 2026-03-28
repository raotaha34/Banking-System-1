using BankingSystemweb.Data;
using BankingSystemweb.Models;
using BankingSystemweb.Service.Interface;
using BankingSystemweb.ViewModels;
using Microsoft.EntityFrameworkCore;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;

    public TransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Transaction>> GetAllTransactions()
    {
        return await _context.Transactions!
            .Include(t => t.Account)
            .ThenInclude(a => a.User)
            .ToListAsync();
    }

    public async Task<int> GetTotalTransactionsAsync()
    {
        return await _context.Transactions!
            .CountAsync();
    }

    public async Task<bool> CreateTransaction(TransactionVM model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var sender = await _context.Accounts!
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AccountId == model.AccountId);

        if (sender == null)
            throw new Exception("Sender account not found");

        if (model.TransactionType == "Withdraw" || model.TransactionType == "Transfer")
        {
            if (sender.Balance < model.Amount)
                throw new Exception("Insufficient balance");

            sender.Balance -= model.Amount;
        }

        if (model.TransactionType == "Deposit")
        {
            sender.Balance += model.Amount;
        }

        Account? receiver = null;

        if (model.TransactionType == "Transfer")
        {
            if (model.ReceiverAccountId == null)
                throw new Exception("Receiver Account is required");

            receiver = await _context.Accounts!
                .FirstOrDefaultAsync(a => a.AccountId == model.ReceiverAccountId);

            if (receiver == null)
                throw new Exception("Receiver not found");

            receiver.Balance += model.Amount;
        }

        var transaction = new Transaction
        {
            AccountId = model.AccountId,
            ReceiverAccountId = model.ReceiverAccountId,
            Amount = model.Amount,
            TransactionType = model.TransactionType,
            Description = model.Description,
            TransactionDate = DateTime.Now
        };

        _context.Transactions!.Add(transaction);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<RecentActivityVM>> GetRecentActivitiesAsync(int count = 10)
    {
        // 1️⃣ Transactions
        var transactionActivities = _context.Transactions!
            .Include(t => t.Account)
            .ThenInclude(a => a.User)
            .OrderByDescending(t => t.TransactionDate)
            .Take(count)
            .AsEnumerable()
            .Select(t => new RecentActivityVM
            {
                Title = t.TransactionType switch
                {
                    "Deposit" => "Deposit Received",
                    "Withdraw" => "Withdrawal Completed",
                    "Transfer" => "Transfer Completed",
                    _ => "Transaction"
                },
                Description = t.TransactionType switch
                {
                    "Deposit" => $"${t.Amount} — Account #{t.Account.AccountNumber}",
                    "Withdraw" => $"${t.Amount} — Account #{t.Account.AccountNumber}",
                    "Transfer" => $"${t.Amount} — To Account #{t.ReceiverAccountId}",
                    _ => $"${t.Amount}"
                },
                Icon = t.TransactionType switch
                {
                    "Deposit" => "D",
                    "Withdraw" => "W",
                    "Transfer" => "T",
                    _ => "T"
                },
                Color = t.TransactionType switch
                {
                    "Deposit" => "orange",
                    "Withdraw" => "red",
                    "Transfer" => "purple",
                    _ => "blue"
                },
                Time = t.TransactionDate
            })
            .ToList();

        // 2️⃣ Users
        var userActivities = _context.Users!
            .AsEnumerable()
            .Select(u => new RecentActivityVM
            {
                Title = "User Account Created",
                Description = $"{u.FullName} — {u.Email}",
                Icon = "U",
                Color = "blue",
                Time = u.CreatedAt
            })
            .ToList();

        // 3️⃣ Accounts
        var accountActivities = _context.Accounts!
            .Include(a => a.User)
            .AsEnumerable()
            .Select(a => new RecentActivityVM
            {
                Title = "Savings Account Opened",
                Description = $"Account #{a.AccountNumber} — Initial deposit ${a.Balance}",
                Icon = "A",
                Color = "green",
                Time = a.CreatedAt
            })
            .ToList();

        // 4️⃣ Merge & sort
        var allActivities = transactionActivities
            .Concat(userActivities)
            .Concat(accountActivities)
            .OrderByDescending(a => a.Time)
            .Take(count)
            .ToList();

        return allActivities;
    }
}
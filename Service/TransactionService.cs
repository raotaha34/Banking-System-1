using BankingSystemweb.Data;
using BankingSystemweb.Models;
using BankingSystemweb.Service.Interface;
using BankingSystemweb.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;

    private readonly UserManager<ApplicationUser> _userManager;

    public TransactionService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
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


    public async Task<List<RecentActivityVM>> GetRecentActivitiesAsync(string adminId)
    {
        // 🔹 Get Admin Info
        var adminUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == adminId);

        var adminName = adminUser?.FullName ?? "Admin";

        // 1️⃣ Transactions
        var transactionActivities = _context.Transactions!
            .Include(t => t.Account)
            .ThenInclude(a => a.User)
            .OrderByDescending(t => t.TransactionDate)
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
                    "Deposit" => $"${t.Amount} — {t.Account.User?.FullName ?? "Unknown"}",
                    "Withdraw" => $"${t.Amount} — {t.Account.User?.FullName ?? "Unknown"}",
                    "Transfer" => $"${t.Amount} — To Account #{t.ReceiverAccountId} (by {adminName})",
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
                    "Withdraw" => "yellow",
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
                Title = $"User Account Created by {adminName}",
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
                Title = $"Savings Account Opened by {adminName}",
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
            .ToList();

        return allActivities;
    }

    public async Task<(decimal deposits, int depositCount, decimal withdrawals, int withdrawalCount)> GetTodaysTransactionsTotalsAsync()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        // Today's deposits sum and count
        var depositsQuery = _context.Transactions!
            .Where(t => t.TransactionType == "Deposit"
                        && t.TransactionDate >= today
                        && t.TransactionDate < tomorrow);

        var deposits = await depositsQuery.SumAsync(t => t.Amount);
        var depositCount = await depositsQuery.CountAsync();

        // Today's withdrawals sum and count
        var withdrawalsQuery = _context.Transactions!
            .Where(t => t.TransactionType == "Withdraw"
                        && t.TransactionDate >= today
                        && t.TransactionDate < tomorrow);

        var withdrawals = await withdrawalsQuery.SumAsync(t => t.Amount);
        var withdrawalCount = await withdrawalsQuery.CountAsync();

        return (deposits, depositCount, withdrawals, withdrawalCount);
    }


    public async Task<bool> CreateTransactions(TransactionVM model, string userId)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        // 1️⃣ Get sender account based on logged-in user
        var sender = await _context.Accounts!
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (sender == null)
            throw new Exception("Sender account not found");

        // 2️⃣ Prepare transaction object
        var transaction = new Transaction
        {
            AccountId = sender.AccountId, // Sender is always current user
            Amount = model.Amount,
            TransactionType = model.TransactionType,
            Description = model.Description,
            TransactionDate = DateTime.Now
        };

        // 3️⃣ Handle transaction types
        switch (model.TransactionType)
        {
            case "Deposit":
                sender.Balance += model.Amount;
                transaction.ReceiverName = "N/A"; // No receiver
                break;

            case "Withdraw":
                if (sender.Balance < model.Amount)
                    throw new Exception("Insufficient balance");

                sender.Balance -= model.Amount;
                transaction.ReceiverName = "N/A"; // No receiver
                break;

            case "Transfer":
                if (model.ReceiverAccountId == null)
                    throw new Exception("Receiver account is required");

                var receiver = await _context.Accounts!
                    .Include(a => a.User) // to get FullName
                    .FirstOrDefaultAsync(a => a.AccountId == model.ReceiverAccountId);

                if (receiver == null)
                    throw new Exception("Receiver account not found");

                if (sender.Balance < model.Amount)
                    throw new Exception("Insufficient balance");

                // 4️⃣ Update balances
                sender.Balance -= model.Amount;
                receiver.Balance += model.Amount;

                // 5️⃣ Set receiver name
                transaction.ReceiverName = receiver.User?.FullName ?? "Unknown";
                transaction.ReceiverAccountId = receiver.AccountId;
                break;

            default:
                throw new Exception("Invalid transaction type");
        }

        // 6️⃣ Save transaction
        _context.Transactions!.Add(transaction);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<Transaction>> GetUserTransactions(string userId)
    {
        // Get the logged-in user's account
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        if (account == null) return new List<Transaction>();

        // Return transactions where the user is sender OR receiver
        return await _context.Transactions!
            .Where(t => t.AccountId == account.AccountId || t.ReceiverAccountId == account.AccountId)
            .Include(t => t.Account)           // sender
            .ThenInclude(a => a.User)
            .ToListAsync();
    }
}
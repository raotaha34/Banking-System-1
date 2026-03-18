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

    
    // Returns total transaction count
    public async Task<int> GetTotalTransactionsAsync()
    {
        return await _context.Transactions!.CountAsync();
    
}

    public async Task<bool> CreateTransaction(TransactionVM model)
    {
        ArgumentNullException.ThrowIfNull(model); // ✅ clean null check

        var sender = await _context.Accounts!
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AccountId == model.AccountId);

        if (sender == null)
            throw new Exception("Sender account not found");

        // ✅ Withdraw / Transfer
        if (model.TransactionType == "Withdraw" || model.TransactionType == "Transfer")
        {
            if (sender.Balance < model.Amount)
                throw new Exception("Insufficient balance");

            sender.Balance -= model.Amount;
        }

        // ✅ Deposit
        if (model.TransactionType == "Deposit")
        {
            sender.Balance += model.Amount;
        }

        Account? receiver = null;

        // ✅ Transfer logic
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

        // ✅ Save transaction
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
}
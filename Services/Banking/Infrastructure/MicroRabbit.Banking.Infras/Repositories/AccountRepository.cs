using System;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Banking.Domain.Models;
using MicroRabbit.Banking.Infras.Contexts;

namespace MicroRabbit.Banking.Infras.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly BankingDbContext _context;

    public AccountRepository(BankingDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Account> GetAccounts()
    {
        return _context.Accounts;
    }
}

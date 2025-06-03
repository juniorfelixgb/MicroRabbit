using System;
using Microsoft.EntityFrameworkCore;

namespace MicroRabbit.Transfer.Infra.Contexts;

public class TransferDbContext : DbContext
{
    public TransferDbContext(DbContextOptions<TransferDbContext> options) : base(options)
    {
    }


}

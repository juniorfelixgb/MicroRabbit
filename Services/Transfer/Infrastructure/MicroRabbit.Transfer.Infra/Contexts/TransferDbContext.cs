using System;
using MicroRabbit.Transfer.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroRabbit.Transfer.Infra.Contexts;

public class TransferDbContext(DbContextOptions<TransferDbContext> options) : DbContext(options)
{
    public DbSet<TransferLog> TransferLogs => Set<TransferLog>();
}

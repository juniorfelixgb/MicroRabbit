using System;

namespace MicroRabbit.Banking.Application.Models;

public class AccountTransfer
{
    public int From { get; set; }
    public int To { get; set; }
    public decimal Amount { get; set; }
}

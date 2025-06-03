using System;
using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Models;
using MicroRabbit.Banking.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicroRabbit.Banking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankingController : ControllerBase
{
    private readonly IAccountService _accountService;
    public BankingController(IAccountService accountService)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
    }

    [HttpGet]
    public ActionResult<IEnumerable<Account>> Get()
    {
        return Ok(_accountService.GetAccounts());
    }

    [HttpPost]
    public IActionResult Post([FromBody] AccountTransfer accountTransfer)
    {
        if (accountTransfer == null)
        {
            return BadRequest("Invalid account transfer data.");
        }

        _accountService.TransferFunds(accountTransfer);
        return Ok("Transfer completed successfully.");
    }
}

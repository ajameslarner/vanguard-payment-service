using Common.Queries;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Internal.Controllers;

/// <summary>
/// Query accounts within the payment service
/// </summary>
[ApiController]
[Route("api/accounts")]
[Produces("application/json")]
//[Authorize] - NOTE: Uncomment to enable auth on the payments endpoints, commented out for brevity
public class AccountsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Get account details for a specific account by account number
    /// </summary>
    /// <param name="accountNumber">The number of the account requested</param>
    /// <returns>Returns the resulting account according to the account number provided</returns>
    [HttpGet("{accountNumber}")]
    [ProducesResponseType(typeof(AccountModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccount(string accountNumber)
    {
        var response = await _mediator.Send(new GetAccountQuery(accountNumber));

        return response != null ? Ok(response) : NotFound($"Cannot find an account matching: {accountNumber}");
    }
}
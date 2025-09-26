using Common.Commands;
using Common.Queries;
using Domain.Enums;
using Domain.Models;
using Domain.Requests;
using Domain.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Internal.Controllers;

/// <summary>
/// Payment processing endpoints
/// </summary>
[ApiController]
[Route("api/payments")]
[Produces("application/json")]
//[Authorize] - NOTE: Uncomment to enable auth on the payments endpoints, commented out for brevity
public class PaymentsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Post payment requests from debtor to creditor accounts
    /// </summary>
    /// <param name="request">The request body for the payment requested</param>
    /// <returns>A payment result containing the success criteria</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MakePaymentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MakePayment(MakePaymentRequest request)
    {
        if (request == null)
            return BadRequest($"Request body is not valid {nameof(MakePaymentRequest)} object.");

        if (!Enum.IsDefined(typeof(PaymentScheme), request.PaymentScheme))
            return BadRequest("The provided payment scheme is not supported.");
        
        var response = await _mediator.Send(new MakePaymentCommand(request));

        return response != null && response.Success ? Ok(response) : BadRequest(response.Details);
    }

    /// <summary>
    /// Get payment details for a specific transaction by transaction id
    /// </summary>
    /// <param name="transactionId">The id for the corresponding transaction</param>
    /// <returns>The payment details for the corresponding transaction</returns>
    [HttpGet("{transactionId}")]
    [ProducesResponseType(typeof(PaymentModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayment(Guid transactionId)
    {
        var response = await _mediator.Send(new GetPaymentQuery(transactionId));

        return response != null ? Ok(response) : NotFound($"Payment with transaction id {transactionId} not found.");
    }
}
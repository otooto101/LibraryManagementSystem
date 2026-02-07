using API.Infrastructure.Extensions;
using API.Models.Request;
using API.Models.Response;
using API.Swagger.Examples.Borrow;
using Application.DTOs.Borrowing;
using Application.Interfaces;
using Application.Models;
using Asp.Versioning;
using Azure.Core;
using Domain.Constants;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace API.Controllers
{
    /// <summary>
    /// Manages borrowing operations (Checkout, Return, History).
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class BorrowRecordsController(
        IBorrowService borrowService,
        IHashIdService hashIdService
        ) : ControllerBase
    {
        /// <summary>
        /// Retrieves a paginated history of borrow records.
        /// </summary>
        /// <remarks>
        /// Use this to see who borrowed what, and filtering by active/overdue status.
        /// </remarks>
        /// <param name="searchParams">Filtering options (e.g., UserId, Overdue only).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A paged list of borrow records.</returns>
        [HttpGet]
        [Authorize(Roles = Roles.Admin + "," + Roles.Librarian)]
        [ProducesResponseType(typeof(PagedResult<BorrowResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<BorrowResponse>>> GetAll(
            [FromQuery] BorrowSearchRequest request,
            CancellationToken ct)
        {
            var searchParams = request.Adapt<BorrowSearchParameters>();

            var dtos = await borrowService.GetPagedBorrowsAsync(searchParams, ct);
            var response = dtos.Adapt<PagedResult<BorrowResponse>>();

            foreach (var borrow in response.Items)
            {
                borrow.AddStandardLinks(HttpContext, Url, borrow.Id,
                    getRouteName: nameof(GetBorrowRecordById),
                    updateRouteName: nameof(ReturnBook));
            }

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a single borrow record by ID.
        /// </summary>
        /// <param name="id">The unique ID of the borrow record.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The borrow details.</returns>
        [HttpGet("{id}", Name = nameof(GetBorrowRecordById))]
        [Authorize(Roles = Roles.Admin + "," + Roles.Librarian)]
        [ProducesResponseType(typeof(BorrowResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BorrowResponseExample))]
        public async Task<ActionResult<BorrowResponse>> GetBorrowRecordById(string id, CancellationToken ct)
        {
            var internalId = hashIdService.Decode(id);

            var dto = await borrowService.GetByIdAsync(internalId, ct);
            var response = dto.Adapt<BorrowResponse>();

            response.AddStandardLinks(HttpContext, Url, response.Id,
                getRouteName: nameof(GetBorrowRecordById),
                deleteRouteName: null,
                updateRouteName: nameof(ReturnBook));

            return Ok(response);
        }

        /// <summary>
        /// Checks out a book for a patron.
        /// </summary>
        /// <remarks>
        /// This creates a new borrow record and decreases the book's stock quantity.
        /// </remarks>
        /// <param name="checkoutDto">The patron and book details.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The newly created borrow record.</returns>
        [HttpPost]
        [Authorize(Roles = Roles.Admin + "," + Roles.Librarian)]
        [ProducesResponseType(typeof(BorrowResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerRequestExample(typeof(BorrowBookDto), typeof(BorrowBookExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(BorrowResponseExample))]
        public async Task<ActionResult<BorrowResponse>> Checkout(
            [FromBody] BorrowBookDto checkoutDto,
            CancellationToken ct)
        {
            var dto = await borrowService.CheckoutBookAsync(checkoutDto, ct);
            var response = dto.Adapt<BorrowResponse>();

            response.AddStandardLinks(HttpContext, Url, response.Id,
                getRouteName: nameof(GetBorrowRecordById),
                deleteRouteName: null,
                updateRouteName: nameof(ReturnBook));

            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

            return CreatedAtRoute(
                routeName: nameof(GetBorrowRecordById),
                routeValues: new { id = response.Id, version = version },
                value: response);
        }

        /// <summary>
        /// Returns a borrowed book.
        /// </summary>
        /// <remarks>
        /// Marks the record as returned and restores the book's stock quantity.
        /// </remarks>
        /// <param name="id">The ID of the borrow record to close.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}/return", Name = nameof(ReturnBook))]
        [Authorize(Roles = Roles.Admin + "," + Roles.Librarian)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReturnBook(string id, CancellationToken ct)
        {
            var internalId = hashIdService.Decode(id);

            await borrowService.ReturnAsync(internalId, ct);
            return NoContent();
        }

        /// <summary>
        /// Retrieves a list of all overdue borrow records.
        /// </summary>
        [HttpGet("overdue")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Librarian)]
        [ProducesResponseType(typeof(PagedResult<BorrowResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<BorrowResponse>>> GetOverdue(
            [FromQuery] PagedSearchParameters pagedParams,
            CancellationToken ct)
        {
            var records = await borrowService.GetOverdueRecordsAsync(pagedParams, ct);
            return Ok(records.Adapt<PagedResult<BorrowResponse>>());
        }

        /// <summary>
        /// Deletes a borrow record.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var internalId = hashIdService.Decode(id);

            await borrowService.DeleteRecordAsync(internalId, ct);
            return NoContent();
        }
    }
}
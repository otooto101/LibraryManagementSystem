using API.Infrastructure.Extensions;
using API.Models.Response;
using API.Swagger.Examples.Patron;
using Application.DTOs.Patron;
using Application.Interfaces;
using Application.Models;
using Asp.Versioning;
using Domain.Constants;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace API.Controllers
{
    /// <summary>
    /// Manages operations for library Patrons (members).
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class PatronsController(
        IPatronService patronService,
        IBookService bookService,
        IHashIdService hashIdService
        ) : ControllerBase
    {
        /// <summary>
        /// Retrieves a paginated list of patrons.
        /// </summary>
        /// <remarks>
        /// Use this to search for members by name or email.
        /// </remarks>
        /// <param name="searchParams">Filtering options (Name, Email, etc.).</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A paged list of patrons.</returns>
        [HttpGet]
        [Authorize(Roles = Roles.Admin + "," + Roles.Librarian)]
        [ProducesResponseType(typeof(PagedResult<PatronResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<PatronResponse>>> GetAll(
            [FromQuery] PatronSearchParameters searchParams,
            CancellationToken ct)
        {
            var dtos = await patronService.GetPagedPatronsAsync(searchParams, ct);
            var response = dtos.Adapt<PagedResult<PatronResponse>>();

            foreach (var patron in response.Items)
            {
                patron.AddStandardLinks(HttpContext, Url, patron.PatronId,
                    getRouteName: nameof(GetPatronById),
                    deleteRouteName: nameof(DeletePatron),
                    updateRouteName: nameof(UpdatePatron));
            }

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a single patron by their unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the patron.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The patron details.</returns>
        [HttpGet("{id}", Name = nameof(GetPatronById))]
        [Authorize(Roles = Roles.Admin + "," + Roles.Librarian)]
        [ProducesResponseType(typeof(PatronResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PatronResponseExample))]
        public async Task<ActionResult<PatronResponse>> GetPatronById(string id, CancellationToken ct)
        {
            var internalId = hashIdService.Decode(id);

            var dto = await patronService.GetByIdAsync(internalId, ct);
            var response = dto.Adapt<PatronResponse>();

            response.AddStandardLinks(HttpContext, Url, response.PatronId,
                getRouteName: nameof(GetPatronById),
                deleteRouteName: nameof(DeletePatron),
                updateRouteName: nameof(UpdatePatron));

            return Ok(response);
        }

        /// <summary>
        /// Retrieves all books currently borrowed (or history) by a specific patron.
        /// </summary>
        /// <param name="id">The Patron ID.</param>
        /// <param name="pagedSearchParametersRequest">Pagination options.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of books.</returns>
        [HttpGet("{id}/books")]
        [Authorize(Roles = Roles.Admin + "," + Roles.Librarian)]
        [ProducesResponseType(typeof(PagedResult<BookResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult<BookResponse>>> GetBorrowedBooks(
            [FromRoute] string id,
            [FromQuery] PagedSearchParameters pagedSearchParametersRequest,
            CancellationToken ct)
        {
            var internalId = hashIdService.Decode(id);

            var bookSearchParams = pagedSearchParametersRequest.Adapt<BookSearchParameters>();

            bookSearchParams.BorrowerId = internalId;

            var bookDtos = await bookService.GetPagedBooksAsync(bookSearchParams, ct);
            var response = bookDtos.Adapt<PagedResult<BookResponse>>();

            foreach (var book in response.Items)
            {
                book.AddStandardLinks(HttpContext, Url, book.BookId,
                    getRouteName: "GetBookById",
                    deleteRouteName: "DeleteBook",
                    updateRouteName: "UpdateBook");
            }

            return Ok(response);
        }

        /// <summary>
        /// Registers a new patron.
        /// </summary>
        /// <param name="createDto">The patron registration details.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The newly created patron.</returns>
        [HttpPost]
        [Authorize(Roles = Roles.Admin + "," + Roles.Librarian)]
        [ProducesResponseType(typeof(PatronResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerRequestExample(typeof(PatronCreateDto), typeof(PatronCreateExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(PatronResponseExample))]
        public async Task<ActionResult<PatronResponse>> Create(
            [FromBody] PatronCreateDto createDto,
            CancellationToken ct)
        {
            var dto = await patronService.CreatePatronAsync(createDto, ct);
            var response = dto.Adapt<PatronResponse>();

            response.AddStandardLinks(HttpContext, Url, response.PatronId,
                getRouteName: nameof(GetPatronById),
                deleteRouteName: nameof(DeletePatron),
                updateRouteName: nameof(UpdatePatron));

            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

            return CreatedAtRoute(
                routeName: nameof(GetPatronById),
                routeValues: new { id = response.PatronId, version = version },
                value: response);
        }

        /// <summary>
        /// Updates an existing patron's information.
        /// </summary>
        /// <param name="id">The ID of the patron to update.</param>
        /// <param name="updateDto">The updated values.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}", Name = nameof(UpdatePatron))]
        [Authorize(Roles = Roles.Admin + "," + Roles.Librarian)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerRequestExample(typeof(PatronUpdateDto), typeof(PatronUpdateExample))]
        public async Task<IActionResult> UpdatePatron(
            string id,
            [FromBody] PatronUpdateDto updateDto,
            CancellationToken ct)
        {
            var internalId = hashIdService.Decode(id);

            await patronService.UpdatePatronAsync(internalId, updateDto, ct);
            return NoContent();
        }

        /// <summary>
        /// Deletes a patron account.
        /// </summary>
        /// <remarks>
        /// Patrons with active borrowed books usually cannot be deleted until books are returned.
        /// </remarks>
        /// <param name="id">The ID of the patron to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}", Name = nameof(DeletePatron))]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePatron(string id, CancellationToken ct)
        {
            var internalId = hashIdService.Decode(id);

            await patronService.DeletePatronAsync(internalId, ct);
            return NoContent();
        }
    }
}
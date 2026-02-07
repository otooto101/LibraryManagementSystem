using API.Infrastructure.Extensions;
using API.Models.Request;
using API.Models.Response;
using API.Swagger.Examples.Book;
using Application.DTOs.Book;
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
    /// Manages operations for Books in the library system.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class BookController(
        IBookService bookService,
        IHashIdService hashIdService
        ) : ControllerBase
    {
        /// <summary>
        /// Retrieves a paginated list of books.
        /// </summary>
        /// <remarks>
        /// Supports filtering by Title, Author, or ISBN via query parameters.
        /// </remarks>
        /// <param name="filter">Search and pagination options.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A paged list of books.</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResult<BookResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<BookResponse>>> GetAll(
            [FromQuery] BookSearchRequest filter,
            CancellationToken ct)
        {
            var dtos = await bookService.GetPagedBooksAsync(filter.Adapt<BookSearchParameters>(), ct);
            var response = dtos.Adapt<PagedResult<BookResponse>>();

            foreach (var book in response.Items)
            {
                book.AddStandardLinks(HttpContext, Url, book.BookId,
                    getRouteName: nameof(GetBookById),
                    deleteRouteName: nameof(DeleteBook),
                    updateRouteName: nameof(UpdateBook));
            }

            return Ok(response);
        }

        /// <summary>
        /// Retrieves a single book by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the book.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The detailed information of the book.</returns>
        [HttpGet("{id}", Name = nameof(GetBookById))]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BookResponseExample))]
        public async Task<ActionResult<BookResponse>> GetBookById(string id, CancellationToken ct)
        {
            var internalId = hashIdService.Decode(id);

            var dto = await bookService.GetBookByIdAsync(internalId, ct);
            var response = dto.Adapt<BookResponse>();

            response.AddStandardLinks(HttpContext, Url, response.BookId,
                getRouteName: nameof(GetBookById),
                deleteRouteName: nameof(DeleteBook),
                updateRouteName: nameof(UpdateBook));

            return Ok(response);
        }

        /// <summary>
        /// Creates a new book in the library.
        /// </summary>
        /// <param name="createDto">The book creation details.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The newly created book.</returns>
        [HttpPost]
        [Authorize(Roles = Roles.Admin + "," + Roles.Librarian)]
        [ProducesResponseType(typeof(BookResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerRequestExample(typeof(BookCreateDto), typeof(BookCreateExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(BookResponseExample))]
        public async Task<ActionResult<BookResponse>> Create(
            [FromBody] BookCreateDto createDto,
            CancellationToken ct)
        {
            var dto = await bookService.CreateBookAsync(createDto, ct);
            var response = dto.Adapt<BookResponse>();

            response.AddStandardLinks(HttpContext, Url, response.BookId,
                getRouteName: nameof(GetBookById),
                deleteRouteName: nameof(DeleteBook),
                updateRouteName: nameof(UpdateBook));

            var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

            return CreatedAtRoute(
                routeName: nameof(GetBookById),
                routeValues: new { id = response.BookId, version = version },
                value: response);
        }

        /// <summary>
        /// Updates an existing book.
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="updateDto">The updated values.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}", Name = nameof(UpdateBook))]
        [Authorize(Roles = Roles.Admin + "," + Roles.Librarian)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerRequestExample(typeof(BookUpdateDto), typeof(BookUpdateExample))]
        public async Task<IActionResult> UpdateBook(
            string id, // Changed int -> string
            [FromBody] BookUpdateDto updateDto,
            CancellationToken ct)
        {
            var internalId = hashIdService.Decode(id);

            await bookService.UpdateBookAsync(internalId, updateDto, ct);
            return NoContent();
        }

        /// <summary>
        /// Deletes a book from the library.
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}", Name = nameof(DeleteBook))]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBook(string id, CancellationToken ct) // Changed int -> string
        {
            var internalId = hashIdService.Decode(id); // Decode
            await bookService.DeleteBookAsync(internalId, ct);
            return NoContent();
        }

        /// <summary>
        /// Checks if a book is currently available to be borrowed.
        /// </summary>
        /// <param name="id">The Book ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Availability status.</returns>
        [HttpGet("{id}/availability")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BookAvailabilityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BookAvailabilityExample))]
        public async Task<ActionResult<BookAvailabilityDto>> CheckAvailability(string id, CancellationToken ct)
        {
            var internalId = hashIdService.Decode(id);
            bool isAvailable = await bookService.CheckAvailabilityAsync(internalId, ct);
            return Ok(new BookAvailabilityDto { IsAvailable = isAvailable });
        }

        /// <summary>
        /// Adjusts the stock quantity of a book.
        /// </summary>
        /// <remarks>
        /// Send a positive number to add stock, or a negative number to reduce stock.
        /// </remarks>
        /// <param name="id">The Book ID.</param>
        /// <param name="request">The adjustment amount.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPost("{id}/stock", Name = nameof(AdjustStock))]
        [Authorize(Roles = Roles.Admin + "," + Roles.Librarian)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerRequestExample(typeof(StockAdjustmentRequest), typeof(StockAdjustmentExample))]
        public async Task<IActionResult> AdjustStock(
            string id,
            [FromBody] StockAdjustmentRequest request,
            CancellationToken ct)
        {
            var internalId = hashIdService.Decode(id);
            await bookService.UpdateStockAsync(internalId, request.ChangeAmount, ct);
            return NoContent();
        }
    }
}
using Application.DTOs.Borrowing;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;
using Mapster;
using Moq;
using Xunit;

namespace Application.UnitTests
{
    public class BorrowServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IBookRepository> _mockBookRepo;
        private readonly Mock<IPatronRepository> _mockPatronRepo;
        private readonly Mock<IBorrowRepository> _mockBorrowRepo;
        private readonly Mock<IHashIdService> _mockHashIdService;

        private readonly BorrowService _service;

        public BorrowServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockBookRepo = new Mock<IBookRepository>();
            _mockPatronRepo = new Mock<IPatronRepository>();
            _mockBorrowRepo = new Mock<IBorrowRepository>();
            _mockHashIdService = new Mock<IHashIdService>();

            // Linking repositories to the Unit of Work mock
            _mockUow.Setup(u => u.Books).Returns(_mockBookRepo.Object);
            _mockUow.Setup(u => u.Patrons).Returns(_mockPatronRepo.Object);
            _mockUow.Setup(u => u.Borrows).Returns(_mockBorrowRepo.Object);

            // Initializing Service with both dependencies
            _service = new BorrowService(_mockUow.Object, _mockHashIdService.Object);

            // Mocking HashId decoding behavior globally for simpler tests
            _mockHashIdService.Setup(s => s.Decode(It.IsAny<string>()))
                .Returns((string s) => int.TryParse(s, out var id) ? id : 0);

            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
        }

        #region CheckoutBookAsync Tests

        [Fact]
        public async Task CheckoutBookAsync_ShouldThrowBookNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var dto = new BorrowBookDto { BookId = "99", PatronId = "1", DaysAllowed = 7 };

            _mockBookRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book?)null);

            // Act
            Func<Task> act = async () => await _service.CheckoutBookAsync(dto);

            // Assert
            await act.Should().ThrowAsync<BookNotFoundException>();
        }

        [Fact]
        public async Task CheckoutBookAsync_ShouldThrowPatronNotFound_WhenPatronDoesNotExist()
        {
            // Arrange
            var dto = new BorrowBookDto { BookId = "1", PatronId = "99", DaysAllowed = 7 };

            _mockBookRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { BookId = 1, Title = "Test Book", Quantity = 5, PublicationYear = 1999 });

            _mockPatronRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Patron?)null);

            // Act
            Func<Task> act = async () => await _service.CheckoutBookAsync(dto);

            // Assert
            await act.Should().ThrowAsync<PatronNotFoundException>();
        }

        [Fact]
        public async Task CheckoutBookAsync_ShouldThrowBookOutOfStock_WhenDecrementFails()
        {
            // Arrange
            var dto = new BorrowBookDto { BookId = "1", PatronId = "1", DaysAllowed = 7 };

            _mockBookRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { BookId = 1, Title = "Popular Book", Quantity = 0, PublicationYear = 1999 });

            _mockPatronRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Patron { PatronId = 1, FirstName = "John", LastName = "Doe", Email = "j@d.com" });

            _mockBookRepo.Setup(r => r.TryDecrementStockAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _service.CheckoutBookAsync(dto);

            // Assert
            await act.Should().ThrowAsync<BookOutOfStockException>()
                .WithMessage("*Popular Book*");

            _mockUow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CheckoutBookAsync_ShouldCreateRecord_WhenStockIsAvailable()
        {
            // Arrange
            var dto = new BorrowBookDto { BookId = "1", PatronId = "1", DaysAllowed = 7 };

            _mockBookRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { BookId = 1, Title = "Test Book", Quantity = 1, PublicationYear = 1999 });

            _mockPatronRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Patron { PatronId = 1, FirstName = "John", LastName = "Doe", Email = "j@d.com" });

            _mockBookRepo.Setup(r => r.TryDecrementStockAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.CheckoutBookAsync(dto);

            // Assert
            result.Should().NotBeNull();
            _mockBorrowRepo.Verify(r => r.AddAsync(It.IsAny<BorrowRecord>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region ReturnAsync Tests

        [Fact]
        public async Task ReturnAsync_ShouldThrowRecordNotFound_WhenIdInvalid()
        {
            // Arrange
            _mockBorrowRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((BorrowRecord?)null);

            // Act
            Func<Task> act = async () => await _service.ReturnAsync(99);

            // Assert
            await act.Should().ThrowAsync<BorrowRecordNotFoundException>();
        }

        [Fact]
        public async Task ReturnAsync_ShouldUpdateStatusAndRestock_WhenSuccessful()
        {
            // Arrange
            var record = new BorrowRecord(bookId: 10, patronId: 5, daysAllowed: 7);

            _mockBorrowRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(record);

            _mockBookRepo.Setup(r => r.AdjustStockAsync(10, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _service.ReturnAsync(1);

            // Assert
            record.ReturnDate.Should().NotBeNull();
            record.Status.Should().Be(BorrowStatus.Returned);

            _mockBorrowRepo.Verify(r => r.UpdateAsync(record, It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region DeleteRecordAsync Tests

        [Fact]
        public async Task DeleteRecordAsync_ShouldRestock_WhenDeletingActiveLoan()
        {
            // Arrange
            var record = new BorrowRecord(bookId: 5, patronId: 2, daysAllowed: 7);

            _mockBorrowRepo.Setup(r => r.GetByIdAsync(100, It.IsAny<CancellationToken>()))
                .ReturnsAsync(record);

            // Act
            await _service.DeleteRecordAsync(100);

            // Assert
            _mockBookRepo.Verify(r => r.AdjustStockAsync(5, 1, It.IsAny<CancellationToken>()), Times.Once);
            _mockBorrowRepo.Verify(r => r.DeleteAsync(record, It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
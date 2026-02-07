namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBookRepository Books { get; }
        IPatronRepository Patrons { get; }
        IBorrowRepository Borrows { get; }
        Task CommitAsync(CancellationToken ct = default);
    }
}



namespace Application.Interfaces
{
    public interface IFileService
    {
        Task WriteTextAsync(string path, string content);
        void CreateDirectory(string path);
    }
}

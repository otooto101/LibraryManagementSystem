using Application.Interfaces;

namespace Infrastructure.Services
{
    public class FileService : IFileService
    {
        public async Task WriteTextAsync(string path, string content)
        {
            await File.WriteAllTextAsync(path, content);
        }

        public void CreateDirectory(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
    }
}


namespace Application.Interfaces
{
    public interface ITrafficLogService
    {
        Task SaveLogAsync(Stream bodyStream, string contentType, string traceId, string method, string path, string typeSuffix);
    }
}

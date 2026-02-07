using API.Models.Request;
using Application.Interfaces;
using Application.Mappings;
using Application.Models;
using Mapster;

namespace API.Infrastructure.Mappings
{
    public static class MappingConfigApi
    {
        public static void ConfigureModels(IServiceProvider services)
        {
            var hashIdService = services.GetRequiredService<IHashIdService>();

            TypeAdapterConfig<BookSearchRequest, BookSearchParameters>.NewConfig()
                .Ignore(dest => dest.BorrowerId);

            TypeAdapterConfig<BorrowSearchRequest, BorrowSearchParameters>.NewConfig()
                .Map(dest => dest.PatronId, src => hashIdService.Decode(src.PatronId))
                .Map(dest => dest.BookId, src => hashIdService.Decode(src.BookId));

            MappingConfig.Configure(hashIdService);

        }
    }
}

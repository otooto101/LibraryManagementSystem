using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);   

            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IPatronService, PatronService>();
            services.AddScoped<IBorrowService, BorrowService>();

            return services;
        }
    }
}

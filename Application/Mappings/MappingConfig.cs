using Application.DTOs.Author;
using Application.DTOs.Book;
using Application.DTOs.Borrowing;
using Application.DTOs.Patron;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
    public static class MappingConfig
    {
        public static void Configure(IHashIdService hashIdService)
        {
            TypeAdapterConfig<Author, AuthorBriefDto>.NewConfig()
                .Map(dest => dest.AuthorId, src => hashIdService.Encode(src.AuthorId))
                .Map(dest => dest.FullName, src => src.FullName);

            TypeAdapterConfig<Author, AuthorDto>.NewConfig()
                .Map(dest => dest.AuthorId, src => hashIdService.Encode(src.AuthorId));
            
            TypeAdapterConfig<AuthorUpdateDto, Author>.NewConfig()
                .Map(dest => dest.AuthorId, src => hashIdService.Decode(src.AuthorId))
                .IgnoreNullValues(true);

            TypeAdapterConfig<Book, BookDto>.NewConfig()
                .Map(dest => dest.BookId, src => hashIdService.Encode(src.BookId))
                .Map(dest => dest.IsAvailable, src => src.Quantity > 0)
                .Map(dest => dest.Author, src => src.Author);

            TypeAdapterConfig<BookCreateDto, Book>.NewConfig()
                .Map(dest => dest.AuthorId, src => hashIdService.Decode(src.AuthorId));

            TypeAdapterConfig<BookUpdateDto, Book>.NewConfig()
                .Map(dest => dest.AuthorId, src => hashIdService.Decode(src.AuthorId))
                .IgnoreNullValues(true);

            TypeAdapterConfig<Patron, PatronDto>.NewConfig()
                .Map(dest => dest.PatronId, src => hashIdService.Encode(src.PatronId));

            TypeAdapterConfig<PatronUpdateDto, Patron>.NewConfig()
                .IgnoreNullValues(true);

            TypeAdapterConfig<BorrowRecord, BorrowDto>.NewConfig()
                .Map(dest => dest.Id, src => hashIdService.Encode(src.Id))
                .Map(dest => dest.PatronId, src => hashIdService.Encode(src.PatronId))
                .Map(dest => dest.BookId, src => hashIdService.Encode(src.BookId))
                .Map(dest => dest.PatronName, src => $"{src.Patron.FirstName} {src.Patron.LastName}")
                .Map(dest => dest.BookTitle, src => src.Book.Title);

            TypeAdapterConfig<BorrowBookDto, BorrowRecord>.NewConfig()
                .Map(dest => dest.PatronId, src => hashIdService.Decode(src.PatronId))
                .Map(dest => dest.BookId, src => hashIdService.Decode(src.BookId));

            TypeAdapterConfig.GlobalSettings.ForType(typeof(PagedResult<>), typeof(PagedResult<>))
                .MapToConstructor(true);
        }
    }
}

# Library Management System

A .NET 8 Web API for managing a library. This started as a project to implement **Clean Architecture** properly while handling real-world issues like ID obfuscation and race conditions during book checkouts.

## Why this exists
I wanted to build a system that doesn't just do CRUD, but handles:
- **ID Security:** No one sees database integers. Everything in the API uses `HashIds`.
- **Concurrency:** Uses atomic updates (`ExecuteUpdateAsync`) for stock management so two people can't borrow the last copy of a book at the same time.
- **Clean Code:** Solid separation between layers. The Domain doesn't know about the Database, and the Application doesn't know about Swagger.

## The Tech Stack
- **Backend:** .NET 8 (C#)
- **DB:** SQL Server + EF Core
- **Mapping:** Mapster (configured to handle HashId <-> Int conversion automatically)
- **Validation:** FluentValidation
- **Auth:** JWT Bearer tokens with Role-based access (Admin/Librarian)
- **Logging:** Serilog (configured for daily file rotation)

## Setup & Running

1. **Database:** Update the connection string in `API/appsettings.json`. 
   The app uses `await app.SeedDatabaseAsync()`, so it will create the DB and add some test data (Admins, Books, Authors) automatically on the first run.

2. **Run:**
   dotnet run --project API

3. **Swagger**: Head to https://localhost:PORT/swagger to see the endpoints.

## Project Structure
- **Domain:** Pure C# entities and custom exceptions.
- **Application:** Business logic, DTOs, and the `IBorrowService`. This is where the unit tests live.
- **Infrastructure:** JWT logic and the `HashIdService`.
- **Persistence:** SQL implementation, Repositories, and the `LibraryContext`.
- **API:** Controllers and Middlewares (including custom Global Exception Handling).

## Security & Roles
- **Public:** Can view books and authors.
- **Librarians:** Can register patrons and handle checkouts/returns.
- **Admins:** Can delete records and register new Librarians.

## Important Note on IDs
Since the API uses HashIds, you can't pass `1` or `2` as an ID in the URL. You need to use the hashed string (e.g., `jR8vW2`) returned in the JSON responses. The backend decodes these automatically before hitting the database.

## Running Tests
I've included unit tests for the `BorrowService` to ensure the logic for checkouts, returns, and stock adjustments is solid.

dotnet test

> [!CAUTION]
> **P.S. კოდი თქვენთან შეიძლება არ გაეშვას თუ SQL-ში FULLTEXT SEARCH feature არ გაქვთ დაყენებული.**

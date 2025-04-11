# Candidate Management API

A clean and extensible REST API built with **.NET 8**, following **Clean Architecture** and **Domain-Driven Design (DDD)** principles. This application is designed to create or update candidate information using a single endpoint and is ready for future scaling and persistence layer replacement.

---

## 🧰 Tech Stack

- **.NET 8** – Modern cross-platform backend framework.
- **ASP.NET Core Web API** – RESTful endpoint implementation.
- **Entity Framework Core** – ORM for data access.
- **SQLite** – Lightweight relational database for local development.
- **Clean Architecture** – Separation of concerns, modular structure.
- **Domain-Driven Design (DDD)** – Rich domain model (`Candidate` entity).
- **FluentValidation** – Input validation.
- **In-Memory Caching** – Faster repeated access by email.
- **Middleware** – Centralized error handling using `ProblemDetails`.
- **XUnit** – Unit testing framework.
- **Moq** – Mocking dependencies in tests.
- **FluentAssertions** – Clear and expressive test assertions.
- **Swagger / OpenAPI** – API documentation and testing UI.

---

## 🚀 How to Run

1. **Clone the repository:**
    ```bash
    git clone https://github.com/your-username/candidate-hub-api.git
    cd candidate-hub-api
    ```
2. **Run the application**
    ```bash
    dotnet run --project CandidateManagement.API
    ```
3. **Open your browser and navigate to Swagger UI**
    ```bash
    https://localhost:7119/swagger
    ```

---

## ✅ What’s Implemented

| Requirement                                           | Status | Notes |
|------------------------------------------------------|--------|-------|
| Single POST endpoint to create or update candidates  | ✅     | `POST /api/candidates` |
| Email as unique identifier                           | ✅     | Lowercased and unique index |
| Required/optional fields                             | ✅     | Validated via FluentValidation |
| Call time interval (start/end)                       | ✅     | Uses `TimeOnly?` and cross-validation |
| SQL DB with potential to replace                       | ✅     | Uses EF Core and Repository abstraction |
| Self deployable                                      | ✅     | SQLite and Migrations |
| Global error handling                                | ✅     | With `ProblemDetails` JSON output |
| In-memory caching                                     | ✅     | 10 min cache by email |
| Unit tests                                            | ✅     | Candidate repository and service |
| Clean, maintainable architecture                      | ✅     | Clean Architecture & DDD |

---

## 🧱 Architecture

- **Domain** – `Candidate` entity encapsulates business logic.
- **Application** – DTOs, validators, and interfaces.
- **Infrastructure** – EF Core, SQLite, caching, and repositories.
- **API** – Minimal controller, global exception middleware.
- **Tests** – Unit tests using `XUnit`, `Moq`, and `FluentAssertions`.

---

## 💡 Improvements

- Extend caching strategy to use **DistributedCache** for multi-instance deployment.
- Add **async validation** for external URLs.
- Add **Docker Compose** support with PostgreSQL and Seq.
- Add **structured logging** with Serilog and dashboarding via Seq.
- **Integration Tests**.
- Role-based **Authentication**.
- Add **pagination**, **filtering**, and **candidate retrieval endpoints**.

---

## 🤔 Assumptions

- Email is the unique identifier and stored as lowercase.
- Start and end call times are optional but must be valid if provided together.
- Timezones for call times are not considered.
- Phone number is regex-validated but not normalized.
- LinkedIn and GitHub URLs are format-checked only.
- SQLite is used for simplicity, but can be replaced easily via `ICandidateRepository`.

---

## 🕒 Time Spent including writing documantation: 7 hours and 50 minutes

---
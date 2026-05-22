# Architecture

This document explains the architectural decisions behind the Project Management API and the trade-offs made for a focused 48-hour technical assessment.

## 1. Architecture Overview

The solution follows Clean Architecture with four layers:

```text
                         HTTP
                          |
                          v
+------------------------------------------------+
| ProjectManagement.API                          |
| Controllers, middleware, Swagger, composition  |
+------------------------+-----------------------+
                         |
                         | sends commands/queries
                         v
+------------------------------------------------+
| ProjectManagement.Application                  |
| CQRS handlers, validators, DTOs, interfaces    |
+------------------------+-----------------------+
                         |
                         | uses domain model
                         v
+------------------------------------------------+
| ProjectManagement.Domain                       |
| Entities, invariants, enums, domain exceptions |
+------------------------------------------------+

+------------------------------------------------+
| ProjectManagement.Infrastructure               |
| EF Core, Identity, JWT, migrations, services   |
+------------------------+-----------------------+
                         |
                         | implements Application ports
                         v
                 Application abstractions
```

Runtime composition happens in the API project. The Domain remains independent from infrastructure and framework details.

## 2. Layer Responsibilities

### Domain

The Domain layer contains the business model and rules:

- `Project`, `TaskItem`, and `RefreshToken`
- Domain constants and enums
- Domain-specific exceptions
- Entity factory methods and behavior such as `Project.Update(...)` and `TaskItem.UpdateStatus(...)`

The Domain layer does not know about EF Core, MediatR, ASP.NET Core Identity, ASP.NET Core hosting, SQL Server, or JWT. Projects and refresh tokens store the user identifier they need for ownership and relationships, while the concrete Identity user type stays in Infrastructure.

### Application

The Application layer contains use cases:

- Commands and queries
- MediatR handlers
- DTOs
- FluentValidation validators
- Pipeline behaviors for validation, logging, performance monitoring, and unhandled exceptions
- Interfaces such as `IApplicationDbContext`, `ICurrentUserService`, `IIdentityService`, and `IJwtService`
- Application-level identity models such as `UserAccount`

This layer coordinates business flow but does not contain persistence implementation details.

### Infrastructure

The Infrastructure layer implements external concerns:

- EF Core `ApplicationDbContext`
- Entity configurations
- Migrations
- Audit and soft-delete interceptors
- ASP.NET Core Identity integration
- Infrastructure `ApplicationUser` implementation
- JWT generation and refresh-token hashing
- Current-user resolution from HTTP context
- Role seeding and optional admin seeding

Infrastructure depends on Application and Domain because it implements Application-defined ports.

### API

The API layer is the delivery mechanism:

- Controllers
- Authentication and authorization setup
- Swagger/OpenAPI
- API versioning
- Global exception handling
- CORS
- Serilog request logging
- Startup composition and migrations

Controllers are intentionally thin. They translate HTTP requests into commands/queries and return the `Result<T>` response.

## 3. Dependency Rule

Clean Architecture depends on one rule: dependencies point inward.

```text
API ---------------> Application ---------------> Domain
 |                         ^
 |                         |
 +-----> Infrastructure ---+
```

The Domain does not depend on any outer layer. Application depends on Domain and defines interfaces for external services. Infrastructure implements those interfaces. API references all layers only to compose the application at startup.

This keeps business logic portable, testable, and protected from framework churn.

## 4. Why CQRS + MediatR

Each use case is represented as one command or query:

- `CreateProjectCommand`
- `GetProjectByIdQuery`
- `CreateTaskCommand`
- `LoginCommand`

This gives every operation a small and focused handler. Reads and writes evolve independently, validators stay close to their request models, and cross-cutting concerns are handled once through MediatR pipeline behaviors.

The pipeline order is:

```text
Request
  -> UnhandledExceptionBehavior
    -> LoggingBehavior
      -> ValidationBehavior
        -> PerformanceBehavior
          -> Handler
```

This keeps handlers clean and predictable.

## 5. Why Result Pattern Over Exceptions for Business Flow

Expected operation outcomes return `Result<T>`:

- Successful responses
- Validation failures
- Authentication failures such as invalid credentials
- Standard status codes and error shape

This makes API responses consistent and avoids using exceptions for normal control flow. For example, invalid login credentials return a failure result instead of throwing.

The API contract becomes predictable:

```json
{
  "isSuccess": false,
  "data": null,
  "error": "Invalid email or password.",
  "errors": ["Invalid email or password."],
  "statusCode": 401
}
```

## 6. Why Exceptions in Domain Layer

The Domain layer uses exceptions to protect invariants because invalid entity state is not a valid business result. Examples:

- A project cannot be created without a name.
- A task cannot be created without a project id.
- A task status must be a defined enum value.

Throwing a `DomainException` prevents invalid aggregates from existing. The API layer maps these exceptions to safe HTTP responses through the global exception handler.

## 7. Authentication & Authorization Flow

```text
Client
  |
  | POST /api/v1/auth/login
  v
API Controller
  |
  | LoginCommand
  v
Application Handler
  |
  | verifies credentials through IIdentityService
  v
IdentityService
  |
  | user valid
  v
JwtService creates access token
Refresh token is generated, hashed, and stored
  |
  v
Client receives:
  - accessToken, short lived
  - refreshToken, long lived

Protected request:

Client -> Authorization: Bearer accessToken -> JWT middleware -> Controller -> Handler

Refresh flow:

Client -> /api/v1/auth/refresh -> validate stored token -> revoke old token -> issue new pair
```

Authorization is enforced in handlers, not only controllers. A user can access only projects where `Project.UserId == CurrentUser.UserId`. Tasks inherit ownership through `TaskItem.Project.UserId`.

## 8. Soft Delete Strategy

Soft delete is centralized:

- Entities that support deletion implement `ISoftDelete`.
- `SoftDeleteInterceptor` converts EF `Deleted` state into `Modified`.
- The interceptor sets `IsDeleted = true` and `DeletedAt`.
- `ApplicationDbContext` applies a global query filter for soft-deletable entities.

Handlers can call `DbContext.Remove(entity)` without knowing the persistence detail. Query code automatically excludes deleted rows.

## 9. Auditing Strategy

Auditing is also centralized:

- `AuditableEntity` exposes `CreatedAt`, `CreatedBy`, `UpdatedAt`, and `UpdatedBy`.
- `AuditableEntityInterceptor` sets audit fields during `SaveChanges`.
- `ICurrentUserService` provides the authenticated user id.

This keeps audit logic out of controllers and handlers.

## 10. Error Handling Strategy

The system uses two layers of error handling:

```text
Expected outcomes:
  Handler -> Result<T> -> Controller -> HTTP response

Unexpected or invariant failures:
  Exception -> GlobalExceptionHandler -> Result<object> -> HTTP response
```

Mapped exceptions:

| Exception | HTTP Status |
| --- | --- |
| `ValidationException` | 400 |
| `UnauthorizedException` | 401 |
| `ForbiddenException` | 403 |
| `NotFoundException` | 404 |
| `DomainException` | 422 |
| `Exception` | 500 |

The global handler logs full exception details but returns a safe generic response for unknown errors.

## 11. Why NOT Repository Pattern

This project intentionally avoids a custom repository abstraction.

EF Core already provides:

- Unit of Work through `DbContext`
- Repository-like access through `DbSet<T>`
- Change tracking
- Transactions
- LINQ query composition

Adding generic repositories would hide useful EF Core capabilities and add boilerplate without increasing clarity for this scope. The Application layer still avoids direct infrastructure coupling by depending on `IApplicationDbContext`.

## 12. Why NOT Separate Contracts Project

A separate Contracts project can be useful for large distributed systems, SDK generation, or independent API contract versioning.

For this assessment, it would add ceremony without meaningful benefit. DTOs live close to their use cases in the Application layer, which keeps feature folders easy to navigate and review.

## 13. Trade-Offs Made for the 48-Hour Scope

- Unit tests focus on handler behavior and authorization rules rather than full API integration tests.
- Tasks return a simple list instead of paginated results because project task lists are scoped and the requirement made pagination optional.
- Docker uses development-friendly defaults while allowing environment variable overrides.
- Refresh tokens are stored in SQL Server, not Redis, to keep the deployment simple.
- Swagger and Postman are both provided to make manual review fast.
- The API includes a liveness health endpoint, while deeper dependency-specific health checks are left for a production hardening pass.

## 14. What I'd Add for True Production

- Redis caching layer for hot reads and distributed token/session scenarios.
- Rate limiting with `AspNetCoreRateLimit` or ASP.NET Core built-in rate limiting policies.
- OpenTelemetry observability with traces, metrics, and logs correlated by request id.
- Health checks endpoint with SQL Server, Redis, and external dependency checks.
- API versioning governance when introducing breaking changes beyond the current v1 surface.
- Integration tests with Testcontainers using real SQL Server.
- CI/CD pipeline with GitHub Actions for build, test, Docker image publishing, and deployment gates.
- Security hardening with secret management, key rotation, and stricter CORS policies.
- Database indexes reviewed against production query plans.
- Background cleanup job for expired refresh tokens.

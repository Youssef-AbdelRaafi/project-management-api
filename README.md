# Project Management API (.NET 9 / Clean Architecture / CQRS)

![.NET](https://img.shields.io/badge/.NET-9-blue)
![Architecture](https://img.shields.io/badge/Clean%20Architecture-green)
![CQRS](https://img.shields.io/badge/CQRS-MediatR-purple)
![Build](https://img.shields.io/badge/build-passing-brightgreen)
![License](https://img.shields.io/badge/License-MIT-yellow)

A production-minded Project and Task Management API built with ASP.NET Core, Clean Architecture, CQRS, Entity Framework Core, SQL Server, JWT authentication, refresh-token rotation, structured logging, Docker, and unit tests.

## Overview

This API models a simple project management workflow where authenticated users can manage their own projects and tasks. Each user owns projects, and tasks inherit authorization through their parent project.

The implementation focuses on maintainability, testability, and clear boundaries rather than unnecessary abstraction. Business rules live in the Domain and Application layers, persistence details stay in Infrastructure, and controllers remain thin HTTP adapters.

## Features

- 🔐 JWT authentication with refresh-token rotation
- 👤 Role-based authorization with seeded `Admin` and `User` roles
- 📁 Project CRUD with ownership enforcement
- ✅ Task creation, status updates, project-scoped listing, and soft delete
- 🧭 API versioning via `/api/v1`
- 📦 Clean Architecture with strict dependency direction
- 🧱 CQRS handlers using MediatR
- 🧪 Comprehensive unit testing using xUnit, Moq, FluentAssertions, and EF Core InMemory
- 🧾 FluentValidation pipeline with aggregated validation errors
- 🪵 Serilog structured logging
- 🛡️ Global exception handling using `IExceptionHandler`
- 🕰️ Auditing and soft delete through EF Core interceptors
- 🐳 Docker Compose stack with SQL Server, health checks, and persisted data
- 📚 Swagger/OpenAPI with JWT Authorize support

## Tech Stack

| Area | Technology |
| --- | --- |
| Runtime | .NET 9 |
| API | ASP.NET Core Web API |
| Architecture | Clean Architecture, CQRS |
| Messaging | MediatR |
| Persistence | Entity Framework Core 9 |
| Database | SQL Server 2022 |
| Identity | ASP.NET Core Identity |
| Authentication | JWT Bearer + Refresh Tokens |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Logging | Serilog Console + File sinks |
| Testing | xUnit, Moq, FluentAssertions, EF Core InMemory |
| Documentation | Swagger/OpenAPI, Postman |
| Containers | Docker, Docker Compose |

## Architecture

```text
+--------------------------------------------+
| ProjectManagement.API                      |
| Controllers, Swagger, Auth, Middleware     |
+---------------------+----------------------+
                      |
                      | depends on
                      v
+--------------------------------------------+
| ProjectManagement.Application              |
| CQRS, DTOs, Validators, Behaviors, Ports   |
+---------------------+----------------------+
                      |
                      | depends on
                      v
+--------------------------------------------+
| ProjectManagement.Domain                   |
| Entities, Value Rules, Enums, Exceptions   |
+--------------------------------------------+

+--------------------------------------------+
| ProjectManagement.Infrastructure           |
| EF Core, Identity, JWT, Interceptors       |
+---------------------+----------------------+
                      |
                      | implements Application interfaces
                      v
              Application + Domain
```

Dependency rule: source code dependencies point inward. Domain has no dependency on any other layer. Application depends only on Domain abstractions. Infrastructure implements Application interfaces. API composes the system.

See [ARCHITECTURE.md](ARCHITECTURE.md) for the detailed design notes.

## Prerequisites

- [.NET SDK 9](https://dotnet.microsoft.com/download)
- SQL Server or Docker Desktop
- Git
- Optional: Postman

## Getting Started

### Clone

```bash
git clone https://github.com/Youssef-AbdelRaafi/project-management-api.git
cd project-management-api
```

### Local Setup

The committed `appsettings` files intentionally do not contain secrets. Use user-secrets or environment variables for local credentials:

```bash
dotnet user-secrets init --project src/ProjectManagement.API
dotnet user-secrets set "JwtSettings:Secret" "<your-long-random-jwt-secret>" --project src/ProjectManagement.API
```

The default development connection string uses LocalDB from `appsettings.Development.json`. Override it with `ConnectionStrings:DefaultConnection` if you use a different SQL Server instance.

Apply migrations:

```bash
dotnet ef database update \
  --project src/ProjectManagement.Infrastructure \
  --startup-project src/ProjectManagement.API
```

Run the API:

```bash
dotnet run --project src/ProjectManagement.API
```

Local Swagger:

```text
http://localhost:5115/swagger
```

### Docker Setup

Create a local `.env` file from the template, set strong local secrets, then run the stack:

```bash
cp .env.example .env
docker-compose up --build
```

The API will be available at:

```text
http://localhost:8080/swagger
```

The API container waits for SQL Server to become healthy and applies EF Core migrations automatically during startup.

## API Documentation

- Swagger UI: `http://localhost:8080/swagger`
- Health check: `http://localhost:8080/health`
- Base API URL: `http://localhost:8080/api/v1`
- Postman collection: [ProjectManagement.postman_collection.json](ProjectManagement.postman_collection.json)
- Postman environment: [ProjectManagement.postman_environment.json](ProjectManagement.postman_environment.json)

## Project Structure

```text
ProjectManagement.sln
│
├── src/
│   ├── ProjectManagement.Domain/
│   │   ├── Common/
│   │   ├── Constants/
│   │   ├── Entities/
│   │   ├── Enums/
│   │   └── Exceptions/
│   │
│   ├── ProjectManagement.Application/
│   │   ├── Common/
│   │   │   ├── Behaviors/
│   │   │   ├── Interfaces/
│   │   │   ├── Mappings/
│   │   │   └── Models/
│   │   └── Features/
│   │       ├── Auth/
│   │       ├── Projects/
│   │       └── Tasks/
│   │
│   ├── ProjectManagement.Infrastructure/
│   │   ├── Identity/
│   │   └── Persistence/
│   │       ├── Configurations/
│   │       ├── Interceptors/
│   │       └── Migrations/
│   │
│   └── ProjectManagement.API/
│       ├── Controllers/
│       ├── Extensions/
│       ├── Middleware/
│       ├── OpenApi/
│       ├── Dockerfile
│       └── Program.cs
│
├── tests/
│   └── ProjectManagement.UnitTests/
│       ├── Application/
│       │   ├── Auth/
│       │   ├── Projects/
│       │   └── Tasks/
│       └── Common/
│
├── docker-compose.yml
├── .env.example
├── ProjectManagement.postman_collection.json
├── ProjectManagement.postman_environment.json
├── README.md
└── ARCHITECTURE.md
```

## Running Tests

```bash
dotnet test ProjectManagement.sln
```

Current coverage focuses on handler-level behavior:

- Authentication success and failure paths
- Project ownership rules
- Task ownership inheritance through projects

## Environment Variables

| Variable | Required | Example | Description |
| --- | --- | --- | --- |
| `SQLSERVER_SA_PASSWORD` | Yes for Docker | `<strong-local-sa-password>` | SQL Server container password loaded from `.env` |
| `JWT_SECRET` | Yes for Docker | `<long-random-jwt-secret>` | JWT secret loaded by Docker Compose and mapped to `JwtSettings__Secret` |
| `ConnectionStrings__DefaultConnection` | Yes | `Server=sqlserver,1433;Database=ProjectManagementDb;User Id=sa;Password=<password>;TrustServerCertificate=True` | SQL Server connection string |
| `JwtSettings__Secret` | Yes | `<long-random-jwt-secret>` | Signing key for JWT access tokens |
| `JwtSettings__Issuer` | Yes | `ProjectManagement.API` | JWT issuer |
| `JwtSettings__Audience` | Yes | `ProjectManagement.API` | JWT audience |
| `JwtSettings__AccessTokenExpiryMinutes` | No | `15` | Access-token lifetime |
| `JwtSettings__RefreshTokenExpiryDays` | No | `7` | Refresh-token lifetime |
| `DefaultAdmin__Email` | No | `admin@example.com` | Optional seeded admin email |
| `DefaultAdmin__Password` | No | `<admin-password-from-secret-store>` | Optional seeded admin password |
| `DefaultAdmin__FullName` | No | `System Admin` | Optional seeded admin full name |
| `ASPNETCORE_ENVIRONMENT` | No | `Development` | Runtime environment |
| `ASPNETCORE_URLS` | No | `http://+:8080` | Container listening URL |

## Security Highlights

- Password hashing using ASP.NET Core Identity
- JWT access tokens with refresh-token rotation
- Role-based authorization with seeded application roles
- Refresh-token hashing before persistence
- Token revocation support during refresh-token rotation
- Ownership enforcement inside application handlers, not only controllers

## Sample API Requests

### 1. Register

```bash
curl -X POST http://localhost:8080/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password1",
    "fullName": "Demo User"
  }'
```

### 2. Login

```bash
curl -X POST http://localhost:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password1"
  }'
```

Copy `data.accessToken` from the response.

### 3. Create Project

```bash
curl -X POST http://localhost:8080/api/v1/projects \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <access-token>" \
  -d '{
    "name": "Technical Assessment",
    "description": "Backend .NET clean architecture project"
  }'
```

Copy `data.id` from the response.

### 4. Create Task

```bash
curl -X POST http://localhost:8080/api/v1/projects/<project-id>/tasks \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <access-token>" \
  -d '{
    "title": "Prepare submission",
    "description": "Review README, migrations, tests, Docker, and Swagger",
    "dueDate": "2026-05-29T12:00:00Z",
    "priority": 3
  }'
```

## Key Design Decisions

- Clean Architecture keeps the domain model independent from frameworks.
- CQRS handlers keep each use case small, testable, and easy to review.
- EF Core DbContext is used directly as Unit of Work and Repository to avoid unnecessary abstraction.
- Result objects standardize successful and expected failure responses.
- Domain exceptions protect invariants inside entities.
- Global exception handling maps unexpected failures to safe API responses.
- Refresh tokens are hashed before persistence and rotated on refresh.
- Soft delete and auditing are handled centrally with EF Core interceptors.
- Controllers return DTOs only, never EF entities.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

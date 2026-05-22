# Project Management API

A Clean Architecture ASP.NET Core Web API for project and task management. The API uses JWT authentication, EF Core, SQL Server, CQRS with MediatR, FluentValidation, Serilog, and unit tests.

## Docker Setup

Prerequisites:

- Docker Desktop
- Port `8080` available for the API
- Port `1433` available for SQL Server, unless you change the compose mapping

Run the full stack:

```bash
docker-compose up --build
```

The API will start on:

- Swagger UI: http://localhost:8080/swagger
- Health check: http://localhost:8080/health
- Base API URL: http://localhost:8080/api/v1

The `api` service waits for SQL Server to become healthy, then applies EF Core migrations automatically on startup.

## Docker Environment Variables

The compose file includes development defaults. Override them when needed:

```bash
SQLSERVER_SA_PASSWORD="Your_Strong_Password_123!" JWT_SECRET="YourLongJwtSecretAtLeast32Bytes!" docker-compose up --build
```

Important variables:

- `ConnectionStrings__DefaultConnection`
- `JwtSettings__Secret`
- `JwtSettings__Issuer`
- `JwtSettings__Audience`
- `JwtSettings__AccessTokenExpiryMinutes`
- `JwtSettings__RefreshTokenExpiryDays`

## Local Verification

```bash
dotnet build ProjectManagement.sln
dotnet test ProjectManagement.sln
```

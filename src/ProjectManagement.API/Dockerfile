FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ProjectManagement.sln", "./"]
COPY ["src/ProjectManagement.Domain/ProjectManagement.Domain.csproj", "src/ProjectManagement.Domain/"]
COPY ["src/ProjectManagement.Application/ProjectManagement.Application.csproj", "src/ProjectManagement.Application/"]
COPY ["src/ProjectManagement.Infrastructure/ProjectManagement.Infrastructure.csproj", "src/ProjectManagement.Infrastructure/"]
COPY ["src/ProjectManagement.API/ProjectManagement.API.csproj", "src/ProjectManagement.API/"]
COPY ["tests/ProjectManagement.UnitTests/ProjectManagement.UnitTests.csproj", "tests/ProjectManagement.UnitTests/"]

RUN dotnet restore "src/ProjectManagement.API/ProjectManagement.API.csproj"

COPY . .
RUN dotnet build "src/ProjectManagement.API/ProjectManagement.API.csproj" -c Release --no-restore
RUN dotnet publish "src/ProjectManagement.API/ProjectManagement.API.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ProjectManagement.API.dll"]

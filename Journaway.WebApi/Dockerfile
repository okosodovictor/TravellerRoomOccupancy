# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy csproj files first (better layer caching)
COPY Journaway.WebApi/*.csproj ./Journaway.WebApi/
COPY Journaway.Application/*.csproj ./Journaway.Application/
COPY Journaway.Domain/*.csproj ./Journaway.Domain/
COPY Journaway.Infrastructure/*.csproj ./Journaway.Infrastructure/

# Restore
WORKDIR /app/Journaway.WebApi
RUN dotnet restore

# Copy everything else and publish
WORKDIR /app
COPY . .
WORKDIR /app/Journaway.WebApi
RUN dotnet publish -c Release -o /out /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /out .
EXPOSE 8080
EXPOSE 8081
ENTRYPOINT ["dotnet", "Journaway.WebApi.dll"]

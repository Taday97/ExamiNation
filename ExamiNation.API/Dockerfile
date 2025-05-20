# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ExamiNation.sln .
COPY ExamiNation.API/ExamiNation.API.csproj ExamiNation.API/
COPY ExamiNation.Application/ExamiNation.Application.csproj ExamiNation.Application/
COPY ExamiNation.Domain/ExamiNation.Domain.csproj ExamiNation.Domain/
COPY ExamiNation.Infrastructure/ExamiNation.Infrastructure.csproj ExamiNation.Infrastructure/
COPY ExamiNation.NUnitTests/ExamiNation.NUnitTests.csproj ExamiNation.NUnitTests/
COPY ExamiNation.Api.IntegrationTests/ExamiNation.Api.IntegrationTests.csproj ExamiNation.Api.IntegrationTests/
COPY ExamiNation.Api.IntegrationTests/ExamiNation.Api.IntegrationTests.csproj ExamiNation.Api.IntegrationTests/

RUN dotnet restore

COPY . .
WORKDIR /src/ExamiNation.API
RUN dotnet publish -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .


# Sedd
COPY ExamiNation.API/Seed/classifications.json /app/
COPY ExamiNation.API/Seed/test-otis.json /app/
COPY ExamiNation.API/Seed/test-otis-english.json /app/
COPY ExamiNation.API/Seed/classifications-english.json /app/

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "ExamiNation.API.dll"]

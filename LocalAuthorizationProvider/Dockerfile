# Zmiana bazowego obrazu na ASP.NET 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Faza budowania na SDK .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Skopiuj plik projektu do katalogu builda
COPY ["LocalAuthorizationProvider.csproj", "./"]
RUN dotnet restore "./LocalAuthorizationProvider.csproj"

# Skopiuj resztê kodu do katalogu builda
COPY . .

# PrzejdŸ do katalogu projektu i zbuduj aplikacjê
WORKDIR "/src"
RUN dotnet build "./LocalAuthorizationProvider.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Faza publikacji aplikacji
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LocalAuthorizationProvider.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Faza finalna, ostateczny obraz
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LocalAuthorizationProvider.dll"]

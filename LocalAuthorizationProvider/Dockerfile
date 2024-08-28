FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["LocalAuthorizationProvider.csproj", "./"]
RUN dotnet restore "./LocalAuthorizationProvider.csproj"

COPY . .

WORKDIR "/src"
RUN dotnet build "./LocalAuthorizationProvider.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LocalAuthorizationProvider.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

RUN chown -R root:root /app

COPY --from=publish /app/publish .

USER root

ENTRYPOINT ["dotnet", "LocalAuthorizationProvider.dll"]
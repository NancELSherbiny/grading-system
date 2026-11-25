# Build context: ./backend  (see docker-compose.yml)

# 1. Build with the .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore packages first, from the project files only, so Docker can reuse this step
# until a package changes.
COPY API/API.csproj API/
COPY Application/Application.csproj Application/
COPY DAL/DAL.csproj DAL/
RUN dotnet restore API/API.csproj

COPY API/ API/
COPY Application/ Application/
COPY DAL/ DAL/
RUN dotnet publish API/API.csproj -c Release -o /app --no-restore

# 2. Run with the smaller ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "API.dll"]

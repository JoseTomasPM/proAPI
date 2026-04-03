# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY proAPI.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish proAPI.csproj -c Release -o out

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "proAPI.dll"]
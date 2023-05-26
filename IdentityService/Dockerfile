# this needs to be run from the root of the repository

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY IdentityService /src/IdentityService
COPY Shared /src/Shared
RUN dotnet publish /src/IdentityService/API/API.csproj -o /app
RUN dotnet run --project /src/IdentityService/DBCLI/DBCLI.csproj -- "Data Source=/app/identity.db"

FROM mcr.microsoft.com/dotnet/aspnet:7.0
COPY --from=build /app .
ENTRYPOINT ["dotnet", "API.dll"]

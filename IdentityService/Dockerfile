# this needs to be run from the root of the repository

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

COPY IdentityService/API/API.csproj /src/IdentityService/API/
COPY IdentityService/BLL/BLL.csproj /src/IdentityService/BLL/
COPY IdentityService/DAL/DAL.csproj /src/IdentityService/DAL/
COPY IdentityService/DBCLI/DBCLI.csproj /src/IdentityService/DBCLI/
COPY Shared/Shared.API/Shared.API.csproj /src/Shared/Shared.API/
COPY Shared/Shared.BLL/Shared.BLL.csproj /src/Shared/Shared.BLL/
RUN dotnet restore src/IdentityService/API/API.csproj

COPY IdentityService /src/IdentityService
COPY Shared /src/Shared
RUN dotnet publish /src/IdentityService/API/API.csproj -o /app
RUN dotnet run --project /src/IdentityService/DBCLI/DBCLI.csproj -- "Data Source=/app/identity.db"

FROM mcr.microsoft.com/dotnet/aspnet:7.0
COPY --from=build /app .
ENTRYPOINT ["dotnet", "API.dll"]

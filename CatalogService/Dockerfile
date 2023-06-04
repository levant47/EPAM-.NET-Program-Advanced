# this needs to be run from the root of the repository

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

COPY CatalogService/API/API.csproj /src/CatalogService/API/
COPY CatalogService/BLL/BLL.csproj /src/CatalogService/BLL/
COPY CatalogService/DAL/DAL.csproj /src/CatalogService/DAL/
COPY CatalogService/Messaging/Messaging.csproj /src/CatalogService/Messaging/
COPY Shared/Shared.API/Shared.API.csproj /src/Shared/Shared.API/
COPY Shared/Shared.BLL/Shared.BLL.csproj /src/Shared/Shared.BLL/
RUN dotnet restore src/CatalogService/API/API.csproj

COPY CatalogService /src/CatalogService
COPY Shared /src/Shared
RUN dotnet publish /src/CatalogService/API/API.csproj -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0
COPY --from=build /app .
ENTRYPOINT ["dotnet", "API.dll"]

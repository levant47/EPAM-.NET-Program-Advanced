# this needs to be run from the root of the repository

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

COPY CartingService/API/API.csproj /src/CartingService/API/
COPY CartingService/BLL/BLL.csproj /src/CartingService/BLL/
COPY CartingService/DAL/DAL.csproj /src/CartingService/DAL/
COPY CartingService/Messaging/Messaging.csproj /src/CartingService/Messaging/
COPY Shared/Shared.API/Shared.API.csproj /src/Shared/Shared.API/
COPY Shared/Shared.BLL/Shared.BLL.csproj /src/Shared/Shared.BLL/
RUN dotnet restore src/CartingService/API/API.csproj

COPY CartingService /src/CartingService
COPY Shared /src/Shared
RUN dotnet publish /src/CartingService/API/API.csproj -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0
COPY --from=build /app .
ENTRYPOINT ["dotnet", "API.dll"]

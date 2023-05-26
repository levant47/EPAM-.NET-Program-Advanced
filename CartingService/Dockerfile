# this needs to be run from the root of the repository

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY CartingService /src/CartingService
COPY Shared /src/Shared
RUN dotnet publish /src/CartingService/API/API.csproj -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0
COPY --from=build /app .
ENTRYPOINT ["dotnet", "API.dll"]

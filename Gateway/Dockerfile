# this needs to be run from the root of the repository

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY Gateway /src/Gateway
COPY Shared /src/Shared
RUN dotnet publish /src/Gateway/Gateway.csproj -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Gateway.dll"]

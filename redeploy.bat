@echo off
git checkout master && git pull
taskkill /im api.exe /f
taskkill /im gateway.exe /f
SET repo=%~dp0
start /b dotnet run --project "%repo%\CatalogService\API\API.csproj" --urls="http://0.0.0.0:5000"
start /b dotnet run --project "%repo%\CartingService\API\API.csproj"
start /b dotnet run --project "%repo%\IdentityService\API\API.csproj" --urls="http://0.0.0.0:5002"
start /b dotnet run --project "%repo%\Gateway\Gateway.csproj" --urls="http://0.0.0.0:5100"

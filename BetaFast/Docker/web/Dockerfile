FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

EXPOSE 5000
EXPOSE 5001

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY BetaFastAPI/*.csproj ./BetaFastAPI/
RUN dotnet restore

# Copy everything else and build
COPY BetaFastAPI/. ./BetaFastAPI/
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/BetaFastAPI/out .
COPY BetaFastAPI/StaticFiles/. ./StaticFiles/
ENTRYPOINT ["dotnet", "BetaFastAPI.dll"]
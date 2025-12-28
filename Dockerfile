FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all csproj files
COPY Aether/*.csproj Aether/
COPY Aether.Console/*.csproj Aether.Console/
COPY Aether.Core/*.csproj Aether.Core/
COPY Aether.Repositories/*.csproj Aether.Repositories/
COPY Aether.Services/*.csproj Aether.Services/

# Restore
WORKDIR /src/Aether
RUN dotnet restore

# Copy everything
WORKDIR /src
COPY . .

# Build
WORKDIR /src/Aether
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Aether.dll"]
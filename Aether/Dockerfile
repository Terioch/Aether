FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all csproj files
COPY Aether/*.csproj Aether/
COPY Aether.Console/*.csproj Aether.Console/
COPY Aether.Core/*.csproj Aether.Core/
COPY Aether.Repositories/*.csproj Aether.Repositories/
COPY Aether.Services/*.csproj Aether.Services/

# Restore from the main project
WORKDIR /src/Aether
RUN dotnet restore

# Copy all source code
WORKDIR /src
COPY Aether/ Aether/
COPY Aether.Console/ Aether.Console/
COPY Aether.Core/ Aether.Core/
COPY Aether.Repositories/ Aether.Repositories/
COPY Aether.Services/ Aether.Services/

# Build and publish
WORKDIR /src/Aether
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Aether.dll"]
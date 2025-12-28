FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Aether/Aether.csproj", "Aether/"]
COPY ["Aether/Aether.Console.csproj", "Aether/"]
COPY ["Aether/AetherCore.csproj", "Aether/"]
COPY ["Aether/Aether.Repositories.csproj", "Aether/"]
RUN dotnet restore "Aether/Aether.Services.csproj"
COPY . .
WORKDIR "/src/Aether"
RUN dotnet build "Aether.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aether.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aether.dll"]
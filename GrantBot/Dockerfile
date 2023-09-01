FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GrantBot/GrantBot.csproj", "GrantBot/"]
RUN dotnet restore "GrantBot/GrantBot.csproj"
COPY . .
WORKDIR "/src/GrantBot"
RUN dotnet build "GrantBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GrantBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GrantBot.dll"]

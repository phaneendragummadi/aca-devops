FROM mcr.microsoft.com/dotnet/aspnet:6.0.5-alpine3.14 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0.300-alpine3.14 AS build
WORKDIR /src
COPY ["Arcus.Demo.WebAPI.csproj", ""]

COPY . .
WORKDIR "/src/."
RUN dotnet build "Arcus.Demo.WebAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Arcus.Demo.WebAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Arcus.Demo.WebAPI.dll"]

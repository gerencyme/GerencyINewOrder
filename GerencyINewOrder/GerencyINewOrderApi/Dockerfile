FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:8000;http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
COPY ["GerencyINewOrder/GerencyINewOrderApi/GerencyINewOrderApi.csproj", "GerencyINewOrderApi/"]
COPY ["GerencyINewOrder/Domain/Domain.csproj", "Domain/"]
COPY ["GerencyINewOrder/Entities/Entities.csproj", "Entities/"]
COPY ["GerencyINewOrder/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore
COPY GerencyINewOrderApi.csproj .
RUN dotnet build "GerencyINewOrderApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GerencyINewOrderApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GerencyINewOrderApi.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:8000;http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copie os arquivos de projeto para evitar copiar todo o diretório '.' desnecessariamente
COPY ["GerencyINewOrder/GerencyINewOrderApi/GerencyINewOrderApi.csproj", "GerencyINewOrderApi/"]
COPY ["GerencyINewOrder/Domain/Domain.csproj", "Domain/"]
COPY ["GerencyINewOrder/Entities/Entities.csproj", "Entities/"]
COPY ["GerencyINewOrder/Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restaurar dependências
RUN dotnet restore

# Copiar todo o restante do código
COPY . .

# Build do projeto
RUN dotnet build "GerencyINewOrderApi.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Publicação do projeto
RUN dotnet publish "GerencyINewOrderApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Copiar os artefatos da publicação
COPY --from=publish /app/publish .

# Comando de entrada
ENTRYPOINT ["dotnet", "GerencyINewOrderApi.dll"]

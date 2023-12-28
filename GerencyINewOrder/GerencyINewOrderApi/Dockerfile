FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:8000;http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar os arquivos de projeto
COPY ["GerencyINewOrderApi/GerencyINewOrderApi.csproj", "GerencyINewOrderApi/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restaurar as dependências
RUN dotnet restore "GerencyINewOrderApi/GerencyINewOrderApi.csproj"

# Copiar o restante do código
COPY . .

# Mudar para o diretório do projeto
WORKDIR "/src/GerencyINewOrderApi"
RUN dotnet build "GerencyINewOrderApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GerencyINewOrderApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GerencyINewOrderApi.dll"]

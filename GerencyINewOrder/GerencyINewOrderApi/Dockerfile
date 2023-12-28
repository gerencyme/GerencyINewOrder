# Estágio 1: Construção da aplicação
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copiar os arquivos de projeto
COPY ["GerencyINewOrder/GerencyINewOrderApi/GerencyINewOrderApi.csproj", "GerencyINewOrderApi/"]
COPY ["GerencyINewOrder/Domain/Domain.csproj", "Domain/"]
COPY ["GerencyINewOrder/Entities/Entities.csproj", "Entities/"]
COPY ["GerencyINewOrder/Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restaurar dependências e compilar
RUN dotnet restore
COPY . .
RUN dotnet build "GerencyINewOrderApi/GerencyINewOrderApi.csproj" -c Release -o /app/build

# Estágio 2: Publicação da aplicação
FROM build AS publish
RUN dotnet publish "GerencyINewOrderApi/GerencyINewOrderApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 3: Configuração final
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
EXPOSE 80

# Copiar os artefatos da publicação
COPY --from=publish /app/publish .

# Comando de entrada
ENTRYPOINT ["dotnet", "GerencyINewOrderApi.dll"]

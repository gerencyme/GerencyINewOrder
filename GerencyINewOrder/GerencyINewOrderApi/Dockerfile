# Estágio 1: Construir a aplicação
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copiar os arquivos do projeto
COPY ["GerencyINewOrder/GerencyINewOrderApi/GerencyINewOrderApi.csproj", "GerencyINewOrderApi/"]
COPY ["GerencyINewOrder/Domain/Domain.csproj", "Domain/"]
COPY ["GerencyINewOrder/Entities/Entities.csproj", "Entities/"]
COPY ["GerencyINewOrder/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
# Estágio 2: Restaurar as dependências
RUN dotnet restore

# Estágio 3: Construir a aplicação
RUN dotnet build -c Release -o /app/build

# Estágio 4: Publicar a aplicação
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Estágio 5: Imagem final
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
EXPOSE 80

# Copiar os arquivos publicados do estágio de publicação
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "GerencyINewOrderApi.dll"]

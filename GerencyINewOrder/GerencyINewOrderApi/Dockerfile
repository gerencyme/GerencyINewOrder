# Estágio 1: Construir a aplicação
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copiar os arquivos do projeto e restaurar as dependências
COPY . .

# Copiar e restaurar as dependências dos projetos referenciados
WORKDIR /src/GerencyINewOrderApi
RUN dotnet restore

# Construir a aplicação
RUN dotnet build -c Release -o /app/build

# Estágio 2: Publicar a aplicação
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Estágio 3: Imagem final
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
EXPOSE 80

# Copiar os arquivos publicados do estágio de publicação
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "GerencyINewOrderApi.dll"]

# Estágio de Construção
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copie todos os arquivos para o contêiner
COPY . .

# Restaure as dependências
RUN dotnet restore

# Copie os arquivos do projeto principal
COPY ./GerencyINewOrderApi/GerencyINewOrderApi.csproj GerencyINewOrderApi/
COPY ./Domain/Domain.csproj Domain/
COPY ./Entities/Entities.csproj Entities/
COPY ./Infrastructure/Infrastructure.csproj Infrastructure/

# Restaure as dependências dos projetos
RUN dotnet restore

# Continue com o build dos projetos principais e adicionais
RUN dotnet build "GerencyINewOrderApi/GerencyINewOrderApi.csproj" -c Release -o /app/build
RUN dotnet build "Domain/Domain.csproj" -c Release -o /app/build
RUN dotnet build "Entities/Entities.csproj" -c Release -o /app/build
RUN dotnet build "Infrastructure/Infrastructure.csproj" -c Release -o /app/build

# Estágio de Publicação
FROM build AS publish
RUN dotnet publish "GerencyINewOrderApi/GerencyINewOrderApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio Final
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Copie os arquivos publicados do projeto principal
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "GerencyINewOrderApi.dll"]

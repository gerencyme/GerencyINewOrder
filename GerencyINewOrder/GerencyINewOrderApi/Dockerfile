# Stage 1: Build libraries
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-libs
WORKDIR /src

# Copy and restore class library projects
COPY GerencyINewOrderApi/GerencyINewOrderApi.csproj ./GerencyINewOrderApi/
COPY Domain/Domain.csproj ./Domain/
COPY Entities/Entities.csproj ./Entities/
COPY Infrastructure/Infrastructure.csproj ./Infrastructure/

RUN dotnet restore ./GerencyINewOrderApi/GerencyINewOrderApi.csproj
RUN dotnet restore ./Domain/Domain.csproj
RUN dotnet restore ./Entities/Entities.csproj
RUN dotnet restore ./Infrastructure/Infrastructure.csproj

# Copy the rest of the source code for libraries
COPY GerencyINewOrderApi/. ./GerencyINewOrderApi/
COPY Domain/. ./Domain/
COPY Entities/. ./Entities/
COPY Infrastructure/. ./Infrastructure/

# Build libraries
RUN dotnet build -c Release

# Stage 2: Build and publish Web API
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-api
WORKDIR /src

# Copy and restore Web API project
COPY GerencyINewOrderApi/GerencyINewOrderApi.csproj ./GerencyINewOrderApi/
RUN dotnet restore ./GerencyINewOrderApi/GerencyINewOrderApi.csproj

# Copy the rest of the source code for Web API
COPY GerencyINewOrderApi/. ./GerencyINewOrderApi/

# Reference libraries built in the previous stage
COPY --from=build-libs /src/Domain/bin/Release/net7.0/ ./GerencyINewOrderApi/References/Domain
COPY --from=build-libs /src/Entities/bin/Release/net7.0/ ./GerencyINewOrderApi/References/Entities
COPY --from=build-libs /src/Infrastructure/bin/Release/net7.0/ ./GerencyINewOrderApi/References/Infrastructure

# Build and publish Web API
RUN dotnet publish ./GerencyINewOrderApi/GerencyINewOrderApi.csproj -c Release -o /app

# Stage 3: Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build-api /app .

# Set up environment variables, expose ports, etc.
EXPOSE 80

# Define entry point
ENTRYPOINT ["dotnet", "GerencyINewOrderApi.dll"]

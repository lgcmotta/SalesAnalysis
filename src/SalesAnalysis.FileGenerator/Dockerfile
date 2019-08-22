FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-preview6-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview6-buster AS build
WORKDIR /src
COPY ["src/SalesAnalysis.FileGenerator/SalesAnalysis.FileGenerator.csproj", "src/SalesAnalysis.FileGenerator/"]
COPY ["src/SalesAnalysis.UnitOfWork/SalesAnalysis.UnitOfWork.csproj", "src/SalesAnalysis.UnitOfWork/"]
COPY ["src/SalesAnalysis.RabbitMQ/SalesAnalysis.RabbitMQ.csproj", "src/SalesAnalysis.RabbitMQ/"]
COPY ["src/SalesAnalysis.ServicesConfiguration/SalesAnalysis.ServicesConfiguration.csproj", "src/SalesAnalysis.ServicesConfiguration/"]
RUN dotnet restore "src/SalesAnalysis.FileGenerator/SalesAnalysis.FileGenerator.csproj"
COPY . .
WORKDIR "/src/src/SalesAnalysis.FileGenerator"
RUN dotnet build "SalesAnalysis.FileGenerator.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SalesAnalysis.FileGenerator.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SalesAnalysis.FileGenerator.dll"]
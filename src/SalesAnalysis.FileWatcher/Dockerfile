FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-preview6-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview6-buster AS build
WORKDIR /src
COPY ["src/SalesAnalysis.FileWatcher/SalesAnalysis.FileWatcher.csproj", "src/SalesAnalysis.FileWatcher/"]
COPY ["src/SalesAnalysis.UnitOfWork/SalesAnalysis.UnitOfWork.csproj", "src/SalesAnalysis.UnitOfWork/"]
COPY ["src/SalesAnalysis.ServicesConfiguration/SalesAnalysis.ServicesConfiguration.csproj", "src/SalesAnalysis.ServicesConfiguration/"]
RUN dotnet restore "src/SalesAnalysis.FileWatcher/SalesAnalysis.FileWatcher.csproj"
COPY . .
WORKDIR "/src/src/SalesAnalysis.FileWatcher"
RUN dotnet build "SalesAnalysis.FileWatcher.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SalesAnalysis.FileWatcher.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SalesAnalysis.FileWatcher.dll"]

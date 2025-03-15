#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pcf.Administration/Pcf.Administration.WebHost/Pcf.Administration.WebHost.csproj", "Pcf.Administration/Pcf.Administration.WebHost/"]
COPY ["Pcf.Administration/Pcf.Administration.Core/Pcf.Administration.Core.csproj", "Pcf.Administration/Pcf.Administration.Core/"]
COPY ["Pcf.Administration/Pcf.Administration.DataAccess/Pcf.Administration.DataAccess.csproj", "Pcf.Administration/Pcf.Administration.DataAccess/"]
COPY ["Pcf.Administration/Pcf.Administration.RabbitMQ/Pcf.Administration.RabbitMQ/Pcf.Administration.RabbitMQ.csproj", "Pcf.Administration/Pcf.Administration.RabbitMQ/Pcf.Administration.RabbitMQ/"]
RUN dotnet restore "./Pcf.Administration/Pcf.Administration.WebHost/./Pcf.Administration.WebHost.csproj"
COPY . .
WORKDIR "/src/Pcf.Administration/Pcf.Administration.WebHost"
RUN dotnet build "./Pcf.Administration.WebHost.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Pcf.Administration.WebHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pcf.Administration.WebHost.dll"]
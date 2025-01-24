#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/Pcf.GivingToCustomer.WebHost.csproj", "Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/"]
COPY ["Pcf.GivingToCustomer/Pcf.GivingToCustomer.Core/Pcf.GivingToCustomer.Core.csproj", "Pcf.GivingToCustomer/Pcf.GivingToCustomer.Core/"]
COPY ["Pcf.GivingToCustomer/Pcf.GivingToCustomer.DataAccess/Pcf.GivingToCustomer.DataAccess.csproj", "Pcf.GivingToCustomer/Pcf.GivingToCustomer.DataAccess/"]
COPY ["Pcf.GivingToCustomer/Pcf.GivingToCustomer.Integration/Pcf.GivingToCustomer.Integration.csproj", "Pcf.GivingToCustomer/Pcf.GivingToCustomer.Integration/"]
COPY ["Pcf.GivingToCustomer/Pcf.GivingToCustomer.RabbitMQ/Pcf.GivingToCustomer.RabbitMQ/Pcf.GivingToCustomer.RabbitMQ.csproj", "Pcf.GivingToCustomer/Pcf.GivingToCustomer.RabbitMQ/Pcf.GivingToCustomer.RabbitMQ/"]
RUN dotnet restore "./Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/./Pcf.GivingToCustomer.WebHost.csproj"
COPY . .
WORKDIR "/src/Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost"
RUN dotnet build "./Pcf.GivingToCustomer.WebHost.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Pcf.GivingToCustomer.WebHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pcf.GivingToCustomer.WebHost.dll"]
#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.WebHost/Pcf.ReceivingFromPartner.WebHost.csproj", "Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.WebHost/"]
COPY ["Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.Core/Pcf.ReceivingFromPartner.Core.csproj", "Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.Core/"]
COPY ["Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.DataAccess/Pcf.ReceivingFromPartner.DataAccess.csproj", "Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.DataAccess/"]
COPY ["Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.Integration/Pcf.ReceivingFromPartner.Integration.csproj", "Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.Integration/"]
COPY ["Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.RabbitMQ/Pcf.ReceivingFromPartner.RabbitMQ/Pcf.ReceivingFromPartner.RabbitMQ.csproj", "Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.RabbitMQ/Pcf.ReceivingFromPartner.RabbitMQ/"]
RUN dotnet restore "./Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.WebHost/./Pcf.ReceivingFromPartner.WebHost.csproj"
COPY . .
WORKDIR "/src/Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.WebHost"
RUN dotnet build "./Pcf.ReceivingFromPartner.WebHost.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Pcf.ReceivingFromPartner.WebHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pcf.ReceivingFromPartner.WebHost.dll"]
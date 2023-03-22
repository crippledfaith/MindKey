#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001
RUN apt-get update && apt-get install -y libfontconfig1

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["MindKey.Server/MindKey.Server.csproj", "MindKey.Server/"]
COPY ["MindKey.Client/MindKey.Client.csproj", "MindKey.Client/"]
COPY ["MindKey.Shared/MindKey.Shared.csproj", "MindKey.Shared/"]
COPY ["MindKey.WordCloudGenerator/MindKey.WordCloudGenerator.csproj", "MindKey.WordCloudGenerator/"]

RUN dotnet restore "MindKey.Server/MindKey.Server.csproj"
COPY . .
WORKDIR "/src/MindKey.Server"
RUN dotnet build "MindKey.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MindKey.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MindKey.Server.dll"]
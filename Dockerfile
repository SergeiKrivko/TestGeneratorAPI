FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TestGeneratorAPI.Web/TestGeneratorAPI.Web.csproj", "TestGeneratorAPI.Web/"]
COPY ["TestGeneratorAPI.DataAccess/TestGeneratorAPI.DataAccess.csproj", "TestGeneratorAPI.DataAccess/"]
COPY ["TestGeneratorAPI.Application/TestGeneratorAPI.Application.csproj", "TestGeneratorAPI.Application/"]
COPY ["TestGeneratorAPI.Core/TestGeneratorAPI.Core.csproj", "TestGeneratorAPI.Core/"]

RUN dotnet restore "TestGeneratorAPI.Web/TestGeneratorAPI.Web.csproj"

COPY . .

WORKDIR "/src/TestGeneratorAPI.Web"
RUN dotnet build "TestGeneratorAPI.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestGeneratorAPI.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

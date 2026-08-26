FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ClinicManagementSystem.API/ClinicManagementSystem.API.csproj", "ClinicManagementSystem.API/"]
COPY ["ClinicManagementSystem.Application/ClinicManagementSystem.Application.csproj", "ClinicManagementSystem.Application/"]
COPY ["ClinicManagementSystem.Domain/ClinicManagementSystem.Domain.csproj", "ClinicManagementSystem.Domain/"]
COPY ["ClinicManagementSystem.Infrastructure/ClinicManagementSystem.Infrastructure.csproj", "ClinicManagementSystem.Infrastructure/"]

RUN dotnet restore "ClinicManagementSystem.API/ClinicManagementSystem.API.csproj"

COPY . .

WORKDIR "/src/ClinicManagementSystem.API"

RUN dotnet publish "ClinicManagementSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "ClinicManagementSystem.API.dll"]
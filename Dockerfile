FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ProjectManagerAPI.sln ./
COPY src/Application/Application.csproj src/Application/
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/ProjectManagerAPI/ProjectManagerAPI.csproj src/ProjectManagerAPI/
COPY src/Security/Security.csproj src/Security/

RUN dotnet restore src/ProjectManagerAPI/ProjectManagerAPI.csproj

COPY src/ src/

WORKDIR /src/src/ProjectManagerAPI
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ProjectManagerAPI.dll"]
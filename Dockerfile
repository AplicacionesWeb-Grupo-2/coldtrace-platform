FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY coldtrace-platform.sln ./
COPY coldtrace-platform/coldtrace-platform.csproj coldtrace-platform/
RUN dotnet restore coldtrace-platform/coldtrace-platform.csproj

COPY coldtrace-platform/ coldtrace-platform/
WORKDIR /src/coldtrace-platform
RUN dotnet publish coldtrace-platform.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ColdTrace.Platform.dll"]

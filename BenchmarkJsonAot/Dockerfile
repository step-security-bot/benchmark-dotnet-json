FROM mcr.microsoft.com/dotnet/runtime:8.0@sha256:8cdcb5832eef96e276bc10fafc4092ee2826368d7b45b15b56d4bf529991ed2e AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:3bc4c8f13482237ab906d38dd9e290b4b1a093a2653ab3c28cca710b46510b9d AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BenchmarkJsonAot/BenchmarkJsonAot.csproj", "BenchmarkJsonAot/"]
RUN dotnet restore "BenchmarkJsonAot/BenchmarkJsonAot.csproj"
COPY . .
WORKDIR "/src/BenchmarkJsonAot"
RUN dotnet build "BenchmarkJsonAot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BenchmarkJsonAot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BenchmarkJsonAot.dll"]

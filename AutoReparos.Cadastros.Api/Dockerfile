# Stage 1: Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files and restore
COPY ["AutoReparos.Cadastros.Api/AutoReparos.Cadastros.Api.csproj", "AutoReparos.Cadastros.Api/"]
COPY ["AutoReparos.Cadastros.Application/AutoReparos.Cadastros.Application.csproj", "AutoReparos.Cadastros.Application/"]
COPY ["AutoReparos.Cadastros.Domain/AutoReparos.Cadastros.Domain.csproj", "AutoReparos.Cadastros.Domain/"]
COPY ["AutoReparos.Cadastros.Infrastructure/AutoReparos.Cadastros.Infrastructure.csproj", "AutoReparos.Cadastros.Infrastructure/"]

RUN dotnet restore "AutoReparos.Cadastros.Api/AutoReparos.Cadastros.Api.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/src/AutoReparos.Cadastros.Api"
RUN dotnet build "AutoReparos.Cadastros.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

# Stage 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AutoReparos.Cadastros.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

# Stage 4: Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AutoReparos.Cadastros.Api.dll"]

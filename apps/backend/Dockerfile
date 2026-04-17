# Multi-stage build para otimizar tamanho e performance
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copia apenas os arquivos de projeto primeiro (melhor cache de layers)
COPY TaskManager/TaskManager.csproj TaskManager/
RUN dotnet restore "TaskManager/TaskManager.csproj"

# Copia o resto dos arquivos e faz o build
COPY TaskManager/ TaskManager/
WORKDIR /src/TaskManager
RUN dotnet publish "TaskManager.csproj" -c Release -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Imagem final - usa Alpine para ser mais leve
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

# Instala bibliotecas ICU para suporte à globalização
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Cria usuário não-root para segurança
RUN addgroup -g 1000 appuser && adduser -u 1000 -G appuser -s /bin/sh -D appuser
USER appuser

EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.dll"]
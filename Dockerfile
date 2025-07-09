# Dockerfile para aplicação .NET + React (build frontend separado)
# Etapa 1: Build do backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
WORKDIR /src
COPY MoodTracking.sln ./
COPY MoodTracking.Api/*.csproj MoodTracking.Api/
COPY MoodTracking.Application/*.csproj MoodTracking.Application/
COPY MoodTracking.Domain/*.csproj MoodTracking.Domain/
COPY MoodTracking.Infra.Data/*.csproj MoodTracking.Infra.Data/
COPY MoodTracking.Infra.IoC/*.csproj MoodTracking.Infra.IoC/
COPY MoodTracking.Test/*.csproj MoodTracking.Test/
COPY . .
RUN dotnet restore
RUN dotnet publish MoodTracking.Api/MoodTracking.Api.csproj -c Release -o /app/publish

# Etapa 2: Build do frontend (React)
FROM node:20 AS build-frontend
WORKDIR /frontend
COPY MoodTracking.Api/wwwroot/react ./
RUN npm install && npm run build

# Etapa 3: Imagem final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build-backend /app/publish .
COPY --from=build-frontend /frontend/build ./wwwroot/react/build
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "MoodTracking.Api.dll"]

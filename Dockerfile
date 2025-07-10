# Dockerfile para rodar apenas o backend .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN mkdir db
COPY --from=build /app/publish .
COPY MoodTracking.Api/LocalStorage.sqlite ./db/LocalStorage.sqlite
RUN chmod 666 ./db/LocalStorage.sqlite
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "MoodTracking.Api.dll"]

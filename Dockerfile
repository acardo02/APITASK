FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY APITask/APITask.csproj APITask/
RUN dotnet restore APITask/APITask.csproj

COPY APITask/ APITask/
RUN dotnet publish APITask/APITask.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

USER $APP_UID
ENTRYPOINT ["dotnet", "APITask.dll"]

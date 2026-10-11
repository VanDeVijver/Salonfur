# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Salonfur.sln ./
COPY Salonfur.Core/Salonfur.Core.csproj Salonfur.Core/
COPY Salonfur.Web/Salonfur.Web.csproj Salonfur.Web/
RUN dotnet restore Salonfur.Web/Salonfur.Web.csproj

COPY . .
RUN dotnet publish Salonfur.Web/Salonfur.Web.csproj -c Release -o /app/publish --no-restore

# Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production \
    PORT=8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Salonfur.Web.dll"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Pedidos.Api.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/data && chown $APP_UID /app/data
USER $APP_UID
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ConnectionStrings__Default="Data Source=/app/data/pedidos.db"
EXPOSE 8080
ENTRYPOINT ["dotnet", "Pedidos.Api.dll"]

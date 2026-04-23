# build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copy csproj first for better layer caching
COPY Recipes.Api/Recipes.Api.csproj Recipes.Api/
RUN dotnet restore Recipes.Api/Recipes.Api.csproj

# copy everything else and publish
COPY . .
RUN dotnet publish Recipes.Api/Recipes.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# ASP.NET container images now default to port 8080 rather than 80
ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/publish .
EXPOSE 8080

ENTRYPOINT ["dotnet", "Recipes.Api.dll"]

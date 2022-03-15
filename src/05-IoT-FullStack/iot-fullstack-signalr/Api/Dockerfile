FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./Api/Api.csproj", "Api/"]
COPY ["./Base/Base.csproj", "Base/"]
COPY ["./Core/Core.csproj", "Core/"]
COPY ["./Persistence/Persistence.csproj", "Persistence/"]
RUN dotnet restore "Api/Api.csproj"
COPY . .
RUN dotnet publish "Api/Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app/publish .
COPY ["./Api/appsettings-docker.json", "/app/appsettings.json"] 
# TODO: Configure sqlite connection string
#COPY ["./Api/data/database.db", "/app/data/database.db"] 
ENTRYPOINT ["dotnet", "Api.dll"]
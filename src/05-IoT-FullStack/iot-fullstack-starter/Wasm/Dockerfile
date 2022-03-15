FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./Wasm/Wasm.csproj", "Wasm/"]
COPY ["./Base/Base.csproj", "Base/"]
COPY ["./Core/Core.csproj", "Core/"]
RUN dotnet restore "Wasm/Wasm.csproj"
COPY . .
RUN dotnet publish "Wasm/Wasm.csproj" -c Release -o /app/publish


FROM nginx:alpine
EXPOSE 80
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY ["./Wasm/appsettings_wasm-docker.json", "/usr/share/nginx/html/appsettings.json"] 

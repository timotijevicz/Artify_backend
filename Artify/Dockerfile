# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiraj csproj i restore (ovo ubrzava build zbog cache-a)
COPY ["Artify/Artify.csproj", "Artify/"]
RUN dotnet restore "Artify/Artify.csproj"

# Kopiraj ostatak koda i publish
COPY . .
WORKDIR /src/Artify
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Artify.dll"]

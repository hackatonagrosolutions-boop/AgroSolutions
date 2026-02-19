# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["PropriedadeService.csproj", "."]
RUN dotnet restore "PropriedadeService.csproj"
COPY . .
RUN dotnet publish "PropriedadeService.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "PropriedadeService.dll"]

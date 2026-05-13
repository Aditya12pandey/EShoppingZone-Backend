FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EShoppingZone.Wallet.API.csproj", "./"]
RUN dotnet restore "EShoppingZone.Wallet.API.csproj"
COPY . .
RUN dotnet build "EShoppingZone.Wallet.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EShoppingZone.Wallet.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "EShoppingZone.Wallet.API.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_14.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ["TodoApp.WebApp/TodoApp.WebApp.csproj", "TodoApp.WebApp/"]
RUN dotnet restore "TodoApp.WebApp/TodoApp.WebApp.csproj"
COPY . .
WORKDIR "/src/TodoApp.WebApp"
RUN dotnet build "TodoApp.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoApp.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoApp.WebApp.dll"]

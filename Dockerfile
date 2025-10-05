FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copy solution and project files (from current folder)
COPY *.sln ./
COPY *.csproj ./

RUN dotnet restore

# Copy all source code
COPY . .

# Publish the app (assuming your .csproj is named penkta.csproj)
RUN dotnet publish penkta.csproj -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "penkta.dll"]
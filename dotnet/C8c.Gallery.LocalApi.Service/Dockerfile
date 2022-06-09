FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR "/build"
    COPY [".","."]
    COPY ["./nuget.config", "/build/C8c.Gallery.LocalApi.Service"]
    RUN dotnet restore "/build/C8c.Gallery.LocalApi.Service/C8c.Gallery.LocalApi.Service.csproj"
    RUN dotnet build "/build/C8c.Gallery.LocalApi.Service/C8c.Gallery.LocalApi.Service.csproj" -c Release -o "/app/build"

    FROM build AS publish
    RUN dotnet publish "/build/C8c.Gallery.LocalApi.Service/C8c.Gallery.LocalApi.Service.csproj" -c Release -o "/app/publish"

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "C8c.Gallery.LocalApi.Service.dll"]
	
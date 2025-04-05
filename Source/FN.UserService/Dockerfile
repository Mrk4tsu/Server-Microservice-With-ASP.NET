# Chọn base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FN.UserService/FN.UserService.csproj", "FN.UserService/"]
RUN dotnet restore "FN.UserService/FN.UserService.csproj"

COPY . .
WORKDIR "/src/FN.UserService"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FN.UserService.dll"]

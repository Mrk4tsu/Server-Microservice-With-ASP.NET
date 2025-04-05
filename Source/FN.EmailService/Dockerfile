# Chọn base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FN.EmailService/FN.EmailService.csproj", "FN.EmailService/"]
RUN dotnet restore "FN.EmailService/FN.EmailService.csproj"

COPY . .
WORKDIR "/src/FN.EmailService"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FN.EmailService.dll"]

# Sử dụng hình ảnh ASP.NET Core Runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Sử dụng hình ảnh .NET SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép tệp cấu hình dự án và thực hiện restore dependency
COPY ["FN.CatalogService/FN.CatalogService.csproj", "FN.CatalogService/"]
RUN dotnet restore "FN.CatalogService/FN.CatalogService.csproj"

# Sao chép toàn bộ mã nguồn và build ứng dụng
COPY . .
WORKDIR "/src/FN.CatalogService"
RUN dotnet build -c Release -o /app/build

# Publish ứng dụng
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Tạo container cuối cùng để chạy ứng dụng
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FN.CatalogService.dll"]
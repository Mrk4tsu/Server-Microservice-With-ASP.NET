# Sử dụng hình ảnh ASP.NET Core Runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Sử dụng hình ảnh .NET SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép tệp cấu hình dự án và thực hiện restore dependency
COPY ["FN.OrderService/FN.OrderService.csproj", "FN.OrderService/"]
RUN dotnet restore "FN.OrderService/FN.OrderService.csproj"

# Sao chép tất cả mã nguồn và build ứng dụng
COPY . .
WORKDIR "/src/FN.OrderService"
RUN dotnet build -c Release -o /app/build

# Publish ứng dụng
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Tạo container cuối cùng để chạy ứng dụng
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FN.OrderService.dll"]
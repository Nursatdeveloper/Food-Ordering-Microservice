#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Image.Grpc.Service/Image.Grpc.Service.csproj", "Image.Grpc.Service/"]
RUN dotnet restore "Image.Grpc.Service/Image.Grpc.Service.csproj"
COPY . .
WORKDIR "/src/Image.Grpc.Service"
RUN dotnet build "Image.Grpc.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Image.Grpc.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Image.Grpc.Service.dll"]
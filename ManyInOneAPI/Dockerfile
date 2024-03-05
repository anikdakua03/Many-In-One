# ------------------- This is for when using docker compose ------------------
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# USER app
# WORKDIR /app
# EXPOSE 5000
# EXPOSE 7150

# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# ARG BUILD_CONFIGURATION=Release
# WORKDIR /src
# COPY ["ManyInOneAPI/ManyInOneAPI.csproj", "ManyInOneAPI/"]
# RUN dotnet restore "ManyInOneAPI/ManyInOneAPI.csproj"
# WORKDIR "/src/ManyInOneAPI"
# COPY . .
# RUN dotnet build "./ManyInOneAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# FROM build AS publish
# ARG BUILD_CONFIGURATION=Release
# RUN dotnet publish "./ManyInOneAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "ManyInOneAPI.dll"]


# ------------ This docker file is for when we run only with docker build -----------------------
#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# USER app
WORKDIR /app
# EXPOSE 80
# EXPOSE 443
# EXPOSE 5000
# EXPOSE 7150
# ENV ASPNETCORE_HTTPS_PORTS=7150
# ENV ASPNETCORE_HTTP_PORTS=5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ManyInOneAPI.csproj", "ManyInOneAPI/"]
RUN dotnet restore "ManyInOneAPI/ManyInOneAPI.csproj"
COPY . ./ManyInOneAPI
WORKDIR "/src/ManyInOneAPI"
RUN dotnet build "ManyInOneAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ManyInOneAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ManyInOneAPI.dll"]

# --------------------------- It is buildin image-------------------
#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# USER app
# WORKDIR /app
# EXPOSE 5000
# EXPOSE 7150

# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# ARG BUILD_CONFIGURATION=Release
# WORKDIR /src
# COPY ["ManyInOneAPI.csproj", "ManyInOneAPI/"]
# RUN dotnet restore "ManyInOneAPI/ManyInOneAPI.csproj"

# COPY . ./ManyInOneAPI

# WORKDIR "/src/ManyInOneAPI"
# RUN dotnet build "ManyInOneAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# FROM build AS publish
# ARG BUILD_CONFIGURATION=Release
# RUN dotnet publish "ManyInOneAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "ManyInOneAPI.dll"]
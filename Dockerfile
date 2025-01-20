# Base image for the .NET app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8082
EXPOSE 8083

# Build image for .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the entire solution directory (all projects: WebAPI, Application, Domain, Infrastructure)
COPY ./Shipping.API /src/Shipping.API
COPY ./Application /src/Application
COPY ./Domain /src/Domain
COPY ./Infrastructure /src/Infrastructure
COPY ./Common /src/Common

# Restore dependencies for all projects
RUN dotnet restore "Shipping.API/Shipping.API.csproj"

# Build the Web API
RUN dotnet build "Shipping.API/Shipping.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the Web API
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Shipping.API/Shipping.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Publish SQL project (.sqlproj)
FROM build AS sqlpublish
COPY ["./ShippingOrderDb/ShippingOrder/ShippingOrder.sqlproj", "ShippingOrderDb/"]
RUN dotnet build "ShippingOrderDb/ShippingOrder.sqlproj" -c $BUILD_CONFIGURATION -o /app/sqlbuild
RUN dotnet publish "ShippingOrderDb/ShippingOrder.sqlproj" -c $BUILD_CONFIGURATION -o /app/sqlpublish

# Final stage: combine everything
FROM base AS final
WORKDIR /app

# Copy the published .NET Web API
COPY --from=publish /app/publish .

# Optionally copy the published SQL project if needed
COPY --from=sqlpublish /app/sqlpublish .

# Set the entry point for your .NET Web API
ENTRYPOINT ["dotnet", "Shipping.API.dll"]

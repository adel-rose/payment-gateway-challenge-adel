# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Copy code into the image
COPY src/ .
WORKDIR /PaymentGateway.Api

# Restore packages
RUN dotnet restore PaymentGateway.Api.csproj

RUN dotnet build PaymentGateway.Api.csproj -c Release -o /app/build

RUN dotnet publish PaymentGateway.Api.csproj -c Release -o /app/publish 

# Stage 2: Runtime
#Compiles your C# source code into DLLs.
#
#Collects all runtime dependencies (NuGet packages, config, static files) needed to run the app.
#
#Places everything into a single folder (/app/publish in your image).


# This is the creation of the final image, lighter, contains only dotnet runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
USER root

# fix for: System.NotSupportedException: Globalization Invariant Mode is not supported.
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "PaymentGateway.Api.dll"]

# docker buildx build --load -t PaymentGateway:latest . (Use this command to run the container)
# docker run -d -p 5000:5000 --name PaymentGateway-container PaymentGateway:latest



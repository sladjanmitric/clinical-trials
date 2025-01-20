# Use the official .NET 8 SDK image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy the project files and restore dependencies
COPY ["ClinicalTrials.API/ClinicalTrials.API.csproj", "ClinicalTrials.API/"]
COPY ["ClinicalTrials.Domain/ClinicalTrials.Domain.csproj", "ClinicalTrials.Domain/"]
COPY ["ClinicalTrials.Application/ClinicalTrials.Application.csproj", "ClinicalTrials.Application/"]
COPY ["ClinicalTrials.Infrastructure/ClinicalTrials.Infrastructure.csproj", "ClinicalTrials.Infrastructure/"]
RUN dotnet restore "ClinicalTrials.API/ClinicalTrials.API.csproj"

# Copy the remaining source code and build the application
COPY . .
WORKDIR /source/ClinicalTrials.API
RUN dotnet publish "ClinicalTrials.API.csproj" -c Release -o /app/publish

# Use the official .NET 8 ASP.NET Core runtime image as a runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the application runs on
EXPOSE 8080
EXPOSE 8081

# Set the entry point for the container
ENTRYPOINT ["dotnet", "ClinicalTrials.API.dll"]
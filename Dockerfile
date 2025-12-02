# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy solution
COPY Rex.sln .

# Copy project files (faster restore)
COPY Rex.Presentation.Api/Rex.Presentation.Api.csproj Rex.Presentation.Api/
COPY Rex.Infrastructure.Persistence/Rex.Infrastructure.Persistence.csproj Rex.Infrastructure.Persistence/
COPY Rex.Infrastructure.Shared/Rex.Infrastructure.Shared.csproj Rex.Infrastructure.Shared/
COPY Rex.Domain/Rex.Domain.csproj Rex.Domain/
COPY Rex.Application/Rex.Application.csproj Rex.Application/

# Restore dependencies using solution file
RUN dotnet restore Rex.sln

# Copy remaining source
COPY . .

# Publish
RUN dotnet publish Rex.Presentation.Api/Rex.Presentation.Api.csproj \
    -c Release -o /app/publish

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV DOTNET_URLS=http://+:5286
EXPOSE 5286

RUN adduser --disabled-password --home /home/rexuser rexuser
USER rexuser

ENTRYPOINT ["dotnet", "Rex.Presentation.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY ["layers/Presentation/RoyaAi.Layers.Presentation.csproj", "layers/Presentation/"]
# Assuming there are other projects it depends on
COPY ["layers/Domain/RoyaAi.Layers.Domain.csproj", "layers/Domain/"]
# Add any other project references here
RUN dotnet restore "layers/Presentation/RoyaAi.Layers.Presentation.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/app/layers/Presentation"
RUN dotnet build "RoyaAi.Layers.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RoyaAi.Layers.Presentation.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "RoyaAi.Layers.Presentation.dll"] 
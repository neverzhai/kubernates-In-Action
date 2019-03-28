FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY CoreDocker/CoreDocker.csproj CoreDocker/
RUN dotnet restore CoreDocker/CoreDocker.csproj
COPY . .
WORKDIR /src/CoreDocker
RUN dotnet build CoreDocker.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish CoreDocker.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CoreDocker.dll"]

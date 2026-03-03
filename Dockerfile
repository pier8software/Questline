FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Questline.slnx ./
COPY src/Questline/Questline.csproj src/Questline/
RUN dotnet restore src/Questline/Questline.csproj

COPY src/Questline/ src/Questline/
COPY content/ content/
RUN dotnet publish src/Questline/Questline.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Questline.dll"]

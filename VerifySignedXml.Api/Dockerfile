#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["VerifySignedXml.Api/VerifySignedXml.Api.csproj", "VerifySignedXml.Api/"]
RUN dotnet restore "VerifySignedXml.Api/VerifySignedXml.Api.csproj"
COPY . .
WORKDIR "/src/VerifySignedXml.Api"
RUN dotnet build "VerifySignedXml.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VerifySignedXml.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VerifySignedXml.Api.dll"]
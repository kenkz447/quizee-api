#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:5.0-buster AS build
WORKDIR /src
COPY ["Quizee.API/Quizee.API.csproj", "Quizee.API/"]
COPY ["Quizee.Modules.UserPermissions/Quizee.Modules.UserPermissions.csproj", "Quizee.Modules.UserPermissions/"]
COPY ["Quizee.Modules/Quizee.Modules.csproj", "Quizee.Modules/"]
COPY ["Quizze.Libs/Quizze.Libs.csproj", "Quizze.Libs/"]
COPY ["Quizee.Utilities/Quizee.Utilities.csproj", "Quizee.Utilities/"]
COPY ["Quizee.Modules.Auth/Quizee.Modules.Auth.csproj", "Quizee.Modules.Auth/"]
RUN dotnet restore "Quizee.API/Quizee.API.csproj"
COPY . .
WORKDIR "/src/Quizee.API"
RUN dotnet build "Quizee.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Quizee.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Quizee.API.dll"]

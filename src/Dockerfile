#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

#COPY ["Notepad.PhysicalActivity.Api/Notepad.PhysicalActivity.Api.csproj", "Notepad.PhysicalActivity/"]
#RUN dotnet restore "Notepad.PhysicalActivity/Notepad.PhysicalActivity.Api.csproj"
#COPY . .
#WORKDIR "/src/Notepad.PhysicalActivity"
#RUN dotnet build "Notepad.PhysicalActivity.Api.csproj" -c Release -o /app/build

COPY . .

WORKDIR /src/Notepad.PhysicalActivity.Api
RUN dotnet restore
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

#FROM build AS publish
#RUN dotnet publish "Notepad.PhysicalActivity.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Notepad.PhysicalActivity.Api.dll"]

#new comment
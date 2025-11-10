# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia el archivo .csproj y restaura
COPY ["SoftWC.csproj", "./"]
RUN dotnet restore "SoftWC.csproj"

COPY . ./
RUN dotnet publish "SoftWC.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/out ./

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "SoftWC.dll"]
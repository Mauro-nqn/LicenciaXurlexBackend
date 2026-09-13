# 1. Usar el SDK oficial de .NET para compilar el código
FROM ://microsoft.com AS build
WORKDIR /app

# Copiar los archivos del proyecto y restaurar las dependencias
COPY *.sln ./
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto de los archivos y publicar los binarios
COPY . ./
RUN dotnet publish -c Release -o out

# 2. Crear la imagen final ligera para ejecutar la API
FROM ://microsoft.com AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Configurar el puerto por defecto de Render
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "LicenciaBackend.dll"]
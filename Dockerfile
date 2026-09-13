FROM ://microsoft.com AS build
WORKDIR /src

# Copiar todo el contenido del repositorio
COPY . .

# Buscar de forma automática cualquier archivo .csproj dentro de las carpetas y publicar
RUN dotnet publish -c Release -o /app/publish

FROM ://microsoft.com AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Este comando busca de forma automática el archivo compilado para arrancarlo sin importar el nombre exacto
ENTRYPOINT ["sh", "-c", "dotnet $(ls *.dll | head -n 1)"]

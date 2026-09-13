FROM ://microsoft.com AS build
WORKDIR /src

COPY . .

RUN dotnet publish -c Release -o /app/publish

FROM ://microsoft.com AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["sh", "-c", "dotnet $(ls *.dll | head -n 1)"]

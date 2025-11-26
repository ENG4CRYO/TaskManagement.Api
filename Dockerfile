
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src


COPY . .


RUN dotnet restore
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .


ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080


ENTRYPOINT ["dotnet", "TaskManagement.Api.dll"]
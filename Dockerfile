FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["PRN232.LMS.API/PRN232.LMS.API.csproj","PRN232.LMS.API/"]
COPY ["PRN232.LMS.Service/PRN232.LMS.Service.csproj","PRN232.LMS.Service/"]
COPY ["PRN232.LMS.Repositories/PRN232.LMS.Repositories.csproj","PRN232.LMS.Repositories/"]

RUN dotnet restore "PRN232.LMS.API/PRN232.LMS.API.csproj"

COPY . .

WORKDIR "/src/PRN232.LMS.API"

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet","PRN232.LMS.API.dll"]
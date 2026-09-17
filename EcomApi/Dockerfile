FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY EcomApi.sln ./
COPY Application/Application.csproj Application/
COPY Domain/Domain.csproj Domain/
COPY InfraData/InfraData.csproj InfraData/
COPY Infrastructure.IoC/Infrastructure.IoC.csproj Infrastructure.IoC/
COPY Web.Mvc/Web.Mvc.csproj Web.Mvc/
RUN dotnet restore Web.Mvc/Web.Mvc.csproj

COPY . .
RUN dotnet publish Web.Mvc/Web.Mvc.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Web.Mvc.dll"]
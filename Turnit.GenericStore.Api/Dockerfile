FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY Turnit.GenericStore.Api.csproj Turnit.GenericStore.Api/
RUN dotnet restore "Turnit.GenericStore.Api/Turnit.GenericStore.Api.csproj"
COPY . ./Turnit.GenericStore.Api/
WORKDIR "/src/Turnit.GenericStore.Api"
RUN dotnet build "Turnit.GenericStore.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Turnit.GenericStore.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Turnit.GenericStore.Api.dll"]

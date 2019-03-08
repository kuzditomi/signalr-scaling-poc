FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY [".", "SignalrScalingPoc.RabbitPublisher/"]
RUN dotnet restore "SignalrScalingPoc.RabbitPublisher/SignalrScalingPoc.RabbitPublisher.csproj"
COPY . .
WORKDIR "/src/SignalrScalingPoc.RabbitPublisher"
RUN dotnet build "SignalrScalingPoc.RabbitPublisher.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SignalrScalingPoc.RabbitPublisher.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SignalrScalingPoc.RabbitPublisher.dll"]
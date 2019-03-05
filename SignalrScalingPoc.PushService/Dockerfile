FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY [".", "SignalrScalingPoc.PushService/"]
RUN dotnet restore "SignalrScalingPoc.PushService/SignalrScalingPoc.PushService.csproj"
COPY . .
WORKDIR "/src/SignalrScalingPoc.PushService"
RUN dotnet build "SignalrScalingPoc.PushService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SignalrScalingPoc.PushService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SignalrScalingPoc.PushService.dll"]
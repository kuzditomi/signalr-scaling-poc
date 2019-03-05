const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:8080/myHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(function () {
    console.log("connected");
});
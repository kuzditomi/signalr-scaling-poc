document.addEventListener('DOMContentLoaded', () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:8080/myHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    const logs = [];

    connection.on('Pong', function () {
        log("Pong received.")
    });

    function log(message) {
        const timestamp = new Date().toLocaleTimeString();
        const log = `${timestamp}: ${message}`;
        logs.push(log);

        document.getElementById('log').innerText = logs.join('\r\n');
    }

    function connect() {
        log('connecting...');

        connection
            .start()
            .then(() => {
                log('connected!');
                document.getElementById('connectbtn').setAttribute('disabled', 'true');
                document.getElementById('pingbtn').removeAttribute('disabled');
            })
            .catch((error) => {
                log(`error connecting : ${error}`);
            });
    }
    
    function ping() {
        log("Sending Ping...");

        connection
            .invoke('Ping')
            .catch(error => {
                log(`Ping failed: ${error}`)
            });
    }

    document.getElementById('connectbtn').onclick = connect;
    document.getElementById('pingbtn').onclick = ping;
});
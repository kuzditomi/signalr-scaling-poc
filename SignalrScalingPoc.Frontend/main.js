document.addEventListener('DOMContentLoaded', () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:8080/myHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    const logs = [];

    connection.on('Pong', function () {
        log("Pong received.")
    });

    connection.on('ReceiveMessage', function (message, name) {
        log(`Message received from ${name}: ${message}`);
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
                document.getElementById('connectbtn').style.display = 'none';
                document.getElementById('pingbtn').style.display = 'inline';
                document.getElementById('messagesending').style.display="block";
                document.getElementById('txt').focus();
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

    function sendMessage(evt) {
        const message = document.getElementById('txt').value;

        connection
            .invoke("SendMessage", message)
            .catch(error => {
                log(`Error sending message: ${error}`);
            })
            .finally(() => {
                document.getElementById('txt').value = "";
                document.getElementById('txt').focus();
            });

        evt.preventDefault();
    }

    document.getElementById('connectbtn').onclick = connect;
    document.getElementById('pingbtn').onclick = ping;
    document.getElementById('sendform').onsubmit = sendMessage;
});
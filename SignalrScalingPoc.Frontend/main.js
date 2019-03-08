document.addEventListener('DOMContentLoaded', () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:9911/myHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    const logs = [];

    connection.on('Pong', function () {
        log("Pong received.")
    });

    connection.on('ReceiveMessage', function (message, name) {
        log(`Message received from push node:${name}: ${message}`);
    });

    function log(message) {
        const logElement = document.getElementById('log');
        const timestamp = new Date().toLocaleTimeString();
        const log = `${timestamp}: ${message}`;
        logs.push(log);

        logElement.innerText = logs.join('\r\n');
        logElement.scrollTo(0, logElement.scrollHeight);        
    }

    function connect() {
        log('connecting...');

        connection
            .start()
            .then(() => {
                log('connected!');
                document.getElementById('connectbtn').style.display = 'none';
                document.getElementById('pingbtn').style.display = 'inline';
                document.getElementById('messagesending').style.display = "block";
                document.getElementById('rabbitbtn').style.display = "inline";
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

    function sendRabbitMessages(evt) {
        fetch("http://localhost:8081/api/messages", {
            method: "POST",
            mode: "cors", 
            cache: "no-cache",
            // credentials: "same-origin", // include, *same-origin, omit
            headers: {
                "Content-Type": "application/json",
                // "Content-Type": "application/x-www-form-urlencoded",
            },
            referrer: "no-referrer", // no-referrer, *client
            //body: JSON.stringify(data), // body data type must match "Content-Type" header
        }).then(()=>{
            log('Rabbit messages sent.');
        }).catch((e)=>{
            log(`Error with rabbit message sending: ${e}`)
        });

        evt.preventDefault();
    }

    document.getElementById('connectbtn').onclick = connect;
    document.getElementById('pingbtn').onclick = ping;
    document.getElementById('sendform').onsubmit = sendMessage;
    document.getElementById('rabbitbtn').onclick = sendRabbitMessages;
});
class ChatClient {
    constructor(user_token, chat_id) {
        let target = 'ws://95.47.167.113:5124';
        this.state = "wait_connection_response";
        this.chat_id = chat_id;
        this.requestChatClientConnection = this.requestChatClientConnection.bind(this);
        this.browserWebSocket = new WebSocket(`${target}/requestChatSocket?token=${user_token}`);
        this.browserWebSocket.onmessage = this.handleResponse;
        this.browserWebSocket.onopen = () => this.requestChatClientConnection();
    }

    requestChatClientConnection() {
        const request = {
            "type": "connectToChatRoom",
            "chatId": this.chat_id,
        };
        const json = JSON.stringify(request);
        this.browserWebSocket.send(json);
    }

    handleResponse(event) {
        switch (this.state) {
            case "wait_connection_response":
                this.onChatConnectionResponse(event);
                break;
            case "connection_established":
                this.handleChatRoomMessage(event);
                break;
        }
    }

    onChatConnectionResponse(event) {
        const message = JSON.parse(event.data);
        if (message["type"] === "connectionEstablished")
            this.state = "connection_established";
    }

    handleChatRoomMessage(event) {
        console.log(event);
    }
}

module.exports = ChatClient;
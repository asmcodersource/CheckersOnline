export default class ChatClient {
    constructor(user_token, chat_id) {
        let target = '';
        this.state = "wait_connection_response";
        this.chat_id = chat_id;
        this.onmessage = null;
        this.requestChatClientConnection = this.requestChatClientConnection.bind(this);
        this.handleResponse = this.handleResponse.bind(this);
        this.browserWebSocket = new WebSocket(`${target}/requestChatSocket?token=${user_token}`);
        this.browserWebSocket.onmessage = this.handleResponse;
        this.browserWebSocket.onopen = this.requestChatClientConnection;
        this.browserWebSocket.onclose = function (event) {
            console.log('Connection closed with code:', event.code);
        };
    }

    async requestChatClientConnection() {
        console.log("chat oppened");
        const request = {
            "type": "connectToChatRoom",
            "chatId": this.chat_id,
        };
        const json = JSON.stringify(request);
        await this.browserWebSocket.send(json);
    }

    handleResponse(event) {
        switch (this.state) {
            case "wait_connection_response":
                this.onChatConnectionResponse(event);
                break;
            case "connection_established":
                this.handleChatRoomMessage(event);
                break;
            default:
                break;
        }
    }

    onChatConnectionResponse(event) {
        const message = JSON.parse(event.data);
        if (message["type"] === "connectionEstablished")
            this.state = "connection_established";
    }

    handleChatRoomMessage(event) {
        const message = JSON.parse(event.data);
        this.onmessage(message);
    }

    async sendMessage(message) {
        let json = JSON.stringify(message);
        await this.browserWebSocket.send(json);
    }
}

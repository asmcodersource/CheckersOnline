import React, { Component } from 'react';
import './Chat.css';

const ChatClient = require('./ChatClient');

export class Chat extends Component {
    constructor(props) {
        super(props);
        this.chatClient = null;
        this.state = { messages: [] }
        this.addMessageClient = this.addMessageClient.bind(this);
    }

    render() {
        return (
            <div className="chat-wrapper">
                <div className="chat-header">
                    <label>Чат кімнати</label>
                </div>
                <div className="chat-messagebox">
                    {this.state.messages}
                </div>
                <div className="chat-inputwrapper">
                    <input type="text" placeholder="Поле введення повідомлення"/>
                </div>
            </div>
        );
    }

    addMessageClient(rawMessage) {
        let messageObject = null;
        switch (rawMessage["Message"]["Type"]) {
            case "ChatInformationMessage":
                messageObject = <InformationalMessage content={rawMessage["Message"]["Content"]} />
                break;
            case "ChatClientMessage":
                messageObject = <ClientMessage sender="asd" content={rawMessage["Message"]["Content"]} />
                break;
            default:
                break;
        }
        this.setState((prewState) => prewState.messages.push(messageObject))
    }

    connectToChatRoom(id, token) {
        this.chatClient = new ChatClient(token, id)
        this.chatClient.onmessage = this.addMessageClient;
    }
}

export class ClientMessage extends Component {
    render() {
        return (
        <div className="message-wrapper">
            <span className="username">{this.props.sender}: </span>
            <span className="message-content">
                    {this.props.content}
            </span>
            </div>
        );
    }
}

export class InformationalMessage extends Component {
    render() {
        return (
            <div className="message-wrapper">
                <span className="message-content">
                    {this.props.content}
                </span>
            </div>
        );
    }
}
import React, { Component } from 'react';
import MetaTags from 'react-meta-tags';
import './Chat.css';

const ChatClient = require('./ChatClient');

export class Chat extends Component {
    constructor(props) {
        super(props);
        this.chatClient = null;
        this.state = { messages: [] }
        this.addMessageClient = this.addMessageClient.bind(this);
        this.onInputKeyDown = this.onInputKeyDown.bind(this);
        this.chatRef = React.createRef();
        this.chatInputRef = React.createRef();
    }

    render() {
        return (
            <div className="chat-wrapper">
                <div className="chat-header">
                    <label>Чат кімнати</label>
                </div>
                <div className="chat-messagebox" ref={this.chatRef}> {/* Добавляем ref */}
                    {this.state.messages}
                </div>
                <div className="chat-inputwrapper">
                    <input type="text" placeholder="Поле введення повідомлення" ref={this.chatInputRef} onKeyPress={this.onInputKeyDown} />
                </div>
            </div>
        );
    }

    componentDidUpdate() { // Вызывается после обновления компонента
        this.scrollToBottom();
    }

    onInputKeyDown(event) {
        if (event.key !== 'Enter')
            return;
        let msg = this.chatInputRef.current.value;
        this.chatInputRef.current.value = "";
        let request = {
            type: "roomSide",
            action: "broadcastMessage",
            content: {
                message: msg,
            },
        };
        this.chatClient.sendMessage(request);
    }

    scrollToBottom() {
        this.chatRef.current.scrollTop = this.chatRef.current.scrollHeight;
    }

    addMessageClient(rawMessage) {
        let messageObject = null;
        switch (rawMessage["Message"]["Type"]) {
            case "ChatInformationMessage":
                messageObject = <InformationalMessage content={rawMessage["Message"]["Content"]} />
                break;
            case "ChatClientMessage":
                messageObject = <ClientMessage sender={rawMessage["Message"]["Sender"]} content={rawMessage["Message"]["Content"]} />
                break;
            default:
                break;
        }
        this.setState((prevState) => ({
            messages: [...prevState.messages, messageObject]
        }), this.scrollToBottom); // Вызываем прокрутку после обновления состояния
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
                <span className="message-content client-message">
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
                <span className="message-content informational-message">
                    {this.props.content}
                </span>
            </div>
        );
    }
}

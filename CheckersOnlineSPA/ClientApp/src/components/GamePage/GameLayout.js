import React, { Component } from 'react';
import { CheckersField } from './GameField/CheckersField';
import { RightSideMenu } from './RightSideMenu/RightSideMenu';
import './GameLayout.css';

export class GameLayout extends Component {
    constructor(props) {
        super(props);
        this.gameWebSocket = null;
        this.tryLoginByStoredToken = this.tryLoginByStoredToken.bind(this);
        this.state = { user: null, games: [], isRoomCreated: false }
        this.checkersFieldRef = React.createRef();
    }


    render() {
        return (
            <div className='game-layout'>
                <div className="game-layout-wrapper">
                    <div className='menu-field-wrapper'>
                        <CheckersField ref={this.checkersFieldRef} />
                        <RightSideMenu />
                    </div>
                </div>
            </div>
        );
    }

    initializeField() {
        for (let y = 0; y < 3; y++) {
            for (let x = 0; x < 8; x++) {
                if ((x + y) % 2 == 0)
                    continue;
                this.checkersFieldRef.current.createChecker({ type: 'Checker', position: { cellX: x, cellY: y }, color:'Black' });
            }
        }

        for (let y = 5; y < 8; y++) {
            for (let x = 0; x < 8; x++) {
                if ((x + y) % 2 == 0)
                    continue;
                this.checkersFieldRef.current.createChecker({ type: 'Checker', position: { cellX: x, cellY: y }, color: 'White' });
            }
        }
    }

    async componentDidMount() {
        this.initializeField();
        await this.tryLoginByStoredToken();

        //this.checkersFieldRef.current.removeChecker({ cellX: 1, cellY: 0 })
        //this.checkersFieldRef.current.moveChecker({ cellX: 1, cellY: 2 }, { cellX: 0, cellY: 3 })
    }

     
    async ConnectToRoom() {
        const data = { type: "createRoom" };
        this.gameWebSocket.send(data)
    }

    async createGameWebsocket() {
        const token = sessionStorage.getItem('token')
        if (this.gameWebSocket != null)
            this.gameWebSocket.close();


        let target = 'ws://95.47.167.113:5124';
        this.gameWebSocket = new WebSocket(`${target}/requestgamesocket?token=${token}`);
        this.gameWebSocket.onopen = async (event) => {
            await new Promise(resolve => setTimeout(resolve, 500));
            await this.gameWebSocket.send(JSON.stringify({ type: "connectToRoom" }))
        };
        this.gameWebSocket.onmessage = (event) => {
            try {
                const message = JSON.parse(event.data);
                console.log(message);
            } catch (err) {
                console.log(err);
            }
        }
    }

    async tryLoginByStoredToken() {
        const token = sessionStorage.getItem('token')
        console.log(token);
        const response = await fetch("/tokenvalidation", {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json",
                "Authorization": "Bearer " + token
            },
        });
        if (response.ok === true) {
            console.log(response);
            const data = await response.json();
            this.setState({ user: { id: data.id, nickname: data.userName, email: data.email } })
            this.createGameWebsocket();
            return true
        } else {
            return false;
        }
    }
}
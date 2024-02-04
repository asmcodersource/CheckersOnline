import React, { Component } from 'react';
import { CheckersField } from './GameField/CheckersField';
import { RightSideMenu } from './RightSideMenu/RightSideMenu';
import './GameLayout.css';

export class GameLayout extends Component {
    constructor(props) {
        super(props);

        this.gameWebSocket = null;
        this.tryLoginByStoredToken = this.tryLoginByStoredToken.bind(this);
        this.mouseClicked = this.mouseClicked.bind(this);
        this.state = {
            user: null, games: [],
            isRoomCreated: false,
            firstClickValues: {},
            firstClick: null
        }
        this.checkersFieldRef = React.createRef();
    }


    render() {
        return (
            <div className='game-layout'>
                <div className="game-layout-wrapper">
                    <div className='menu-field-wrapper'>
                        <CheckersField ref={this.checkersFieldRef} mouseClicked={this.mouseClicked} />
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


    mouseClicked(row, column) {
        if (this.state.firstClick == null) {
            this.setState({ firstClick: { row, column } });
        } else {
            let firstClickValues = this.state.firstClick;
            this.setState({ firstClick: null });
            let secondClickValues = { row, column };

            let jsonObject = JSON.stringify({
                "type": "makeAction",
                "firstPosition": firstClickValues,
                "secondPosition": secondClickValues,
            });

            this.gameWebSocket.send(jsonObject);
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


        let target = 'ws://192.168.0.100:44463';
        this.gameWebSocket = new WebSocket(`${target}/requestgamesocket?token=${token}`);
        this.gameWebSocket.onopen = async (event) => {
            await new Promise(resolve => setTimeout(resolve, 500));
            await this.gameWebSocket.send(JSON.stringify({ type: "connectToRoom" }))
        };
        this.gameWebSocket.onmessage = (event) => {
            try {
                const message = JSON.parse(event.data);
                console.log(message);
                if (message["type"] == "moveAction") {
                    let x1 = message["firstPosition"]["column"];
                    let y1 = message["firstPosition"]["row"];
                    let x2 = message["secondPosition"]["column"];
                    let y2 = message["secondPosition"]["row"];
                    this.checkersFieldRef.current.moveChecker({ cellX: x1, cellY: y1 }, { cellX: x2, cellY: y2 })
                } else if (message["type"] == "removeAction") {
                    let x1 = message["removePosition"]["column"];
                    let y1 = message["removePosition"]["row"];
                    this.checkersFieldRef.current.removeChecker({ cellX: x1, cellY: y1 })
                }
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
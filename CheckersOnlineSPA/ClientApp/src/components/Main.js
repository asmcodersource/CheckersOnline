import React, { Component } from 'react';
import { UserInfo } from './UserInfo/UserInfo.js'
import { Login } from './Login/Login.js';
import { Register } from './Register/Register.js';
import { Browser } from './Browser/Browser.js'
import { NewGame } from './NewGame/NewGame.js';
import { CreateRoom } from './CreateRoom/CreateRoom.js';
import './Main.css'

export class Main extends Component {

  constructor(props) {
      super(props)
      this.browserWebsocker = null;
      this.state = { user: null, games: [], isRoomCreated : false }
      this.loginFormRef = React.createRef();  
      this.registerFormRef = React.createRef();  
      this.userInfoRef = React.createRef();
      this.browserRef = React.createRef();
      this.createRoomRef = React.createRef();
      this.tryLoginByStoredToken = this.tryLoginByStoredToken.bind(this);
      this.logoutHandler = this.logoutHandler.bind(this);
      this.createRoom = this.createRoom.bind(this);
      this.removeRoom = this.removeRoom.bind(this);
  }

  render() {
    return (
        <div className='main-container'>
            <div className='main-container-inner'>
                <CreateRoom ref={this.createRoomRef} createRoomHandler={this.createRoom} />
                <Login ref={this.loginFormRef} loginHandler={this.tryLoginByStoredToken} />
                <Register ref={this.registerFormRef} registerHandler={this.tryLoginByStoredToken} />
                <UserInfo
                    user={this.state.user}
                    ref={this.userInfoRef}
                    loginHandler={() => this.loginFormRef.current.showDialog()}
                    registerHandler={() => this.registerFormRef.current.showDialog()}
                    logoutHandler={this.logoutHandler}
                />
                <div className="browser-newgame-wrapper">
                    <Browser games={this.state.games} />
                    <NewGame
                        cancelClickedHandler={() => this.removeRoom() }
                        isRoomCreated={this.state.isRoomCreated}
                        playWithHumanHandler={() => this.createRoomRef.current.showDialog()}
                    />
                </div>
            </div>
      </div>
    );
  }

    async componentDidMount() {
        await this.tryLoginByStoredToken();
        this.createBrowserWebsocket();
    }


    async removeRoom() {
        this.setState({ isRoomCreated: false });
        const data = { type: "removeRoom" };
        const jsonMessage = JSON.stringify(data);
        await this.browserWebsocker.send(jsonMessage);
    }


    async createRoom(room) {
        this.setState({ isRoomCreated: true });
        const data = { type: "createRoom", data: room };
        const jsonMessage = JSON.stringify(data);
        await this.browserWebsocker.send(jsonMessage);
    }

    async createBrowserWebsocket() {
        const token = sessionStorage.getItem('token')
        if (this.browserWebsocker != null)
            this.browserWebsocker.close();


        let target = 'ws://95.47.167.113:5124';
        this.browserWebsocker = new WebSocket(`${target}/ws?token=${token}`);
        this.browserWebsocker.onopen = async (event) => {
            await new Promise(resolve => setTimeout(resolve, 500));

            const data1 = { type: "getAllRooms" }
            const jsonMessage1 = JSON.stringify(data1);
            await this.browserWebsocker.send(jsonMessage1);
        };
        this.browserWebsocker.onmessage = (event) => {
            try {
                const message = JSON.parse(event.data);
                if (message["type"] === "addRoom") {
                    const room = message["data"]
                    this.state.games.push(room);
                    this.setState({ games: this.state.games });
                } else if (message["type"] === "allRooms") {
                    const rooms = message["data"]
                    this.setState({ games: rooms });
                } else if (message["type"] === "removeRoom") {
                    var roomId = message["data"]["id"];
                    const rooms = this.state.games.filter((room) => room.id !== roomId);
                    this.setState({ games: rooms });
                }
            } catch (err) {
                console.log(err);
            }
        }
    }

    async tryLoginByStoredToken() {
        const token = sessionStorage.getItem('token')
        const response = await fetch("/tokenvalidation", {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json",
                "Authorization": "Bearer " + token
            },
        });
        if (response.ok === true) {
            const data = await response.json();
            this.setState({ user: { id: data.id, nickname: data.userName, email: data.email } })
            this.createBrowserWebsocket();
            return true
        } else {
            return false;
        }
    }

    logoutHandler() {
        this.removeRoom();
        this.browserWebsocker.close();
        sessionStorage.removeItem('token');
        this.setState({ user: null });
    }
}

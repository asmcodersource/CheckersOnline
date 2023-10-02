import React, { Component, setState } from 'react';
import { UserInfo } from './UserInfo/UserInfo.js'
import { Login } from './Login/Login.js';
import { Register } from './Register/Register.js';
import { Browser } from './Browser/Browser.js'
import { NewGame } from './NewGame/NewGame.js';
import './Main.css'

export class Main extends Component {

  constructor(props) {
      super(props)
      this.browserWebsocker = null;
      this.state = { user: null, games : [] }
      this.loginFormRef = React.createRef();  
      this.registerFormRef = React.createRef();  
      this.userInfoRef = React.createRef();
      this.browserRef = React.createRef();
      this.tryLoginByStoredToken = this.tryLoginByStoredToken.bind(this);
      this.logoutHandler = this.logoutHandler.bind(this);
  }

  render() {
    return (
        <div className='main-container'>
            <div className='main-container-inner'>
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
                    <NewGame />
                </div>
            </div>
      </div>
    );
  }

    async componentDidMount() {
        await this.tryLoginByStoredToken();
        this.createBrowserWebsocket();
    }

    async createBrowserWebsocket() {
        const token = sessionStorage.getItem('token')
        if (this.browserWebsocker != null)
            this.browserWebsocker.close();

        await fetch('/api/webSocketTarget')
            .then(response => response.json())
            .then(data => {
                let target = data["target"];
                target = target.replace(/http:\/\//g, "ws://");
                target = 'ws://192.168.0.100:5124';
                this.browserWebsocker = new WebSocket(`${target}/ws?token=${token}`);
                this.browserWebsocker.onopen = async (event) => {
                    const data = { type: "createRoom", data: { title: "Room", description: "Alalaa" } };
                    const jsonMessage = JSON.stringify(data);
                    await this.browserWebsocker.send(jsonMessage);

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
                };
            })
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
        sessionStorage.removeItem('token');
        this.setState({ user: null });
    }
}

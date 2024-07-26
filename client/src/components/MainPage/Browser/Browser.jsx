import React, { Component } from 'react';
import {Game} from './Game'
import './Browser.css'

export class Browser extends Component {
    constructor(props) {
        super(props)
        this.state = {
            availableGames: 0,
            activeGames: 0,
            clientsOnline: 0,
        }

        this.requestSiteStatistic = this.requestSiteStatistic.bind(this);
        this.interval = setInterval(this.requestSiteStatistic, 5000);
        this.ws = null;
    }

    render() {
        let games = this.props.games.map((game) => <Game key={"game=" + game.creatorId} {...game} claimRoom={this.props.claimRoom} />)
        return (
            <div className="browser-wrapper">
                <div className="browser-header">
                <h1>Games browser</h1>
                <hr />
                <div className="info-wrapper">
                    <span>Games: {this.state.activeGames},</span>
                    <span>Players: {this.state.clientsOnline}</span>
                </div>
                </div>
                <div className="games-wrapper">
                    {games}
                </div>
            </div>
        );
    }

    componentWillUnmount() {
        clearInterval(this.interval)
        this.ws.close();
    }

    initHeartbeatConnect() {
        this.ws = new WebSocket("ws://localhost:5124/ws");
    }

    async requestSiteStatistic() {
        const response = await fetch("/statistic", {
            method: "POST",
            headers: { "Accept": "application/json" }
        });
        if (response.ok === true) {
            const data = await response.json();
            this.setState({ clientsOnline: data.clientsOnline, activeGames: data.gamesOnline })
        }
    }
}



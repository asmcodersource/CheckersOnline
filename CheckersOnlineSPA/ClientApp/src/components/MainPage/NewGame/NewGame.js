import React, { Component } from 'react';
import './bot.png'
import humansPicture from './humans.png'
import botPicture from './bot.png'
import './NewGame.css'


export class NewGame extends Component {
    constructor(props) {
        super(props);
        this.state = {
            loadingAnimation: ""
        }
        this.animationHandler = this.animationHandler.bind(this);
        this.animationInterval = setInterval(this.animationHandler, 700)
    }

    render() {
        let state = this.props.isRoomCreated ? "waiting_for_opponent" : "select_game_type";
        if (state === "select_game_type") {
            return (
                <div className="new-game">
                    <h1>New game</h1>
                    <hr />
                    <div className="game-type-wrapper">
                        <div className="game-type">
                            <img src={humansPicture}></img>
                            <div onClick={this.props.playWithHumanHandler} className="button">Play with human</div>
                        </div>
                        <div className="game-type">
                            <img src={botPicture}></img>
                            <div onClick={this.props.playWithBotHandler} className="button">Play with bot</div>
                        </div>
                    </div>
                </div>
            )
        } else if (state === "waiting_for_opponent") {
            return (
                <div className="new-game">
                    <h1>New game</h1>
                    <hr />
                    <div className="game-opponent-waiting-wrapper">
                        <label>Waiting for opponent{this.state.loadingAnimation}</label>
                    </div>
                    <button onClick={this.props.cancelClickedHandler} className="button">Cancel</button>
                </div>
            )
        }
    }

    animationHandler() {
        let animState = this.state.loadingAnimation;
        if (animState.length > 2)
            animState = ""
        else
            animState += '.'
        this.setState({ loadingAnimation: animState });
    }
}

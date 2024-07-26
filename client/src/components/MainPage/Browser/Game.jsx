import React, { Component } from 'react';
import DefaultUserPicture from '../UserInfo/defaultUserPic.png'
import './Game.css'

export class Game extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="game" onClick={() => this.props.claimRoom(this.props.creatorId)}>
                <div className="avatar-info-wrapper">
                    <div className="user-picture"><img src={DefaultUserPicture}></img></div>
                    <div className="title-description-wrapper">
                        <label className="title">{this.props.title}</label>
                        <hr />
                        <p className="description">{this.props.description}</p>
                    </div>
                </div>
            </div>
        );
    }
}
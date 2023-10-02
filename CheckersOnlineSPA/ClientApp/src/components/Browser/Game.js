import React, { Component } from 'react';
import DefaultUserPicture from '../UserInfo/defaultUserPic.png'
import './Game.css'

export class Game extends React.Component {
    render() {
        return (
            <div className="game">
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
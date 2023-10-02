import React, { Component } from 'react';
import LogoutImg from './logout.png'
import DefaultUserPic from './defaultUserPic.png'
import './UserInfo.css'

export class UserInfo extends Component {
    render() {
        let innerObject = this.props.user != null ? this.renderUserInfo() : this.renderLogIn();
        return (
            <div className="user-info">
                {innerObject}
            </div>
        )
    }

    renderLogIn() {
        return (
            <div className='login-wrapper'>
                <label>You are not logged in.</label>
                <div className='buttons'>
                    <button onClick={this.props.loginHandler}>Sign in</button>
                    <button onClick={this.props.registerHandler}>Sign Up</button>
                </div>
            </div>
        )
    }

    renderUserInfo() {
        let userPicture = null;
        if (this.props.user == undefined )
            userPicture = DefaultUserPic
        else if (this.props.user.picture == null)
            userPicture = DefaultUserPic
        else
            userPicture = this.props.user.picture

        return (
            
            <div>
            <div className="user-picture"><img src={userPicture}></img></div>
            <h1 className="user-name">{this.props.user.nickname}</h1>
            <hr />
            <div className="user-statistic">
                <div className="property">
                    <div className="property-name">Кількість партій</div>
                    <div className="property-value">0</div>
                </div>
                <div className="property">
                    <div className="property-name">Кількість перемог</div>
                    <div className="property-value">0</div>
                </div>
                <div className="property">
                    <div className="property-name">Кількість поразок</div>
                    <div className="property-value">0</div>
                </div>
                <div className="property">
                    <div className="property-name">Кількість нічиїх</div>
                    <div className="property-value">0</div>
                </div>
            </div>
            <hr />
            <div className="exit-wrapper">
                <div className="button" onClick={this.props.logoutHandler}><label>Вийти</label><img src={LogoutImg}></img></div>
            </div>
            </div>
        )
    }
}

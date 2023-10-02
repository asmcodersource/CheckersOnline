import React, { Component } from 'react';
import './Register.css'

export class Register extends Component {
    constructor(props) {
        super(props);
        this.state = {
            dialogHidden: true,
            registerHandler: undefined,
        }
        this.showDialog = this.showDialog.bind(this);
        this.hideDialog = this.hideDialog.bind(this);
        this.submitClickedHandler = this.submitClickedHandler.bind(this);
        this.clickedOnBackground = this.clickedOnBackground.bind(this);
    }

    render() {
        let style = !this.state.dialogHidden ? { display: "flex" } : { display: "none" };
        return (
            <div className="modal-dialog" onClick={this.clickedOnBackground} style={style}>
                <form className='register-dialog'>
                    <h1>Регістрація</h1>
                    <hr />
                    <div className="fields">
                        <label>Нікнейм</label>
                        <input className="username-field" required></input>
                        <label>Електронна пошта</label>
                        <input className="email-field"  type="email" required></input>
                        <label>Пароль</label>
                        <input className="password-field"  type="password" required></input>
                    </div>
                    <p className="disclaimer-text">
                        Важливо! Сервер не несе відповідальності за конфіденційність вашого логіну та паролю. Будь ласка, будьте обережні і не розголошуйте свої особисті дані.
                    </p>
                    <div className="buttons">
                        <button onClick={this.hideDialog}>Назад</button>
                        <button className="register-submit" onClick={this.submitClickedHandler}>Регістрація</button>
                    </div>
                </form>
            </div>
        )
    }

    showDialog() {
        this.setState({ dialogHidden: false });
    }

    hideDialog(e) {
        if (e != undefined)
            e.preventDefault();
        this.setState({ dialogHidden: true });
    }

    clickedOnBackground(e) {
        if (e.target.classList.contains('modal-dialog'))
            this.hideDialog();
    };

    async submitClickedHandler(e) {
        if (e.target.classList.contains('register-submit') === false)
            return;
        e.preventDefault();
        let form = document.querySelector('.register-dialog')
        let email = document.querySelector('.register-dialog .email-field');
        let password = document.querySelector('.register-dialog .password-field');
        let username = document.querySelector('.register-dialog .username-field');
        if (form.checkValidity() === false)
            return;
        const response = await fetch("/registration", {
            method: "POST",
            headers: { "Accept": "application/json", "Content-Type": "application/json" },
            body: JSON.stringify({
                email: email.value,
                password: password.value,
                username: username.value
            })
        });
        if (response.ok === true) {
            const data = await response.json();
            sessionStorage.setItem('token', data.access_token);
            this.props.registerHandler();
            this.hideDialog()
        }
    }
}

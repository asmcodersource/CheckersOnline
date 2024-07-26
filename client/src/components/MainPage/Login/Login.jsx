import React, { Component } from 'react';
import './Login.css'

export class Login extends Component {
    constructor(props) {
        super(props);
        this.state = {
            dialogHidden: true,
        }
        this.showDialog = this.showDialog.bind(this);
        this.hideDialog = this.hideDialog.bind(this);
        this.clickedOnBackground = this.clickedOnBackground.bind(this);
        this.submitClickedHandler = this.submitClickedHandler.bind(this);
    }

    render() {
        let style = !this.state.dialogHidden ? { display: "flex" } : { display: "none" };
        return (
            <div className="modal-dialog" onClick={this.clickedOnBackground} style={style}>
                <form className='login-dialog'>
                    <h1>Авторизація</h1>
                    <hr />
                    <div className="fields">
                        <label>Електронна пошта</label>
                        <input id="email-field" name="email" type="email" required></input>
                        <label>Пароль</label>
                        <input id="password-field" name="password" type="password" required></input>
                    </div>
                    <p className="disclaimer-text">
                        Важливо! Сервер не несе відповідальності за конфіденційність вашого логіну та паролю. Будь ласка, будьте обережні і не розголошуйте свої особисті дані.
                    </p>
                    <div className="buttons">
                        <button onClick={this.hideDialog}>Назад</button>
                        <button onClick={this.submitClickedHandler} className="login-submit">Вхід</button>
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
        if (e.target.classList.contains('modal-dialog') )
            this.hideDialog();
    };

    async submitClickedHandler(e) {
        if (e.target.classList.contains('login-submit') == false)
            return;
        e.preventDefault();
        let form = document.querySelector('.login-dialog')
        let email = document.querySelector('.login-dialog #email-field');
        let password = document.querySelector('.login-dialog #password-field');
        if (form.checkValidity() == false)
            return;
        const response = await fetch("/login", {
            method: "POST",
            headers: { "Accept": "application/json", "Content-Type": "application/json" },
            body: JSON.stringify({
                email: email.value,
                password: password.value
            })
        });
        if (response.ok === true) {
            const data = await response.json();
            sessionStorage.setItem('token', data.access_token);
            this.props.loginHandler();
            this.hideDialog()
        }
    }
}

import React, { Component } from 'react';
import './CreateRoom.css'

export class CreateRoom extends Component {
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
                <form className='create-room-dialog'>
                    <h1>Створити кімнату</h1>
                    <hr />
                    <div className="fields">
                        <label>Назва кімнати</label>
                        <input className="username-field" required></input>
                        <label>Опис кімнати</label>
                        <input className="description-field" type="text" required></input>
                    </div>
                    <p className="disclaimer-text">
                        Важливо! Сервер не несе відповідальності за конфіденційність вашого логіну та паролю. Будь ласка, будьте обережні і не розголошуйте свої особисті дані.
                    </p>
                    <div className="buttons">
                        <button onClick={this.hideDialog}>Назад</button>
                        <button className="form-submit" onClick={this.submitClickedHandler}>Створити</button>
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
        e.preventDefault();
        if (e.target.classList.contains('form-submit') === false)
            return;
        let description = document.querySelector('.create-room-dialog .description-field').value;
        let title = document.querySelector('.create-room-dialog .username-field').value;
        let form = document.querySelector('.create-room-dialog')
        if (form.checkValidity() === false)
            return;
        this.hideDialog();
        this.props.createRoomHandler({title, description });
    }
}

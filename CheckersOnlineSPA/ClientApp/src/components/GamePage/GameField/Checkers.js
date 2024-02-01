import React, { Component, useState } from 'react';
import whiteChecker from './img/white.svg';
import blackChecker from './img/black.svg';
import whiteQueen from './img/white_queen.svg';
import blackQueen from './img/black_queen.svg';
import './Checkers.css';


export class Checker extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        const posX = this.props.position.cellX * 64;
        const posY = this.props.position.cellY * 64;
        let imgSrc = null;
        if (this.props.color == 'White')
            imgSrc = this.props.type == 'Queen' ? whiteQueen : whiteChecker;
        else
            imgSrc = this.props.color == 'Queen' ? blackQueen : blackChecker;
   
        return (
            <div style={{ left: posX, top: posY }} className="checker-wrapper">
                <div className='checker'>
                    <img src={imgSrc}></img>
                </div>
            </div>
        );
    }
}

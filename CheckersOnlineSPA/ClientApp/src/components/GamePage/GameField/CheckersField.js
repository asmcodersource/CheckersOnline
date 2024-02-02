import React, { Component, useState } from 'react';
import './CheckersField.css';
import { Checker } from './Checkers.js';

export class CheckersField extends Component {
    constructor(props) {
        super(props);
        this.state = {
            rows: [],
            checkers: [],
        };
        this.uniqueIndexCounter = 0;

        for (let row = 0; row < 8; row++) {
            let cells = [];
            for (let column = 0; column < 8; column++) {
                const isWhite = (row + column) % 2 === 0;
                cells.push(
                    <CheckerCell key={`${row}-${column}`} isWhite={isWhite} />
                );
            }
            this.state.rows.push(
                <div className='checkers-row'>
                    {cells}
                </div>
            );
        }
    }

    render() {
        return (
            <div className="field-wrapper">
                <div className="checker-field" onClick={(e) => this.mouseClickEvent(e)}>{this.state.rows}</div>
                {this.state.checkers.map((checker, index) => (
                    <Checker key={checker.key} {...checker} />
                ))}
            </div>
        );
    }


    mouseClickEvent(event) {
        const fieldRect = event.currentTarget.getBoundingClientRect();
        const mouseX = event.clientX - fieldRect.left;
        const mouseY = event.clientY - fieldRect.top;
        const cellSize = fieldRect.width / 8;
        const column = Math.floor(mouseX / cellSize);
        const row = Math.floor(mouseY / cellSize);
        this.props.mouseClicked(row, column);
    }

    createChecker(newChecker) {
        this.state.checkers.push({ ...newChecker, key: this.uniqueIndexCounter });
        this.setState({ checkers: this.state.checkers })
        this.uniqueIndexCounter++;
    }

    removeChecker(position) {
        const cellX = position.cellX
        const cellY = position.cellY
        this.setState(prevState => {
            const filteredCheckers = prevState.checkers.filter(checker => !(checker.position.cellX === cellX && checker.position.cellY === cellY));
            return { checkers: filteredCheckers };
        });
    }

    moveChecker(currentPosition, newPosition) {
        this.setState(prevState => {
            const updatedCheckers = prevState.checkers.map(checker => {
                if (checker.position.cellX == currentPosition.cellX && checker.position.cellY == currentPosition.cellY) {
                    return {
                        ...checker,
                        position: newPosition,
                    };
                } else {
                    return checker;
                }
            });
            return {
                checkers: updatedCheckers,
            };
        });
    }
}


class CheckerCell extends Component {
    render() {
        let className = 'checker-cell ';
        className += this.props.isWhite ? 'white-cell' : 'black-cell';
        return (<div className={className}></div>)
    }
}

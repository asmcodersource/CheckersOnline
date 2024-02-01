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
                <div className="checker-field">{this.state.rows}</div>
                {this.state.checkers.map((checker, index) => (
                    <Checker key={checker.key} {...checker} />
                ))}
            </div>
        );
    }

    createChecker(newChecker) {
        this.state.checkers.push({ ...newChecker, key: this.uniqueIndexCounter });
        this.setState({ checkers: this.state.checkers })
        this.uniqueIndexCounter++;
    }

    removeChecker(position) {
        const cellX = position.cellX
        const cellY = position.cellY
        const filteredCheckers = this.state.checkers.filter((checker) => !(checker.position.cellX == cellX && checker.position.cellY == cellY));
        this.setState({ checkers: filteredCheckers })
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

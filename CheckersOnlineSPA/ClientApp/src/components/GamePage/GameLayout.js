import React, { Component } from 'react';
import { CheckersField } from './GameField/CheckersField';
import { RightSideMenu } from './RightSideMenu/RightSideMenu';
import './GameLayout.css';

export class GameLayout extends Component {
    constructor(props) {
        super(props);
        this.checkersFieldRef = React.createRef();
    }


    render() {
        return (
            <div className='game-layout'>
                <div className="game-layout-wrapper">
                    <div className='menu-field-wrapper'>
                        <CheckersField ref={this.checkersFieldRef} />
                        <RightSideMenu />
                    </div>
                </div>
            </div>
        );
    }

    initializeField() {
        for (let y = 0; y < 3; y++) {
            for (let x = 0; x < 8; x++) {
                if ((x + y) % 2 == 0)
                    continue;
                this.checkersFieldRef.current.createChecker({ type: 'Checker', position: { cellX: x, cellY: y }, color:'Black' });
            }
        }

        for (let y = 5; y < 8; y++) {
            for (let x = 0; x < 8; x++) {
                if ((x + y) % 2 == 0)
                    continue;
                this.checkersFieldRef.current.createChecker({ type: 'Checker', position: { cellX: x, cellY: y }, color: 'White' });
            }
        }
    }

    componentDidMount() {
        this.initializeField();
        setTimeout(() => {
            console.log("REMOVE!");
            this.checkersFieldRef.current.removeChecker({ cellX: 1, cellY: 0 })
        }, 5000);
        setTimeout(() => {
            console.log("MOVE!");
            this.checkersFieldRef.current.moveChecker({ cellX: 1, cellY: 2 }, { cellX: 0, cellY: 3 })
        }, 10000);
        

    }
}
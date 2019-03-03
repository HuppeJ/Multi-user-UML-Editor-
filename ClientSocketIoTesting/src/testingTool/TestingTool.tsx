import * as React from 'react';
import * as styles from './TestingTool.scss';
import { IItemBoxProps } from '../itemBox/itemBox';
import { TestTools } from '../testTools/TestTools';
import io from 'socket.io-client';

export interface ITestingToolStateProps {
    favouritedItems?: IItemBoxProps[];
}

export interface ITestingToolProps extends ITestingToolStateProps {}

export class TestingTool extends React.Component<ITestingToolProps> {
    private socket: any;

    constructor(props: any) {
        super(props);
        this.initialiseSocket();
    }

    public initialiseSocket() {
        // CONNECT TO 
        const socketTemp = io.connect('http://localhost:8080');
        // const socketTemp = io.connect('https://projet-3-228722.appspot.com/');
        this.socket = socketTemp;
    }

    render() {
        return (
            <div>
                <div className={styles.container}>
                    <div className={styles.titleContainer}>
                        <div className={styles.titleContent}>
                            PolyPaint Server Testing Tool
                        </div>
                    </div>
                </div>
                <TestTools 
                    socket={this.socket}
                />
            </div>
        );
    }
}

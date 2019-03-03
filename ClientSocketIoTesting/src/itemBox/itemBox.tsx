import * as React from 'react';
import * as styles from './itemBox.scss';

export interface IItemBoxProps {
    eventName: string;
    roomEventToListenTo: string;
    socket: any;
    dataToSend?: any;
}

export interface IItemBoxState {
    response: any;
    roomEventResponse: string;
}

export class ItemBox extends React.Component<IItemBoxProps, IItemBoxState> {

    constructor(props: IItemBoxProps) {
        super(props);
        this.state = { 
            response: "",
            roomEventResponse: "",
        };
    }

    componentDidMount() {
        this.initialiseListener();

    }

    private initialiseListener() {
        this.props.socket.on(`${this.props.eventName}Response`, (data: any) => {
            this.setState({response: data});

        }); 

        this.props.socket.on(`${this.props.roomEventToListenTo}`, (data: any) => {
            this.setState({roomEventResponse: data});

        }); 
    }   

    private onButtonClick(): void {
        if (this.props.dataToSend) {
            this.props.socket.emit(this.props.eventName, this.props.dataToSend)
        } else {
            this.props.socket.emit(this.props.eventName)
        }
    }

    render() {
        return (
            <div className={styles.container}>
                <label className={`${styles.star} ${styles.bold}`} onClick={() => this.onButtonClick()}>
                    Trigger : {this.props.eventName}
                </label>
                <div className={styles.description} > 
                    <div className={styles.bold}>  Response :</div> {this.state.response}
                </div>
                <div className={styles.description}>
                <div className={styles.bold}> Listen to event Room:</div> {this.props.roomEventToListenTo}
                </div>
                <div className={styles.description}>
                <div className={styles.bold}> Response to event Room:</div> {this.state.roomEventResponse}
                </div>

            </div>
        );
    }
}

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
            // this.setState({ response: data });
            console.log(`${this.props.eventName}Response`);
            const temp = JSON.parse(data);
            console.log(temp);
        });

        this.props.socket.on(`${this.props.roomEventToListenTo}`, (data: any) => {
            const temp = JSON.parse(data);

            // this.setState({ roomEventResponse: JSON.stringify(temp) });
            console.log(`${this.props.roomEventToListenTo}`);
            console.log(temp);
        });
    }

    private onButtonClick(): void {
        if (this.props.dataToSend) {
            this.props.socket.emit(this.props.eventName, this.props.dataToSend)
        } else {
            this.props.socket.emit(this.props.eventName)
        }
    }
    private getWithData(): string {
        return `with data : ${this.props.dataToSend}`
    }

    private getListentoeventRoom(): any {
        return (<div className={styles.description}>
            <div className={styles.bold}> Listen to event Room:</div> {this.props.roomEventToListenTo}
        </div>);
    }

    private getResponsetoeventRoom(): any {
        return (<div className={styles.description}>
            <div className={styles.bold}> Response to event Room:</div> {this.state.roomEventResponse}
        </div>);
    }



    render() {
        return (
            <div className={styles.container}>
                <label className={`${styles.star} ${styles.bold}`} onClick={() => this.onButtonClick()}>
                    Trigger : {this.props.eventName}
                    <div>
                     {this.props.dataToSend ? this.getWithData() : ""}
                    </div>
                </label>
                <div className={styles.description} >
                    <div className={styles.bold}>  Response :</div> {this.state.response}
                </div>

                {this.props.roomEventToListenTo ? this.getListentoeventRoom() : ""}

                {this.state.roomEventResponse ? this.getResponsetoeventRoom() : ""}

            </div>
        );
    }
}

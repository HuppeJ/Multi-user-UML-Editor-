import * as React from 'react';
import * as styles from './TestTools.scss';
import { IItemBoxProps } from '../itemBox/itemBox';
import { ListBox } from '../listBox/listBox';

export interface ITestToolsState {
    items?: IItemBoxProps[];
}

export interface ITestToolsProps {
    socket: any;
}


export class TestTools extends React.Component<ITestToolsProps, ITestToolsState> {
    public static readonly ID = "TestTools_LISTBOX_ID";

    constructor(props: ITestToolsProps) {
        super(props);
        this.state = { items: [] };
        this.initialiseTestTools();
    }

    public initialiseTestTools() {
        this.addTestTool("joinCanvasTest", "", this.props.socket);
        this.addTestTool("canvasUpdateTest", "canvasUpdateTestResponse", this.props.socket, "dataToSendTEST");
    }
    
    public addTestTool(eventName: string, roomEventToListenTo: string, socket: any, dataToSend?: any) {
        this.state.items.push( {
            eventName: eventName,
            roomEventToListenTo: roomEventToListenTo,
            socket: socket,
            dataToSend: dataToSend,
        } as IItemBoxProps);
    }

    render() {
        return (
            <div className={styles.container}>
                <div className={styles.title}>
                Test Tools
                </div>
                <ListBox
                    id={TestTools.ID}
                    items={this.state.items}
                />
            </div>
        );
    }
}

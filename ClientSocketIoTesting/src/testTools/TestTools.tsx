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

    // TESTTOOLS TO ADD
    public initialiseTestTools() {
        this.addTestTool("getServerState", "", this.props.socket);

        this.addTestTool("createCanvasRoom", "canvasRoomCreated", this.props.socket, this.getMockCanvas("CanvasRoom1"));
        this.addTestTool("createCanvasRoom", "canvasRoomCreated", this.props.socket, this.getMockCanvas("CanvasRoom2"));

        this.addTestTool("removeCanvasRoom", "canvasRoomRemoved", this.props.socket, "CanvasRoom1");
        this.addTestTool("removeCanvasRoom", "canvasRoomRemoved", this.props.socket, "CanvasRoom2");



        this.addTestTool("joinCanvasRoom", "", this.props.socket, "CanvasRoom1");
        this.addTestTool("joinCanvasRoom", "", this.props.socket, "CanvasRoom2");


        this.addTestTool("leaveCanvasRoom", "", this.props.socket, "CanvasRoom1");


 




        this.addTestTool("joinCanvasTest", "", this.props.socket);
        this.addTestTool("canvasUpdateTest", "canvasUpdateTestResponse", this.props.socket, "dataToSendTEST");
    }
    
    private getMockCanvas(name: string): string {
        const newCanvas: any = {
            id: "tempid",  
            name: name,  
            author: "string",  
            owner: "string",  
            accessibility: 1,
            shapes: [],  
            links: []
        };
        return JSON.stringify(newCanvas);
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

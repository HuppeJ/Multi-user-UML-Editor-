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

        this.addTestTool("createCanvas", "canvasRoomCreated", this.props.socket, this.getMockCanvas("CanvasRoom1"));
        this.addTestTool("createCanvas", "canvasRoomCreated", this.props.socket, this.getMockCanvas("CanvasRoom2"));

        this.addTestTool("removeCanvas", "canvasRoomRemoved", this.props.socket, "CanvasRoom1");
        this.addTestTool("removeCanvas", "canvasRoomRemoved", this.props.socket, "CanvasRoom2");



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
            shapes: {
                id: "shape1",  
                type: 1,  // {Enum_ShapeTypes}
                name: "string",  
                shapeStyle: { 
                    coordinates: { 
                        x: 0,  
                        y: 0
                    },  
                    width: 0,  
                    height: 0,  
                    rotation: 0,  
                    borderColor: "string",  
                    borderStyle: 0, // {Enum_BorderTypes}, 
                    backgroundColor: "string"
                },  
                linksTo: [{ 
                    id: "string",  
                    from: {
                        formId: "string", 
                        anchor: "number" // enum AnchorPoints
                    },  
                    to: {
                        formId: "string", 
                        anchor: "number" // enum AnchorPoints
                    },  
                    type: 0,  
                    style: { 
                        color: "string",  
                        thickness: 0,  
                        type: 0
                    },  
                    path: [{ 
                        x: 0,  
                        y: 0
                    },{ 
                        x: 0,  
                        y: 0
                    }]
                }
                ],
                linksFrom: [{ 
                    id: "string",  
                    from: {
                        formId: "string", 
                        anchor: "number" // enum AnchorPoints
                    },  
                    to: {
                        formId: "string", 
                        anchor: "number" // enum AnchorPoints
                    },  
                    type: 0,  
                    style: { 
                        color: "string",  
                        thickness: 0,  
                        type: 0
                    },  
                    path: [{ 
                        x: 0,  
                        y: 0
                    },{ 
                        x: 0,  
                        y: 0
                    }]
                }
                ],
            },  
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

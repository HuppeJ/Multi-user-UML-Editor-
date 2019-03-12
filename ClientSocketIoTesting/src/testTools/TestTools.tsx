import * as React from 'react';
import * as styles from './TestTools.scss';
import { IItemBoxProps } from '../itemBox/itemBox';
import { ListBox } from '../listBox/listBox';

const uuidv1 = require('uuid/v1');


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

        // this.addTestTool("createCanvas", "canvasRoomCreated", this.props.socket, this.getMockCanvas_IEditCanevasData("CanvasRoom1"));
        // this.addTestTool("createCanvas", "canvasRoomCreated", this.props.socket, this.getMockCanvas_IEditCanevasData("CanvasRoom2"));

        // this.addTestTool("removeCanvas", "canvasRoomRemoved", this.props.socket, this.getMockCanvas_IEditGalleryData("CanvasRoom1"));
        // this.addTestTool("removeCanvas", "canvasRoomRemoved", this.props.socket, this.getMockCanvas_IEditGalleryData("CanvasRoom2"));



        // this.addTestTool("joinCanvasRoom", "", this.props.socket, this.getMockCanvas_IEditGalleryData("CanvasRoom1"));
        // this.addTestTool("joinCanvasRoom", "", this.props.socket, this.getMockCanvas_IEditGalleryData("CanvasRoom2"));
        // this.addTestTool("joinCanvasRoom", "", this.props.socket, this.getMockCanvas_IEditGalleryData("CanvasRoom3"));


        // this.addTestTool("leaveCanvasRoom", "", this.props.socket, this.getMockCanvas_IEditGalleryData("CanvasRoom1"));
        // this.addTestTool("leaveCanvasRoom", "", this.props.socket, this.getMockCanvas_IEditGalleryData("CanvasRoom2"));
        // this.addTestTool("leaveCanvasRoom", "", this.props.socket, this.getMockCanvas_IEditGalleryData("CanvasRoom3"));


 
        this.addTestTool("getAllCanvas", "", this.props.socket);
        this.addTestTool("getPublicCanvas", "", this.props.socket);
        // this.addTestTool("getPrivateCanvas", "", this.props.socket, "string");
        // this.addTestTool("getCanvas", "",  this.props.socket, this.getMockCanvas_IEditGalleryData("CanvasRoom1"));

        this.addTestTool("joinCanvasRoom", "", this.props.socket, this.getMockCanvas_IEditGalleryData("qwe"));
        this.addTestTool("reinitializeCanvas", "canvasReinitialized",  this.props.socket, this.getMockCanvas_IEditCanevasData("qwe"));
        this.addTestTool("createForm", "formCreated",  this.props.socket, this.getMockCanvas_IEditCanevasData("qwe"));



        this.addTestTool("joinCanvasTest", "", this.props.socket);
        this.addTestTool("canvasUpdateTest", "canvasUpdateTestResponse", this.props.socket, "dataToSendTEST");
    }
    
    private getMockCanvas_IEditCanevasData(name: string): string {
          const temp: any =   {
            username: "martin",
            canevas: {
                id: uuidv1(),  
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
            }
        }
    

        return JSON.stringify(temp);
    }
    private getMockCanvas_IEditGalleryData(name: string): string {
        const temp: any =   {
          username: "martin",
          canevasName: name,
          password: null            
      }
  

      return JSON.stringify(temp);
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

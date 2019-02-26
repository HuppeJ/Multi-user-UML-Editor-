export interface ICanvasRoom {

}


export default class CanvasRoom {
    private canvasRoomName: string;
    private connectedUsers: any;  // connectedUsers is a Map : [key: socketId, value: username]

    constructor(canvasRoomName: string, CanvasProperties: any) {
        this.canvasRoomName = canvasRoomName;
        this.connectedUsers = new Map();
    }

}

import CanvasRoom from "./CanvasRoom";
import { ICanevas } from "../interfaces/interfaces";
import { CANVAS_ROOM_ID } from "../../../constants/RoomID";

export default class CanvasManager {  
    private canvasRooms: any; // [key: canvasRoomId, value: CanvasRoom]

    constructor() {
        this.canvasRooms = new Map(); 
    }

    public getCanvasRoomIdFromName(canvasName: string): string {
        return `${CANVAS_ROOM_ID}-${canvasName}`;
    }
    
    public addCanvasRoom(newCanvas: ICanevas, socketId: any) {
        const canvasRoomId: string = this.getCanvasRoomIdFromName(newCanvas.name);

        if (this.doesCanvasRoomExist(canvasRoomId)) {
            return false;
        }

        const canvasRoom = new CanvasRoom(newCanvas);
        canvasRoom.addUser(socketId);
        this.canvasRooms.set(canvasRoomId, canvasRoom);
        return true;
    }

    public doesCanvasRoomExist(canvasRoomId: string) {
        return this.canvasRooms.has(canvasRoomId);
    }

    public addUserToCanvasRoom(canvasRoomId: string, socketId: any) {
        if (this.doesCanvasRoomExist(canvasRoomId)){
            const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
            if (!canvasRoom.hasUser(socketId)) {
                canvasRoom.addUser(socketId);
                return true;
            }
        }
        return false;
    }


    public removeCanvas(canvasRoomId: string) {
        if (this.doesCanvasRoomExist(canvasRoomId)) {
            this.canvasRooms.delete(canvasRoomId);
            // TODO : à compléter
        }
    }

    // public isClientInCanvas(canvasName: string, socketId: any) {
    //     if (this.isCanvas(canvasName)){
    //         let Canvas = this.canvasRooms.get(canvasName);
    //         return Canvas.hasUser(socketId);
    //     }
    //     return false;
    // }

    // public removeClientFromCanvas(canvasName: string, socketId: any) {
    //     if(this.isClientInCanvas(canvasName, socketId)) {
    //         let Canvas = this.canvasRooms.get(canvasName);
    //         Canvas.removeUser(socketId);
    //         return true;
    //     }
    //     return false;
    // }

    public getCanvasRooms() {
        return JSON.stringify(Array.from(this.canvasRooms.keys()));
    }

    // public getCanvasClients(canvasName: string) {
    //     return JSON.stringify(this.canvasRooms.get(canvasName));
    // }
}

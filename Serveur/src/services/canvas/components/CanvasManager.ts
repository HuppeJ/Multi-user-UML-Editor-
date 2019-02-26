import CanvasRoom from "./CanvasRoom";
import { Canvas } from "./Canvas";

export default class CanvasManager {  
    private canvasRooms: any; // [key: CanvasRoomId, value: CanvasRoom]

    constructor() {
        this.canvasRooms = new Map(); 
    }

    
    // public addCanvas(canvasName: string, socketId: any) {
    //     if (this.doesCanvasExist(canvasName)) {
    //         return false;
    //     }

    //     const canvas = new Canvas();
    //     canvas.addUserToCanvas(canvasName, socketId);
    //     this.canvasRooms.set(canvasName, canvas);
    //     return true;
    // }

    // public doesCanvasExist(canvasName: string) {
    //     return this.canvasRooms.has(canvasName);
    // }

    // public addUserToCanvas(canvasName: string, socketId: any) {
    //     if (this.doesCanvasExist(canvasName)){
    //         const Canvas = this.canvasRooms.get(canvasName);
    //         if (!Canvas.hasUser(socketId)) {
    //             Canvas.addUser(socketId);
    //             return true;
    //         }
    //     }
    //     return false;
    // }


    // public removeCanvas(canvasName: string) {
    //     if (this.isCanvas(canvasName)) {
    //         this.canvasRooms.delete(canvasName);
    //     }
    // }

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

    // public getCanvass() {
    //     let strKeys = JSON.stringify(Array.from(this.canvasRooms.keys()));
    //     return strKeys;
    // }

    // public getCanvasClients(canvasName: string) {
    //     let strKeys = JSON.stringify(this.canvasRooms.get(canvasName));
    //     return strKeys;
    // }
}

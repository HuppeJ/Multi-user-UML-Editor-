import Drawing from "./Drawing";

export default class DrawingManager {  
    // private drawings: any = null;

    // constructor() {
    //     this.drawings = new Map(); // [key: drawing.name, value: Drawing]
    // }

    // public addDrawing(drawingName: string, socketId: any) {
    //     if (!this.isDrawing(drawingName)) {
    //         const drawing = new Drawing(drawingName);
    //         drawing.addUser(socketId);
    //         this.drawings.set(drawingName, drawing);
    //         return true;
    //     }
    //     return false;
    // }

    // public isDrawing(drawingName: string) {
    //     return this.drawings.has(drawingName);
    // }

    // public removeDrawing(drawingName: string) {
    //     if (this.isDrawing(drawingName)) {
    //         this.drawings.delete(drawingName);
    //     }
    // }

    // public addUserToDrawing(drawingName: string, socketId: any) {
    //     if (this.isDrawing(drawingName)){
    //         const drawing = this.drawings.get(drawingName);
    //         if (!drawing.hasUser(socketId)) {
    //             drawing.addUser(socketId);
    //             return true;
    //         }
    //     }
    //     return false;
    // }

    // public isClientInDrawing(drawingName: string, socketId: any) {
    //     if (this.isDrawing(drawingName)){
    //         let drawing = this.drawings.get(drawingName);
    //         return drawing.hasUser(socketId);
    //     }
    //     return false;
    // }

    // public removeClientFromDrawing(drawingName: string, socketId: any) {
    //     if(this.isClientInDrawing(drawingName, socketId)) {
    //         let drawing = this.drawings.get(drawingName);
    //         drawing.removeUser(socketId);
    //         return true;
    //     }
    //     return false;
    // }

    // public getDrawings() {
    //     let strKeys = JSON.stringify(Array.from(this.drawings.keys()));
    //     return strKeys;
    // }

    // public getDrawingClients(drawingName: string) {
    //     let strKeys = JSON.stringify(this.drawings.get(drawingName));
    //     return strKeys;
    // }
}

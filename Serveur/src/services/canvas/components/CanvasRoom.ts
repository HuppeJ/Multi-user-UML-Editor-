import { Canvas, ICanvas } from "./Canvas";

export default class CanvasRoom {
    private CanvasRoomId: string;
    private connectedUsers: any;  // connectedUsers is a Map : [key: socketId, value: username]
    private Canvas: Canvas;

    constructor(CanvasRoomId: string, CanvasProperties: ICanvas) {
        this.CanvasRoomId = CanvasRoomId;
        this.connectedUsers = new Map();
        this.Canvas = new Canvas(CanvasProperties);
    }

}

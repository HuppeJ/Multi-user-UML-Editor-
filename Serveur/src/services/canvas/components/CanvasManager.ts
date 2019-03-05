import CanvasRoom from "./CanvasRoom";
import { ICanevas, ICreateFormData } from "../interfaces/interfaces";
import { CANVAS_ROOM_ID } from "../../../constants/RoomID";
import { mapToObj } from "../../../utils/mapToObj";

export default class CanvasManager {
    private canvasRooms: Map<string, CanvasRoom>; // [key: canvasRoomId, value: canvasRoom]

    constructor() {
        this.canvasRooms = new Map<string, CanvasRoom>();
    }

    public getCanvasRoomIdFromName(canvasName: string): string {
        return `${CANVAS_ROOM_ID}-${canvasName}`;
    }

    public addCanvasRoom(newCanvas: ICanevas, socketId: any) {
        const canvasRoomId: string = this.getCanvasRoomIdFromName(newCanvas.name);

        if (this.canvasRooms.has(canvasRoomId)) {
            return false;
        }

        const canvasRoom = new CanvasRoom(newCanvas);
        canvasRoom.addUser(socketId);
        this.canvasRooms.set(canvasRoomId, canvasRoom);
        return true;
    }

    public addUserToCanvasRoom(canvasRoomId: string, socketId: any) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (canvasRoom && !canvasRoom.hasUser(socketId)) {
            canvasRoom.addUser(socketId);
            return true;
        }

        return false;
    }

    public removeUserFromCanvasRoom(canvasRoomId: string, socketId: any) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (canvasRoom && canvasRoom.hasUser(socketId)) {
            canvasRoom.removeUser(socketId);
            return true;
        }

        return false;
    }

    public removeCanvasRoom(canvasRoomId: string) {
        if (this.canvasRooms.has(canvasRoomId)) {
            this.canvasRooms.delete(canvasRoomId);
            return true;
        }

        return false;
    }

    public isClientInCanvas(canvasRoomId: string, socketId: any) {
        if (this.canvasRooms.has(canvasRoomId)) {
            const canvasRoom = this.canvasRooms.get(canvasRoomId);
            return canvasRoom.hasUser(socketId);
        }

        return false;
    }

    // TODO : Fonction pas testée... 
    public getCanvasRoomFromSocketId(socketId: any): string {
        for (const [canvasRoomId, canvasRoom] of this.canvasRooms.entries()) {
            if (canvasRoom.hasUser(socketId)) {
                return canvasRoomId;
            }
        }

        return null;
    }

    public addFormToCanvas(canvasRoomId: string, form: any, socketId: any) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        canvasRoom.addForm(form);
        return true;
    }

    // TODO : remove (version lorsqu'on ne reçoit pas le nom du canvas)
    // public addFormToCanvas(form: any, socketId: any) {
    //     const canvasRoomId: string = this.getCanvasRoomFromSocketId(socketId);
    //     if (!canvasRoomId) {
    //         return false;
    //     }
        
    //     const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
    //     if (!canvasRoom) {
    //         return false;
    //     }

    //     canvasRoom.addForm(form);
    //     return true;
    // }

    public getCanvasRoomsSERI(): string {
        return JSON.stringify({
            canvasRooms: mapToObj(this.canvasRooms)
        });
    }

    public getUsersInCanvasRoomSERI(canvasName: string) {
        return JSON.stringify({
            connectedUsers: JSON.parse(this.canvasRooms.get(canvasName).getConnectedUsersSERI())
        });
    }

    toJSON() {
        return Object.assign({}, this, {
            canvasRooms: mapToObj(this.canvasRooms)
        });
    }
}

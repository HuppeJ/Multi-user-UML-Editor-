import CanvasRoom from "./CanvasRoom";
import { ICanevas, IEditFormsData, IEditLinkData, IEditLinksData, IEditFormData, IEditCanevasData } from "../interfaces/interfaces";
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

    public addCanvasRoom(data: IEditCanevasData) {
        const canvasRoomId: string = this.getCanvasRoomIdFromName(data.canevas.name);

        if (this.canvasRooms.has(canvasRoomId)) {
            return false;
        }

        const canvasRoom = new CanvasRoom(data.canevas);
        canvasRoom.addUser(data.username);
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

    /***********************************************
    * Functions related to Forms
    ************************************************/
    public addFormToCanvas(canvasRoomId: string, data: IEditFormData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.addForm(data);
    }

    public updateCanvasForms(canvasRoomId: string, data: IEditFormsData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.updateForms(data);
    }

    public deleteCanvasForms(canvasRoomId: string, data: IEditFormsData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.deleteForms(data);
    }

    public selectCanvasForms(canvasRoomId: string, data: IEditFormsData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.selectForms(data);
    }

    public deselectCanvasForms(canvasRoomId: string, data: IEditFormsData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.deselectForms(data);
    }


    /***********************************************
    * Functions related to Links
    ************************************************/
    public addLinkToCanvas(canvasRoomId: string, data: IEditLinkData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.addLink(data);
    }

    public updateCanvasLinks(canvasRoomId: string, data: IEditLinksData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.updateLinks(data);
    }

    public deleteCanvasLinks(canvasRoomId: string, data: IEditLinksData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.deleteLinks(data);
    }



    /***********************************************
    * Functions related to the Canvas
    ************************************************/
    public resizeCanvas(canvasRoomId: string, data: IEditCanevasData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.resize(data);
    }

    public reinitializeCanvas(canvasRoomId: string) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.reinitialize();
    }


    /***********************************************
    * Serialize / Deserialize
    ************************************************/
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

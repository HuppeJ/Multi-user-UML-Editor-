import CanvasRoom from "./CanvasRoom";
import { IEditLinksData, IEditCanevasData, IEditGalleryData, IUpdateFormsData, IUpdateLinksData } from "../interfaces/interfaces";
import { CANVAS_ROOM_ID } from "../../../constants/RoomID";
import { mapToObj } from "../../../utils/mapToObj";

export default class CanvasManager {
    private canvasRooms: Map<string, CanvasRoom>; // [key: canvasRoomId, value: canvasRoom]

    constructor() {
        this.canvasRooms = new Map<string, CanvasRoom>();
    }

    public getCanvasRoomIdFromName(canvasName: string): string {
        return `${CANVAS_ROOM_ID}_${canvasName}`;
    }

    public addCanvasRoom(canvasRoomId: string, data: IEditCanevasData) {
        if (this.canvasRooms.has(canvasRoomId)) {
            return false;
        }

        const canvasRoom = new CanvasRoom(data.canevas);
        this.canvasRooms.set(canvasRoomId, canvasRoom);
        return true;
    }

    public removeCanvasRoom(canvasRoomId: string, data: IEditGalleryData) {
        if (this.canvasRooms.has(canvasRoomId)) {
            this.canvasRooms.delete(canvasRoomId);
            return true;
        }

        return false;
    }

    public addUserToCanvasRoom(canvasRoomId: string, data: IEditGalleryData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (canvasRoom && !canvasRoom.hasUser(data.username)) {
            canvasRoom.addUser(data.username);
            return true;
        }

        return false;
    }

    public removeUserFromCanvasRoom(canvasRoomId: string, data: IEditGalleryData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (canvasRoom && canvasRoom.hasUser(data.username)) {
            canvasRoom.removeUser(data.username);
            return true;
        }

        return false;
    }

    // TODO : Non utilisée et Fonction pas testée... 
    public isUserInCanvas(canvasRoomId: string, unsername: string) {
        if (this.canvasRooms.has(canvasRoomId)) {
            const canvasRoom = this.canvasRooms.get(canvasRoomId);
            return canvasRoom.hasUser(unsername);
        }

        return false;
    }

    // TODO : Non utilisée et Fonction pas testée... 
    public getCanvasRoomFromSocketId(unsername: any): string {
        for (const [canvasRoomId, canvasRoom] of this.canvasRooms.entries()) {
            if (canvasRoom.hasUser(unsername)) {
                return canvasRoomId;
            }
        }

        return null;
    }

    /***********************************************
    * Functions related to Forms
    ************************************************/
    public addFormToCanvas(canvasRoomId: string, data: IUpdateFormsData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.addForm(data);
    }

    public updateCanvasForms(canvasRoomId: string, data: IUpdateFormsData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.updateForms(data);
    }

    public deleteCanvasForms(canvasRoomId: string, data: IUpdateFormsData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.deleteForms(data);
    }

    public selectCanvasForms(canvasRoomId: string, data: IUpdateFormsData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.selectForms(data);
    }

    public deselectCanvasForms(canvasRoomId: string, data: IUpdateFormsData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.deselectForms(data);
    }


    /***********************************************
    * Functions related to Links
    ************************************************/
    public addLinkToCanvas(canvasRoomId: string, data: IUpdateLinksData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.addLink(data);
    }

    public updateCanvasLinks(canvasRoomId: string, data: IUpdateLinksData) {
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

    public selectCanvasLinks(canvasRoomId: string, data: IEditLinksData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.selectLinks(data);
    }

    public deselectCanvasLinks(canvasRoomId: string, data: IEditLinksData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.deselectLinks(data);
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

    public reinitializeCanvas(canvasRoomId: string, data: IEditCanevasData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.reinitialize(data);
    }

    public selectCanvas(canvasRoomId: string, data: IEditGalleryData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.selectCanvas(data);
    }

    public deselectCanvas(canvasRoomId: string, data: IEditGalleryData) {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return false;
        }

        return canvasRoom.deselectCanvas(data);
    }


    /***********************************************
    * Serialize / Deserialize
    ************************************************/
    public getSelectedFormsInCanvasRoomSERI(canvasRoomId: string): string {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        if (!canvasRoom) {
            return null;
        }

        return canvasRoom.getSelectedFormsSERI();
    }

    public getCanvasRoomSERI(canvasRoomId: string): string {
        const canvasRoom: CanvasRoom = this.canvasRooms.get(canvasRoomId);
        return JSON.stringify(canvasRoom);
    }

    public getCanvasRoomsSERI(): string {
        return JSON.stringify({
            canvasRooms: mapToObj(this.canvasRooms)
        });
    }

    public getPublicCanvasSERI(): string {
        const publicCanvasArray = [];

        for (const [canvasRoomId, canvasRoom] of this.canvasRooms.entries()) {
            if (canvasRoom.isCanvasPublic()) {
                publicCanvasArray.push(canvasRoom.canvas);
            }
        }

        return JSON.stringify({
            publicCanvas: publicCanvasArray
        });
    }

    public getPrivateCanvasSERI(username: string): string {
        const privateCanvasArray = [];

        for (const [canvasRoomId, canvasRoom] of this.canvasRooms.entries()) {
            if (canvasRoom.isCanvasPrivate() && canvasRoom.isAuthorOfCanvase(username)) {
                privateCanvasArray.push(canvasRoom.canvas);
            }
        }

        return JSON.stringify({
            privateCanvas: privateCanvasArray
        });
    }

    toJSON() {
        return Object.assign({}, this, {
            canvasRooms: mapToObj(this.canvasRooms)
        });
    }
}

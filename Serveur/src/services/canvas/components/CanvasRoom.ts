import { ICanevas } from "../interfaces/interfaces";

export interface ICanvasRoom {

}


export default class CanvasRoom {
    public connectedUsers: any;  // connectedUsers is a Map : [key: socketId, value: username]

    constructor(public canvas: ICanevas) {
        this.connectedUsers = new Set();
    }

    public addUser(socketId: any) {
        this.connectedUsers.add(socketId);
    }

    public removeUser(socketId: any) {
        this.connectedUsers.delete(socketId);
    }

    public hasUser(socketId: any) {
        return this.connectedUsers.has(socketId);
    }
}

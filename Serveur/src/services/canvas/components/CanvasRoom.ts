import { ICanevas } from "../interfaces/interfaces";
import { mapToObj } from "../../../utils/mapToObj";

export interface ICanvasRoom {

}


export default class CanvasRoom {
    public connectedUsers: any;  // connectedUsers is a Set : [key: socketId]

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

    public getConnectedUsersSERI(): string {
        return JSON.stringify({
            connectedUsers: Array.from(this.connectedUsers)
        });
    }

    public addForm(form: any) {
        this.canvas.shapes.push(form);
    }

    // toJSON is automatically used by JSON.stringify
    toJSON() {
        return Object.assign({}, this, {
            // convert fields that need converting
            connectedUsers: Array.from(this.connectedUsers)
        });
    }

    // // fromJSON is used to convert an serialized version
    // // of the User to an instance of the class
    // static fromJSON(json: ICanvasRoom | string): User {
    //     if (typeof json === 'string') {
    //         // if it's a string, parse it first
    //         return JSON.parse(json, User.reviver);
    //     } else {
    //         // create an instance of the User class
    //         let user = Object.create(User.prototype);
    //         // copy all the fields from the json object
    //         return Object.assign(user, json, {
    //             // convert fields that need converting
    //             created: new Date(json.created),
    //             account: Account.fromJSON(json.account)
    //         });
    //     }
    // }
}

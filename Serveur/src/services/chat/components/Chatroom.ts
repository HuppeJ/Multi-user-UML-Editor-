import { IEditChatroomData, IMessageData } from "../../canvas/interfaces/interfaces";

export default class Chatroom {
    private chatroomName: string = "";
    private author: string = "";
    private connectedUsers: any = null; // [key: username]

    constructor(data: IEditChatroomData) {
        this.chatroomName = data.chatroomName;
        this.author = data.username;
        this.connectedUsers = new Set();
    }

    public sendMessage(data: IMessageData) {
        return this.connectedUsers.has(data.username);
    }

    public addUser(username: string) {
        if (!this.connectedUsers.has(username)) {
            this.connectedUsers.add(username);
        }
    }

    public removeUser(username: string) {
        if (this.connectedUsers.has(username)) {
            this.connectedUsers.delete(username);
        }
    }

    public hasUser(username: string) {
        return this.connectedUsers.has(username);
    }

    public getConnectedUsersSERI(): string {
        return JSON.stringify({
            connectedUsers: Array.from(this.connectedUsers)
        });
    }
}

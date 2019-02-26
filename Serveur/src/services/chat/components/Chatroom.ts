export default class Chatroom {
    private chatroomName: string = "";
    private users: any = null; // [key: socketId]

    constructor(name: string) {
        this.chatroomName = name;
        this.users = new Set();
    }

    public addUser(socketId: any) {
        this.users.add(socketId);
    }

    public removeUser(socketId: any) {
        this.users.delete(socketId);
    }

    public hasUser(socketId: any) {
        return this.users.has(socketId);
    }
}

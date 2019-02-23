// Based on https://github.com/justadudewhohacks/websocket-chat
// export class Chatroom {
//     private chatroomName: string = "";
//     private users = null; // [key: socketId]

//     constructor(name) {
//         this.chatroomName = name;
//         this.users = new Set();
//     }

//     public addUser(socketId) {
//         this.users.add(socketId);
//     }

//     public removeUser(socketId) {
//         this.users.delete(socketId);
//     }

//     public hasUser(socketId) {
//         return this.users.has(socketId);
//     }
// }

module.exports = function (name) {
    const chatroomName = name;
    const users = new Set(); // [key: socketId]

    function addUser(socketId) {
        users.add(socketId);
    }

    function removeUser(socketId) {
        users.delete(socketId);
    }

    function hasUser(socketId) {
        return users.has(socketId);
    }

    return {
        addUser,
        removeUser,
        hasUser
    }
}

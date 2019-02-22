// Based on https://github.com/justadudewhohacks/websocket-chat
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
        addEntry,
        getChatHistory,
        addUser,
        removeUser,
        hasUser
    }
}

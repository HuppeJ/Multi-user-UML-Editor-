// Based on https://github.com/justadudewhohacks/websocket-chat

module.exports = function () {
    let clients = new Set();
    let chatHistory = [];

    function addEntry(entry) {
        chatHistory = chatHistory.concat(entry);
    }

    function getChatHistory() {
        return chatHistory.slice();
    }

    function addUser(socketId) {
        clients.add(socketId);
    }

    function removeUser(socketId) {
        clients.delete(socketId);
    }

    function hasUser(socketId) {
        return clients.has(socketId);
    }

    /*
    function serialize() {
        return {
            name,
            image,
            numClients: clients.size
        }
    }
    */

    return {
        addEntry,
        getChatHistory,
        addUser,
        removeUser,
        //serialize,
        hasUser
    }
}

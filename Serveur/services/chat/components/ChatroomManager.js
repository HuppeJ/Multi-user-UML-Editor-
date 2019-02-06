// Based on https://github.com/justadudewhohacks/websocket-chat

module.exports = function () {
    // mapping of all available chatrooms 
    // chatrooms est une Map : [key: chatroom.name, value: Chatroom]
    const chatrooms = new Map();

    // TODO : Arranger le type chatroom pour avoir l'historique des messages et les sockets id

    function addChatroom(chatroomName, socketId) {
        if(!isChatroom(chatroomName)) {
            let clients = [];
            clients.concat(socketId);
            chatrooms.set(chatroomName, clients);
            return true;
        }
        return false;
    }

    function isChatroom(chatroomName) {
        return chatrooms.has(chatroomName);
    }

    function removeChatroom(chatroomName) {
        if(isChatroom(chatroomName)) {
            chatrooms.delete(chatroomName);
        }
    }

    function addClientToChatroom(chatroomName, socketId) {
        if (isChatroom(chatroomName)){
            let clients = chatrooms.get(chatroomName);
            if (clients.includes(socketId)) {
                return false;
            }
            clients.concat(socketId);
            chatrooms.set(chatroomName, clients);
            return true;
        }
        return false;
    }

    function isClientInChatroom(chatroomName, socketId) {
        if (isChatroom(chatroomName)){
            let clients = chatrooms.get(chatroomName);
            if (clients.includes(socketId)) {
                return true;
            }
        }
        return false;
    }

    function addClientToChatroom(chatroomName, socketId) {
        if (isChatroom(chatroomName)){
            let clients = chatrooms.get(chatroomName);
            if (isClientInChatroom(chatroomName, socketId)) {
                clients.concat(socketId);
                chatrooms.set(chatroomName, clients);
                return true;
            }
        }
        return false;
    }

    function removeClientFromChatroom(chatroomName, socketId) {
        if(isClientInChatroom(chatroomName, socketId)) {
            let clients = chatrooms.get(chatroomName);
            let lastSocketId = clients.pop();
            if(lastSocketId === socketId) {
                return true;
            }
            clients.forEach(function(element) {
                if(element === socketId) {
                    element = lastSocketId;
                    return true;
                }
            });
        }
        return false;
    }

    function getChatrooms() {
        let strKeys = JSON.stringify(Array.from(chatrooms.keys()));
        return strKeys;
    }

    return {
        addChatroom,
        isChatroom,
        removeChatroom,
        addClientToChatroom,
        removeClientFromChatroom,
        getChatrooms
    }
}

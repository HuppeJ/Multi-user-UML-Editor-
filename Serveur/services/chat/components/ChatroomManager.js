// Based on https://github.com/justadudewhohacks/websocket-chat

const Chatroom = require('./Chatroom')
const chatroomTemplates = require('../config/chatrooms')

module.exports = function () {
    // mapping of all available chatrooms 
    // chatrooms est une Map : [key: chatroom.name, value: Chatroom]
    const chatrooms = new Map(
        chatroomTemplates.map(chatroom => [
            chatroom.name,
            Chatroom(chatroom)
        ])
    )

    function removeClient(client) {
        chatrooms.forEach(chatroom => chatroom.removeUser(client));
    }

    function getChatroomByName(chatroomName) {
        return chatrooms.get(chatroomName);
    }

    function serializeChatrooms() {
        return Array.from(chatrooms.values()).map(chatroom => chatroom.serialize());
    }

    return {
        removeClient,
        getChatroomByName,
        serializeChatrooms
    }
}

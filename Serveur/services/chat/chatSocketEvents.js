// Based on https://github.com/justadudewhohacks/websocket-chat
const SocketEvents = require('../../SocketEvents');


const ClientManager = require('./components/ClientManager');
const ChatroomManager = require('./components/ChatroomManager');
// const makeHandler = require('./handlers');

const clientManager = ClientManager();
const chatroomManager = ChatroomManager();

module.exports = (io) => {
    io.on('connection', function (socket) {
        // const handleEvent = makeHandler(client, clientManager, chatroomManager);

        clientManager.addClient(socket);

        // TODO : CAn we remove the next socket message?
        socket.on(SocketEvents.REGISTER_TO_CHAT, function (userStr) {
            const user = JSON.parse(userStr);

            let isUserRegisteredToChat = false;
            if (clientManager.isUserAvailable(user.username)) {
                clientManager.registerClient(socket, user);
                isUserRegisteredToChat = true;
            }

            const response = JSON.stringify({
                isUserRegisteredToChat: isUserRegisteredToChat
            });

            socket.emit(SocketEvents.REGISTER_TO_CHAT_RESPONSE, response);
        });

        socket.on(SocketEvents.CREATE_CHATROOM, function (roomName) {
            let response = {
                roomName = roomName,
                isCreated: chatroomManager.addChatroom(roomName, socket.id)
            };

            if(response.isCreated) {
                socket.join(roomName);
                io.emit(SocketEvents.GET_CHATROOMS_RESPONSE, chatroomManager.getChatrooms());
            } 
            
            socket.emit(SocketEvents.CREATE_CHATROOM_RESPONSE, JSON.stringify(response));
        });

        socket.on(SocketEvents.JOIN_CHATROOM, function () {
            const defaultChatroom = "default_room";
            socket.join(defaultChatroom);
            if(chatroomManager.addChatroom(defaultChatroom, socket.id)) {
                io.emit(SocketEvents.GET_CHATROOMS_RESPONSE, chatroomManager.getChatrooms());
            } else {
                chatroomManager.addClientToChatroom(defaultChatroom, socket.id);
                socket.emit(SocketEvents.JOIN_CHATROOM_RESPONSE, defaultChatroom);
            }
        });

        socket.on(SocketEvents.JOIN_SPECIFIC_CHATROOM, function (roomName) {
            const response = {
                roomName = roomName,
                isJoined: chatroomManager.addClientToChatroom(roomName, socket.id)
            };

            if(response.isJoined) {
                socket.join(roomName);
            } 

            socket.emit(SocketEvents.JOIN_CHATROOM_RESPONSE, JSON.stringify(response));
        });

        socket.on(SocketEvents.LEAVE_SPECIFIC_CHATROOM, function(roomName) {
            const response = {
                roomName = roomName,
                isJoined: chatroomManager.removeClientFromChatroom(roomName, socket.id)
            };

            if(response.isJoined) {
                socket.leave(roomName);
            }
            
            socket.emit(SocketEvents.LEAVE_CHATROOM_RESPONSE, JSON.stringify(response));

        });

        socket.on(SocketEvents.SEND_MESSAGE, function(messageData) {
            io.to('default_room').emit(SocketEvents.MESSAGE_SENT, messageData);
        });

        socket.on(SocketEvents.GET_CHATROOMS, function() {
            socket.emit(SocketEvents.GET_CHATROOMS_RESPONSE, chatroomManager.getChatrooms());
        });

        socket.on('disconnect', function () {
             console.log('client disconnect...', socket.id);
        });

        socket.on('error', function (err) {
            console.log('received error from client:', socket.id);
            console.log(err);
        });
    });
};
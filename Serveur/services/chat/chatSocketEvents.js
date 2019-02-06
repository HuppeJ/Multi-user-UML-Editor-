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

        socket.on(SocketEvents.JOIN_CHATROOM, function () {
            const defaultChatroom = "default_room";
            socket.join(defaultChatroom);
            console.log(`JOIN_CHATROOM`, defaultChatroom);
            
            if(chatroomManager.addChatroom(defaultChatroom, socket.id)) {
                io.emit(SocketEvents.GET_CHATROOMS_RESPONSE, chatroomManager.getChatrooms());
            } else {
                chatroomManager.addClientToChatroom(defaultChatroom, socket.id);
                socket.emit(SocketEvents.JOIN_CHATROOM_RESPONSE, defaultChatroom);
            }
        });

        socket.on(SocketEvents.JOIN_SPECIFIC_CHATROOM, function (roomName) {
            socket.join(roomName);
            if(chatroomManager.addChatroom(roomName, socket.id)) {
                io.emit(SocketEvents.GET_CHATROOMS_RESPONSE, chatroomManager.getChatrooms());
            } else {
                chatroomManager.addClientToChatroom(roomName, socket.id);
                socket.emit(SocketEvents.JOIN_CHATROOM_RESPONSE, roomName);
            }
        });

        socket.on(SocketEvents.LEAVE_SPECIFIC_CHATROOM, function(roomName) {
            if(chatroomManager.removeClientFromChatroom(roomName, socket.id)) {
                socket.leave(roomName);
            }
        });

        socket.on(SocketEvents.SEND_MESSAGE, function(messageDataStr) {
            const messageData = JSON.parse(messageDataStr);

            const date = new Date();
            const timestamp = date.getTime();

            messageData.createdAt = timestamp;
            const response = JSON.stringify(messageData);
        
            console.log(`SEND_MESSAGE, response:`, response);

            io.to('default_room').emit(SocketEvents.MESSAGE_SENT, response);
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
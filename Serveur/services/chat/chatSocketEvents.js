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
            socket.join('default_room');
            if(chatroomManager.addChatroom('default_room', socket.id)) {
                io.emit(SocketEvents.GET_CHATROOMS_RESPONSE, chatroomManager.getChatrooms());
            } else {
                chatroomManager.addClientToChatroom(roomName, socketId);
            }
        });

        socket.on(SocketEvents.JOIN_SPECIFIC_CHATROOM, function (roomName) {
            socket.join(roomName);
            if(chatroomManager.addChatroom(roomName, socket.id)) {
                io.emit(SocketEvents.GET_CHATROOMS_RESPONSE, chatroomManager.getChatrooms());
            } else {
                chatroomManager.addClientToChatroom(roomName, socketId);
            }
        });

        socket.on(SocketEvents.LEAVE_SPECIFIC_CHATROOM, function(roomName) {
            if(chatroomManager.removeClientFromChatroom(roomName, socket.id)) {
                socket.leave(roomName);
            }
        });

        socket.on(SocketEvents.SEND_MESSAGE, function(messageData) {
            io.to('default_room').emit(SocketEvents.MESSAGE_SENT, messageData);
        });

        // TODO : Utiliser le chatroom manager
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
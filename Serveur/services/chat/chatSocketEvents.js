const SocketEvents = require('../../constants/SocketEvents');

const ChatroomManager = require('./components/ChatroomManager');
const chatroomManager = ChatroomManager();

module.exports = (io) => {
    io.on('connection', function (socket) {
        console.log(socket.id + " connected on the server");

        function getRoom() {

        }

        socket.on(SocketEvents.CREATE_CHATROOM, function (roomName) {
            const response = {
                roomName: roomName,
                isCreated: chatroomManager.addChatroom(roomName, socket.id)
            };

            if (response.isCreated) {
                socket.join(roomName);
                console.log(socket.id + " created chatroom " + roomName);
                io.emit(SocketEvents.GET_CHATROOMS_RESPONSE, chatroomManager.getChatrooms());
            } else {
                console.log(socket.id + " failed to create chatroom " + roomName);
            }
            
            socket.emit(SocketEvents.CREATE_CHATROOM_RESPONSE, JSON.stringify(response));
        });

        socket.on(SocketEvents.JOIN_CHATROOM, function () {
            const defaultChatroom = "default_room";
            socket.join(defaultChatroom);
            // console.log(`JOIN_CHATROOM`, defaultChatroom);
            
            if (chatroomManager.addChatroom(defaultChatroom, socket.id)) {
                io.emit(SocketEvents.GET_CHATROOMS_RESPONSE, chatroomManager.getChatrooms());
            } else {
                chatroomManager.addUserToChatroom(defaultChatroom, socket.id);
                socket.emit(SocketEvents.JOIN_CHATROOM_RESPONSE, defaultChatroom);
            }
        });

        socket.on(SocketEvents.JOIN_SPECIFIC_CHATROOM, function (roomName) {
            const response = {
                roomName: roomName,
                isJoined: chatroomManager.addUserToChatroom(roomName, socket.id)
            };

            if (response.isJoined) {
                socket.join(roomName);
                console.log(socket.id + " joined chatroom " + roomName);
            }

            console.log(socket.id + " failed to join chatroom " + roomName);
            socket.emit(SocketEvents.JOIN_CHATROOM_RESPONSE, JSON.stringify(response));
        });

        socket.on(SocketEvents.LEAVE_SPECIFIC_CHATROOM, function(roomName) {
            const response = {
                roomName: roomName,
                isJoined: chatroomManager.removeClientFromChatroom(roomName, socket.id)
            };

            if (response.isJoined) {
                socket.leave(roomName);
                console.log(socket.id + " left chatroom " + roomName);
            }
            
            console.log(socket.id + " failed to leave chatroom " + roomName);
            socket.emit(SocketEvents.LEAVE_CHATROOM_RESPONSE, JSON.stringify(response));

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
            console.log("Sending the chatrooms on the server: " + chatroomManager.getChatrooms());
            socket.emit(SocketEvents.GET_CHATROOMS_RESPONSE, chatroomManager.getChatrooms());
        });

        socket.on(SocketEvents.GET_CHATROOM_CLIENTS, function(roomName) {
            console.log("Sending the chatroom " + roomName + " clients: " + chatroomManager.getChatroomClients(roomName));
            socket.emit(SocketEvents.GET_CHATROOM_CLIENTS_RESPONSE, chatroomManager.getChatroomClients(roomName));
        });

        socket.on('disconnect', function () {
            console.log('client disconnected...', socket.id);
        });

        socket.on('error', function (err) {
            console.log('received error from client:', socket.id);
            console.log(err);
        });
    });
};
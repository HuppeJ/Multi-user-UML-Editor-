import * as SocketEvents from "../../constants/SocketEvents";

module.exports = (io) => {
    io.on('connection', function (socket) {
        socket.on(SocketEvents.CREATE_CHATROOM, function (roomName) {
            let response = {
                roomName: roomName,
                isCreated: chatroomManager.addChatroom(roomName, socket.id)
            };

            if(response.isCreated) {
                socket.join(roomName);
                console.log(socket.id + " created chatroom " + roomName);
                io.emit(SocketEvents.GET_CHATROOMS_RESPONSE, chatroomManager.getChatrooms());
            } else {
                console.log(socket.id + " failed to create chatroom " + roomName);
            }
            
            socket.emit(SocketEvents.CREATE_CHATROOM_RESPONSE, JSON.stringify(response));
        });


    });
};
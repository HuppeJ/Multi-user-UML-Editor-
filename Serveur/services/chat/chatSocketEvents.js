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
            // let joinedChatroom = true;

            socket.join('default_room');

            // const response = JSON.stringify({
                // joinedChatroom: joinedChatroom
            // });

            // socket.emit(SocketEvents.JOIN_CHATROOM_RESPONSE, response);
        });

        // socket.on('leave', handleLeave);

        socket.on(SocketEvents.SEND_MESSAGE, function(messageData) {
            io.to('default_room').emit(SocketEvents.MESSAGE_SENT, messageData);
        });

        // socket.on('chatrooms', handleGetChatrooms);

        // socket.on('availableUsers', handleGetAvailableUsers);

        // socket.on('disconnect', function () {
        //     console.log('client disconnect...', socket.id);
        //     handleDisconnect();
        // })

        // socket.on('error', function (err) {
        //     console.log('received error from client:', socket.id);
        //     console.log(err);
        // })
    })
};
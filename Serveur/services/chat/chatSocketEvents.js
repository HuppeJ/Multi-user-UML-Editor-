// Based on https://github.com/justadudewhohacks/websocket-chat

const ClientManager = require('./components/ClientManager');
const ChatroomManager = require('./components/ChatroomManager');
const makeHandlers = require('./handlers');

const clientManager = ClientManager();
const chatroomManager = ChatroomManager();

module.exports = (io) => {
    io.on('connection', function (client) {
        const {
            handleRegister,
            handleJoin,
            handleLeave,
            handleMessage,
            handleGetChatrooms,
            handleGetAvailableUsers,
            handleDisconnect
        } = makeHandlers(client, clientManager, chatroomManager);

        console.log('client connected...', client.id);
        clientManager.addClient(client);

        client.on('register', handleRegister);

        client.on('join', handleJoin);

        client.on('leave', handleLeave);

        client.on('message', handleMessage);

        client.on('chatrooms', handleGetChatrooms);

        client.on('availableUsers', handleGetAvailableUsers);

        client.on('disconnect', function () {
            console.log('client disconnect...', client.id);
            handleDisconnect();
        })

        client.on('error', function (err) {
            console.log('received error from client:', client.id);
            console.log(err);
        })
    })
};
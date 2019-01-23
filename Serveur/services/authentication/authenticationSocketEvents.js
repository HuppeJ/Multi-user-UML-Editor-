// Based on https://github.com/justadudewhohacks/websocket-chat

const UserAccountManager = require('./components/UserAccountManager');
const userAccountManager = UserAccountManager();

module.exports = (io) => {
    io.on('connection', function (user) {
        const {
            handleAddUser,
            handleLoginUser,
            handleLeave,
            handleMessage,
            handleGetChatrooms,
            handleGetAvailableUsers,
            handleDisconnect
        } = makeHandlers(user, userAccountManager);

        client.on('createUser', handleAddUser);

        client.on('loginUser', handleLoginUser);

        client.on('disconnect', function () {
            console.log('client disconnect...', client.id);
        })

        client.on('error', function (err) {
            console.log('received error from client:', client.id);
            console.log(err);
        })
    })
};
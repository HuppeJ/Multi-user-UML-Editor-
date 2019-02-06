// Based on https://github.com/justadudewhohacks/websocket-chat
const SocketEvents = require('../../SocketEvents');
const UserAccountManager = require('./components/UserAccountManager');
const userAccountManager = UserAccountManager();

module.exports = (io) => {
    io.on('connection', function (socket) {
        socket.on(SocketEvents.CREATE_USER, async function (dataStr) {
            let isUserCreated = false;
            let data = JSON.parse(dataStr);

            if (await userAccountManager.isUsernameAvailable(data.username)) {
                userAccountManager.addUser(data.username, data.password);
                isUserCreated = true;
            }
            const response = JSON.stringify({
                isUserCreated: isUserCreated
            });

            console.log(`CREATE_USER, isUserCreated:`, isUserCreated)

            socket.emit(SocketEvents.CREATE_USER_RESPONSE, response);
        });

        socket.on(SocketEvents.LOGIN_USER, async function (dataStr) {
            let isLoginSuccessful = false;
            let data = JSON.parse(dataStr);

            if (await userAccountManager.authenticateUser(data.username, data.password)) {
                isLoginSuccessful = true;
            }
            const response = JSON.stringify({
                isLoginSuccessful: isLoginSuccessful
            });

            console.log(`LOGIN_USER, isLoginSuccessful:`, isLoginSuccessful)

            socket.emit(SocketEvents.LOGIN_USER_RESPONSE, response);
        });
    })
};
// Based on https://github.com/justadudewhohacks/websocket-chat
const SocketEvents = require('../../SocketEvents');
const UserAccountManager = require('./components/UserAccountManager');
const userAccountManager = UserAccountManager();

module.exports = (io) => {
    io.on('connection', function (client) {
        client.on(SocketEvents.CREATE_USER, async function (dataStr) {
            let isUserCreated = false;
            let data = JSON.parse(dataStr);

            if (await userAccountManager.isUsernameAvailable(data.username)) {
                userAccountManager.addUser(data.username, data.password);
                isUserCreated = true;
            }
            const response = JSON.stringify({
                isUserCreated: isUserCreated
            });

            client.emit(SocketEvents.CREATE_USER_RESPONSE, response);
        });

        client.on(SocketEvents.LOGIN_USER, async function (dataStr) {
            let isLoginSuccessful = false;
            let data = JSON.parse(dataStr);

            if (await userAccountManager.authenticateUser(data.username, data.password)) {
                isLoginSuccessful = true;
            }
            const response = JSON.stringify({
                isLoginSuccessful: isLoginSuccessful
            });

            client.emit(SocketEvents.LOGIN_USER_RESPONSE, response);
        });
    })
};
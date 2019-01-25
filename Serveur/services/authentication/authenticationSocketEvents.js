// Based on https://github.com/justadudewhohacks/websocket-chat
const SocketEvents = require('../../SocketEvents');
const UserAccountManager = require('./components/UserAccountManager');
const userAccountManager = UserAccountManager();

module.exports = (io) => {
    io.on('connection', function (client) {
        // TODO : remove
        client.on('test', function () {
            client.emit('hello');
        });

        client.on(SocketEvents.CREATE_USER, async function (data) {
            let isUserCreated = false;
            if (await userAccountManager.isUsernameAvailable(data.username)) {
                userAccountManager.addUser(data.username, data.password);
                isUserCreated = true;
            }
            const response = {
                isUserCreated: isUserCreated,
                message: isUserCreated ? `User ${data.username} has been created` : `${data.username} is not valid.`
            }
            client.emit(SocketEvents.CREATE_USER_RESPONSE, response);
        });

        client.on(SocketEvents.LOGIN_USER, async function (username, password) {
            let isLoginSuccessful = false;
            if (await userAccountManager.authenticateUser(username, password)) {
                isLoginSuccessful = true;
            }
            const response = {
                isLoginSuccessful: isLoginSuccessful,
                message: isLoginSuccessful ? `Login Successful.` : `Login not Successful.`
            }

            client.emit(SocketEvents.LOGIN_USER_RESPONSE, response);
        });
    })
};
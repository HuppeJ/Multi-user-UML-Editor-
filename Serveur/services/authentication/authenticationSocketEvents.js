// Based on https://github.com/justadudewhohacks/websocket-chat

const UserAccountManager = require('./components/UserAccountManager');
const userAccountManager = UserAccountManager();

module.exports = (io) => {
    io.on('connection', function (client) {
        // TODO : remove
        client.on('test', function () {
            client.emit('hello');
        });

        client.on('createUser', async function (data) {
            let isUserCreated = false;
            if (await userAccountManager.isUsernameAvailable(data.username)) {
                userAccountManager.addUser(data.username, data.password);
                isUserCreated = true;
            }
            const response = {
                isUserCreated: isUserCreated,
                message: isUserCreated ? `User ${data.username} has been created` : `${data.username} is not valid.`
            }
            client.emit('createUserResponse', response);
        });

        client.on('loginUser', async function (username, password) {
            let isLoginSuccessful = false;
            if (await userAccountManager.authenticateUser(username, password)) {
                isLoginSuccessful = true;
            }
            const response = {
                isLoginSuccessful: isLoginSuccessful,
                message: isLoginSuccessful ? `Login Successful.` : `Login not Successful.`
            }

            client.emit('loginUserResponse', response);
        });
    })
};
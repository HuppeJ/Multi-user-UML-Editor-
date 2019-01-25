// Based on https://github.com/justadudewhohacks/websocket-chat

const UserAccountManager = require('./components/UserAccountManager');
const userAccountManager = UserAccountManager();

module.exports = (io) => {
    io.on('connection', function (client) {
        client.on('test', function () {
            client.emit('hello');
        });

        client.on('createUser', function (data) {
            console.log("Username: " + data.username);
            console.log("Password: " + data.password);
            if (userAccountManager.isUsernameAvailable(data.username)) {
                userAccountManager.addUser(data.username, data.password);
                client.emit('userCreated', data.username);
            }
            else
                client.emit('usernameUnavailable', data.username);
        });

        client.on('loginUser', function (username, password) {
            if (userAccountManager.authenticateUser(username, password)) {
                client.emit('loginSuccessful');
            }
            else
                client.emit('loginFailed');
        });
    })
};
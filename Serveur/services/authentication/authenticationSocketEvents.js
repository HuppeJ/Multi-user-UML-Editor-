// Based on https://github.com/justadudewhohacks/websocket-chat

const UserAccountManager = require('./components/UserAccountManager');
const userAccountManager = UserAccountManager();

module.exports = (io) => {
    io.on('connection', function (client) {
        client.on('test', function () {
            client.emit('hello');
        });

        client.on('createUser', function (user) {
            if (userAccountManager.isUsernameAvailable(user.username)) {
                userAccountManager.addUser(user);
            }
        });

        client.on('loginUser', function (user) {
            userAccountManager.authenticateUser(user.username, user.password);
        });
    })
};
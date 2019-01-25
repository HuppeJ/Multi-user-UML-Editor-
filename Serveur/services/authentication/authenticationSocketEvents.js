// Based on https://github.com/justadudewhohacks/websocket-chat

const UserAccountManager = require('./components/UserAccountManager');
const userAccountManager = UserAccountManager();

module.exports = (io) => {
    io.on('connection', function (user) {

        client.on('createUser', function () {
            if (userAccountManager.isUsernameAvailable(user.username))
                userAccountManager.addUser(user);
        });

        client.on('loginUser', userAccountManager.authenticateUser(user.username, user.password));

        client.on('disconnect', function () {
            console.log('client disconnect...', client.id);
        })

        client.on('error', function (err) {
            console.log('received error from client:', client.id);
            console.log(err);
        })
    })
};
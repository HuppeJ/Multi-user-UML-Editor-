const datastore = require("./../../datastore/datastore");
const crypto = require('crypto');

module.exports = function () {
  // connectedUsers is a Map : [key: username, value: socketId]
  const connectedUsers = new Map();

  async function addUser(username, password) {
    const user = {
      username: username,
      // Store a hash of the password
      password: crypto
        .createHash('sha256')
        .update(password)
        .digest('hex')
    };
    // console.log("User: " + user.username + " -> " + user.password);
    return datastore.save({
      key: datastore.key('User'),
      data: user
    });
  }

  async function isUsernameAvailable(username) {
    const query = datastore
    .createQuery('User')
    .filter('username', '=', username)
    .limit(1);
    
    const users = await datastore.runQuery(query);

    return users[0][0] === undefined;
  }

  async function authenticateUser(username, password, socketId) {
    if (isConnected(username)) {
      return false;
    }
    const query = datastore
    .createQuery('User')
    .filter('username', '=', username)
    .limit(1);

    const users = await datastore.runQuery(query);
    if (users[0][0] !== undefined) {
      const user = users[0].map(
        entity => { return {'password': entity.password} }
      );
      if (user[0].password === crypto.createHash('sha256').update(password).digest('hex')) {
        connectedUsers.set(socketId, username);
        return true;
      }
    }
    return false;
  }

  function disconnectUser(socketId) {
    return connectedUsers.delete(socketId);
  }

  function isConnected(username) {
    return Array.from(connectedUsers.values()).includes(username);
  }

  function removeUser(username) {
    //TODO
  }

  return {
    addUser,
    isUsernameAvailable,
    authenticateUser,
    removeUser,
    isConnected,
    disconnectUser
  }
}

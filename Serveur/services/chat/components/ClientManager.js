// Based on https://github.com/justadudewhohacks/websocket-chat

module.exports = function () {
  // mapping of all connected clients
  const sockets = new Map()

  function addClient(socket) {
    sockets.set(socket.id, { socket });
  }

  function registerClient(socket, user) {
    sockets.set(socket.id, { socket, user });
  }

  function removeClient(socket) {
    sockets.delete(socket.id);
  }

  function getUnavailableUsers() {
    return usersTaken = new Set(
      Array.from(sockets.values())
        .filter(c => c.user)
        .map(c => c.user.username)
    );
  }

  function isUserAvailable(username) {
    return !getUnavailableUsers().has(username);
  }

  // function getUserByName(username) {
  //   return userTemplates.find(u => u.username === username);
  // }

  function getUserByClientId(clientId) {
    return (sockets.get(clientId) || {}).user;
  }

  return {
    addClient,
    registerClient,
    removeClient,
    getUnavailableUsers,
    isUserAvailable,
    getUserByClientId
  }
}

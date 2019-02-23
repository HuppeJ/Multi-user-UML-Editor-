const crypto = require('crypto');

export default class UserAccountManager {
  // connectedUsers is a Map : [key: socketId, value: username]
  private connectedUsers: any = null;
  private datastore: any = null;

  constructor(datastore: any) {
    this.connectedUsers = new Map();
    this.datastore = datastore;
  }


  public async addUser(name: string, password: string) {
    const user = {
      username: name,
      // Store a hash of the password
      password: crypto
        .createHash('sha256')
        .update(password)
        .digest('hex'),
    };

    return this.datastore.save({
      key: this.datastore.key('User'),
      data: user
    });
  }

  public async isUsernameAvailable(username: string) {
    const query = this.datastore
    .createQuery('User')
    .filter('username', '=', username)
    .limit(1);

    const users = await this.datastore.runQuery(query);

    return users[0][0] === undefined;
  }

  public async authenticateUser(username: string, password: string, socketId: any) {
    if (this.isConnected(username)) {
      return false;
    }
    const query = this.datastore
    .createQuery('User')
    .filter('username', '=', username)
    .limit(1);

    const users = await this.datastore.runQuery(query);
    if (users[0][0] !== undefined) {
      const user = users[0].map(
        (entity: any) => {
          return {password: entity.password};
        },
      );
      if (user[0].password === crypto.createHash('sha256').update(password).digest('hex')) {
        this.connectedUsers.set(socketId, username);
        return true;
      }
    }
    return false;
  }

  public disconnectUser(socketId: any) {
    return this.connectedUsers.delete(socketId);
  }

  public isConnected(username: string) {
    for (const [k, v] of this.connectedUsers) {
      if (v === username) {
        return true;
      }
    }
    return false;
    // Before : return Array.from(this.connectedUsers.values()).includes(username);
  }
}

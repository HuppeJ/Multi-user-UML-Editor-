/**
 * Copyright 2017, Google, Inc.
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

'use strict'; // is used by new versions of Javascript to enforce secure coding practices

// Configure .env file 
const dotenv = require('dotenv');
dotenv.config();

const express = require('express');
const app = express();
app.enable('trust proxy');

const crypto = require('crypto');

// Set up the express routing system
var routes = require('./routes/routes');
routes(app);

const http = require('http');
const server = http.createServer(app);

const socketIO = require('socket.io');
const io = socketIO(server);

// Initialise Socket Events
const chatSocketEvents = require('./services/chat/chatSocketEvents');
chatSocketEvents(io);
const authenticationSocketEvents = require('./services/authentication/authenticationSocketEvents');
authenticationSocketEvents(io);

// Set up the Socket.io communication system
io.on('connection', (client) => {
  console.log('New client connected')
});

require('socketio-auth')(io, {
  authenticate: authenticate,
  // postAuthenticate: postAuthenticate,
  // disconnect: disconnect,
  timeout: 1000
});

function authenticate(socket, data, callback) {
  var username = data.username;
  var password = data.password;
 
  try {
    findUser(username, password);
  }
  catch (err) {
    console.log(err);
  }
}

const UserAccountManager = require('./services/authentication/components/UserAccountManager');
const userAccountManager = UserAccountManager();

app.get('/setUser', async (req, res, next) => {
  // Create a user account to be stored in the database
  const user = {
    username: "WAZZZZZA654738GTDW",
    // Store a hash of the password
    password: crypto
      .createHash('sha256')
      .update("un mot de passe que j'aime")
      .digest('hex')
  };

  try {
    if (await userAccountManager.isUsernameAvailable(user.username)) {
      await userAccountManager.addUser(user);
      res
      .status(200)
      .set('Content-Type', 'text/plain')
      .send(`Created user ${user.username}`)
      .end();
    }
    else {
      res
      .status(200)
      .set('Content-Type', 'text/plain')
      .send(`User ${user.username} already exists`)
      .end();
    }
  } catch (error) {
    console.log(error);
    next(error);
  }
});

app.get('/login', async (req, res, next) => {
  try {
    const result = await userAccountManager.authenticateUser("Something", "anything");
    if (result) {
      res
      .status(200)
      .set('Content-Type', 'text/plain')
      .send(`Login Successful\n${result}`)
      .end();
    } else {
      res
      .status(200)
      .set('Content-Type', 'text/plain')
      .send(`Login failed`)
      .end();
    }
  } catch (error) {
    next(error);
  }
})

const PORT = process.env.PORT;
server.listen(PORT, () => {
  console.log(`App listening on port ${PORT}`);
  console.log('Press Ctrl+C to quit.');
});


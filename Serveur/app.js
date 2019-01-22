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

// Set up the datastore
const Datastore = require('@google-cloud/datastore');

// Instantiate a datastore client
const datastore = new Datastore({
  projectId: 'projet-3-228722',
  keyFilename: './keys/key.json'
});

const http = require('http');
const server = http.createServer(app);

const socketIO = require('socket.io');
const io = socketIO(server);

// Initialise Socket Events
const chatSocketEvents = require('./services/chat/chatSocketEvents');
chatSocketEvents(io);

// Set up the Socket.io communication system
io.on('connection', (client) => {
  console.log('New client connected')
});

// TODO : section test

/**
 * Insert a visit record into the database.
 *
 * @param {object} visit The visit record to insert.
 */
function insertVisit(visit) {
  return datastore.save({
    key: datastore.key('visit'),
    data: visit,
  });
}

/**
 * Retrieve the latest 10 visit records from the database.
 */
function getVisits() {
  const query = datastore
    .createQuery('visit')
    .order('timestamp', {descending: true})
    .limit(10);

  return datastore.runQuery(query);
}

app.get('/visit', async (req, res, next) => {
  // Create a visit record to be stored in the database
  const visit = {
    timestamp: new Date(),
    // Store a hash of the visitor's ip address
    userIp: crypto
      .createHash('sha256')
      .update(req.ip)
      .digest('hex')
      .substr(0, 7),
  };

  try {
    await insertVisit(visit);
    const results = await getVisits();
    const entities = results[0];
    const visits = entities.map(
      entity => `Time: ${entity.timestamp}, AddrHash: ${entity.userIp}`
    );
    res
      .status(200)
      .set('Content-Type', 'text/plain')
      .send(`Last 10 visits:\n${visits.join('\n')}`)
      .end();
  } catch (error) {
    next(error);
  }
});

// TODO : section test fin

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

/**
 * Insert a user account into the database.
 *
 * @param {object} user The visit record to insert.
 */
function insertUser(user) {
  return datastore.save({
    key: datastore.key('User'),
    data: user
  });
}

/**
 * Retrieve a user account from the database.
 */
async function findUser(username, password) {
  const query = datastore
    .createQuery('User')
    .filter('username', '=', username);

  const users = await datastore.runQuery(query);
  const user = users[0].map(
    entity => { return {'password': entity.password} }
  );
  console.log(user);
  if (user !== undefined && user[0].password === crypto.createHash('sha256').update(password).digest('hex')) {
    console.log("Connection successful");
    return true;
  } else {
    console.log("Invalid user or password");
    return false;
  }
}

app.get('/setUser', async (req, res, next) => {
  // Create a user account to be stored in the database
  const user = {
    username: "Something",
    // Store a hash of the visitor's ip address
    password: crypto
      .createHash('sha256')
      .update("anything")
      .digest('hex')
  };

  try {
    await insertUser(user);
    res
      .status(200)
      .set('Content-Type', 'text/plain')
      .send(`Created user\n${user}`)
      .end();
  } catch (error) {
    console.log(error);
    next(error);
  }
});

app.get('/login', async (req, res, next) => {
  try {
    const result = await findUser("Something", "anything");
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


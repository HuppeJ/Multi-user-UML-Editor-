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


// TODO : const PORT = process.env.PORT;
const PORT = 3300;
server.listen(PORT, () => {
  console.log(`App listening on port ${PORT}`);
  console.log('Press Ctrl+C to quit.');
});


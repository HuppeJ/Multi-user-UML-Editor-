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

// Set up the express routing system
var routes = require('./routes/routes');
routes(app);

const http = require('http');
const server = http.createServer(app);

const socketIO = require('socket.io');
const io = socketIO(server);

// const chatIo = io.of("/chat");

// Initialise Socket Events
const chatSocketEvents = require('./services/chat/chatSocketEvents');
chatSocketEvents(io);
const authenticationSocketEvents = require('./services/authentication/authenticationSocketEvents');
authenticationSocketEvents(io);

// Set up the Socket.io communication system
io.on('connection', (client) => {
    console.log('New client connected');

    // TODO : remove
    client.on('test', function () {
        client.emit('hello');
    });
});

const PORT = process.env.PORT;
server.listen(PORT, () => {
    console.log(`App listening on port ${PORT}`);
    console.log('Press Ctrl+C to quit.');
});


// Configure .env file 
const dotenv = require('dotenv');
dotenv.config();

import express from 'express';
const app = express();
app.enable('trust proxy');

// Set up the express routing system
// var routes = require('./routes/routes');
// routes(app);

const http = require('http');
const server = http.createServer(app);

const socketIO = require('socket.io');
const io = socketIO(server);

// Initialise components
const UserAccountManager = require('../components/UserAccountManager');
const userAccountManager = UserAccountManager();

// Initialise Socket Events
const chatSocketEvents = require('../services/chat/chatSocketEvents');
chatSocketEvents(io);
const authenticationSocketEvents = require('../services/authentication/authenticationSocketEvents');
authenticationSocketEvents(io, userAccountManager);

// Set up the Socket.io communication system
io.on('connection', (client: any) => {
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




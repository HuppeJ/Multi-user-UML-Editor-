/* tslint:disable */

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
import datastore from "./services/datastore/datastore";

import UserAccountManager from "./components/UserAccountManager";
const userAccountManager = new UserAccountManager(datastore);

import ChatroomManager from "./services/chat/components/ChatroomManager";
const chatroomManager = new ChatroomManager();

import CanvasManager from "./services/canvas/components/CanvasManager";
const canvasManager = new CanvasManager();


// Initialise Socket Events
import ChatSocketEvents from "./services/chat/ChatSocketEvents";
new ChatSocketEvents(io, chatroomManager);

import AuthenticationSocketEvents from "./services/Authentication/AuthenticationSocketEvents";
new AuthenticationSocketEvents(io, userAccountManager);

import CanvasSocketEvents from "./services/canvas/CanvasSocketEvents";
new CanvasSocketEvents(io, canvasManager);

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

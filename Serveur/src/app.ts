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

import CanvasGallerySocketEvents from './services/canvas/CanvasGallerySocketEvents';
new CanvasGallerySocketEvents(io, canvasManager);

import CanvasEditionSocketEvents from './services/canvas/CanvasEditionSocketEvents';
import CanvasRoom from './services/canvas/components/CanvasRoom';
import { IUpdateFormsData, IUpdateLinksData, IEditGalleryData } from './services/canvas/interfaces/interfaces';
new CanvasEditionSocketEvents(io, canvasManager);

// Set up the Socket.io communication system
io.on('connection', (socket: any) => {
    // TODO : remove
    socket.on('test', function () {
        socket.emit('hello');
    });

    socket.on("disconnect", function () {
        console.log("Socket " + socket.id + " disconnected");

        try {
            const username: string = userAccountManager.getUsernameBySocketId(socket.id);
            if (username != null) {
                const canvasRoomId: string = canvasManager.getCanvasRoomFromUsername(username);
                if (canvasRoomId != null) {

                    const canvasRoom: CanvasRoom = canvasManager.canvasRooms.get(canvasRoomId);
                    if (canvasRoom != null) {

                        const selectedFormsByUser: any[] = canvasRoom.getSelectedFormsByUser(username);
                        const formsData: IUpdateFormsData = {
                            username: username,
                            canevasName: canvasManager.getNameFromCanvasRoomId(canvasRoomId),
                            forms: selectedFormsByUser,
                        }

                        if (canvasManager.deselectCanvasForms(canvasRoomId, formsData)) {
                            io.to(canvasRoomId).emit("formsDeselected", JSON.stringify(formsData));
                        }

                        const selectedLinksByUser: any[] = canvasRoom.getSelectedLinksByUser(username);
                        const linksData: IUpdateLinksData = {
                            username: username,
                            canevasName: canvasManager.getNameFromCanvasRoomId(canvasRoomId),
                            links: selectedLinksByUser,
                        }

                        if (canvasManager.deselectCanvasLinks(canvasRoomId, linksData)) {
                            io.to(canvasRoomId).emit("linksDeselected", JSON.stringify(linksData));
                        }

                        const canvasData: IEditGalleryData = {
                            username: username,
                            canevasName: canvasManager.getNameFromCanvasRoomId(canvasRoomId),
                            password: "",
                        }
                        if (canvasManager.deselectCanvas(canvasRoomId, canvasData)) {
                            io.to(canvasRoomId).emit("canvasDeselected", JSON.stringify(canvasData));
                        }

                        canvasRoom.removeUser(username);
                    }
                }
            }

            if (userAccountManager.disconnectUser(socket.id)) {
                console.log(socket.id + " disconnected its user");
            } else {
                console.log(socket.id + " failed to disconnect its user");
            }

        } catch (e) {
            console.log("[Error]: ", e)
        }


    });

    socket.on('getServerState', function () {
        const response = {
            canvasManager_canvasRooms: JSON.parse(canvasManager.getCanvasRoomsSERI())
        };

        socket.emit("getServerStateResponse", JSON.stringify(response));
    });

});

const PORT = process.env.PORT;
server.listen(PORT, () => {
    console.log(`App listening on port ${PORT}`);
    console.log('Press Ctrl+C to quit.');
});

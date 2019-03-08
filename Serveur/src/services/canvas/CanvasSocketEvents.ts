import * as SocketEvents from "../../constants/SocketEvents";
import { ICanevas, IEditCanevasData, IEditGalleryData } from "./interfaces/interfaces";
import CanvasManager from "./components/CanvasManager";

export const CanvasTestRoom: string = "Canvas_test_room";

export default class CanvasSocketEvents {
    constructor(io: any, canvasManager: CanvasManager) {
        io.on("connection", function (socket: any) {
            console.log(socket.id + " connected to Canvas server");

            socket.on("createCanvas", function (dataStr: string) {
                try {
                    const data: IEditCanevasData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevas.name);
    
                    const response = {
                        isCreated: canvasManager.addCanvasRoom(canvasRoomId, data)
                    };
    
                    if (response.isCreated) {
                        // (broadcast)
                        io.sockets.emit("canvasRoomCreated", canvasManager.getCanvasRoomsSERI());
                        console.log(socket.id + " created  canvasRoom " + data.canevas.name);
                    } else {
                        console.log(socket.id + " failed to create canvasRoom " + data.canevas.name);
                    }
    
                    socket.emit("createCanvasRoomResponse", JSON.stringify(response));
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });


            // TODO à compléter voir qu'est-ce qu'on fait lorsqu'il y a des utilisateurs dans une room
            socket.on("removeCanvas", function (dataStr: string) {
                try {
                    const data: IEditGalleryData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);
    
                    const response = {
                        isCanvasRoomRemoved: canvasManager.removeCanvasRoom(canvasRoomId, data)
                    };
    
                    if (response.isCanvasRoomRemoved) {
                        // TODO on gère cela ici? 
                        // io.sockets.clients(someRoom).forEach(function(s){
                        //     s.leave(someRoom);
                        // });
    
                        // (broadcast)
                        io.sockets.emit("canvasRoomRemoved", canvasManager.getCanvasRoomsSERI());
                        console.log(socket.id + " removed canvasRoom " + data.canevasName);
                    } else {
                        console.log(socket.id + " failed to remove canvasRoom " + data.canevasName);
                    }
    
                    socket.emit("removeCanvasRoomResponse", JSON.stringify(response));
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("joinCanvasRoom", function (dataStr: string) {
                try {
                    const data: IEditGalleryData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);
    
                    const response = {
                        isCanvasRoomJoined: canvasManager.addUserToCanvasRoom(canvasRoomId, data)
                    };
    
                    if (response.isCanvasRoomJoined) {
                        socket.join(canvasRoomId);
                        console.log(socket.id + " joined canvasRoom " + data.canevasName);
                    } else {
                        console.log(socket.id + " failed to join canvasRoom " + data.canevasName);
                    }
    
                    socket.emit("joinCanvasRoomResponse", JSON.stringify(response));
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("leaveCanvasRoom", function (dataStr: string) {
                try {
                    const data: IEditGalleryData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);
    
                    const response = {
                        isCanvasRoomLeaved: canvasManager.removeUserFromCanvasRoom(canvasRoomId, data)
                    };
    
                    if (response.isCanvasRoomLeaved) {
                        socket.leave(canvasRoomId);
                        console.log(socket.id + " leaved canvasRoom " + data.canevasName);
                    } else {
                        console.log(socket.id + " failed to leave canvasRoom " + data.canevasName);
                    }
    
                    socket.emit("leaveCanvasRoomResponse", JSON.stringify(response));
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("saveCanvas", function (data: any) {
                try {
                    // TODO  
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });


            // [**************************************************
            // Temporary for testing

            socket.on("joinCanvasTest", function () {
                socket.join(CanvasTestRoom);
                console.log(`joinCanvasTest`, CanvasTestRoom);
                io.to(CanvasTestRoom).emit("joinCanvasTestResponse", "joinedCanvasTestRoom");
            });

            socket.on("canvasUpdateTest", function (CanvasFormChanges: any) {
                console.log(`CanvasUpdateTest from ${socket.id}, response:`, CanvasFormChanges);
                io.to(CanvasTestRoom).emit("canvasUpdateTestResponse", CanvasFormChanges);
            });
            // ****************************************************************]

        });
    }
};
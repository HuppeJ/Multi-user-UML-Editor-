import * as SocketEvents from "../../constants/SocketEvents";
import CanvasGallerySocketEvents from "./CanvasGallerySocketEvents";
import CanvasEditionSocketEvents from "./CanvasEditionSocketEvents";
import { ICanevas } from "./interfaces/interfaces";
import CanvasManager from "./components/CanvasManager";

export const CanvasTestRoom: string = "Canvas_test_room";

export default class CanvasSocketEvents {
    constructor(io: any, canvasManager: CanvasManager) {
        io.on("connection", function (socket: any) {
            console.log(socket.id + " connected to Canvas server");

            // Save Canvas 
            socket.on("createCanvasRoom", function (data: string) { 
                try {
                    const newCanvas: ICanevas = JSON.parse(data);

                    const response = {
                        isCreated: canvasManager.addCanvasRoom(newCanvas, socket.id)
                    };

                    if (response.isCreated) {
                        const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(newCanvas.name);
                        socket.join(canvasRoomId);
                        console.log(socket.id + " created  canvasRoom " + newCanvas.name);
                        io.to(canvasRoomId).emit("canvasRoomCreated", canvasManager.getCanvasRooms());
                    } else {
                        console.log(socket.id + " failed to create canvasRoom " + newCanvas.name);
                    }
                    
                    socket.emit("createCanvasRoomResponse", JSON.stringify(response));
                } catch (e) {
                    console.log("[error]","couldnt perform: createCanvasRoom :", e);
                }
            });

            socket.on("removeCanvasRoom", function (canvasName: string) { 
                try {            
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(canvasName);

                    const response = {
                        isCanvasRoomRemoved: canvasManager.removeCanvasRoom(canvasRoomId)
                    };

                    if (response.isCanvasRoomRemoved) {
                        io.to(canvasRoomId).emit("canvasRoomRemoved", canvasManager.getCanvasRooms());
                        console.log(socket.id + " removed canvasRoom " + canvasName);
                    } else {
                        console.log(socket.id + " failed to remove canvasRoom " + canvasName);
                    }

                    socket.emit("removeCanvasRoomResponse", JSON.stringify(response));
                    
                    // TODO on gère cela là? 
                    // io.sockets.clients(someRoom).forEach(function(s){
                    //     s.leave(someRoom);
                    // });
                } catch (e) {
                    console.log("[error]","couldnt perform: removeCanvasRoom :", e);
                } 
             });

            socket.on("joinCanvasRoom", function (canvasName: string) { 
                try {
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(canvasName);

                    const response = {
                        isCanvasRoomJoined: canvasManager.addUserToCanvasRoom(canvasRoomId, socket.id)
                    };
    
                    if (response.isCanvasRoomJoined) {
                        socket.join(canvasRoomId);
                        console.log(socket.id + " joined canvasRoom " + canvasName);
                    } else {
                        console.log(socket.id + " failed to join canvasRoom " + canvasName);
                    }
    
                    socket.emit("joinCanvasRoomResponse", JSON.stringify(response));
                } catch (e) {
                    console.log("[error]","couldnt perform: joinCanvasRoom :", e);
                }
            });

            socket.on("leaveCanvasRoom", function (canvasName: string) { 
                try {
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(canvasName);

                    const response = {
                        isCanvasRoomLeaved: canvasManager.removeUserFromCanvasRoom(canvasRoomId, socket.id)
                    };
    
                    if (response.isCanvasRoomLeaved) {
                        socket.leave(canvasRoomId);
                        console.log(socket.id + " leaved canvasRoom " + canvasName);
                    } else {
                        console.log(socket.id + " failed to leave canvasRoom " + canvasName);
                    }
    
                    socket.emit("joinCanvasRoomResponse", JSON.stringify(response));
                } catch (e) {
                    console.log("[error]","couldnt perform: leaveCanvasRoom :", e);
                }
            });

            socket.on("saveCanvas", function (data: any) { 
               // TODO  
            });




            // [**************************************************
            // Temporary for testing

            socket.on("joinCanvasTest", function () {
                socket.join(CanvasTestRoom);
                console.log(`joinCanvasTest`, CanvasTestRoom);
                io.emit("joinCanvasTestResponse", "joinedCanvasTestRoom");
            });

            socket.on("canvasUpdateTest", function (CanvasFormChanges: any) {
                console.log(`CanvasUpdateTest from ${socket.id}, response:`, CanvasFormChanges);
                io.to(CanvasTestRoom).emit("canvasUpdateTestResponse", CanvasFormChanges);
            });
            // ****************************************************************]

        });
    }
};
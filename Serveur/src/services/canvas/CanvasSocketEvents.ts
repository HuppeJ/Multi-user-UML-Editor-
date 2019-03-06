import * as SocketEvents from "../../constants/SocketEvents";
import { ICanevas, IEditCanevasData } from "./interfaces/interfaces";
import CanvasManager from "./components/CanvasManager";

export const CanvasTestRoom: string = "Canvas_test_room";

export default class CanvasSocketEvents {
    constructor(io: any, canvasManager: CanvasManager) {
        io.on("connection", function (socket: any) {
            console.log(socket.id + " connected to Canvas server");

            socket.on("createCanvasRoom", function (dataStr: string) {
                const data: IEditCanevasData = JSON.parse(dataStr);
                
                const response = {
                    isCreated: canvasManager.addCanvasRoom(data)
                };

                if (response.isCreated) {
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevas.name);
                    // TODO il ne faudrait pas join la room automatiquement. 
                    // Il faudrait mémoriser quels Canvas existent et créer les canvasRoom lorsqu'il y a un user de connecté
                    socket.join(canvasRoomId);
                    console.log(socket.id + " created  canvasRoom " + data.canevas.name);
                    io.to(canvasRoomId).emit("canvasRoomCreated", canvasManager.getCanvasRoomsSERI());
                } else {
                    console.log(socket.id + " failed to create canvasRoom " + data.canevas.name);
                }

                socket.emit("createCanvasRoomResponse", JSON.stringify(response));
            });

            // + username
            socket.on("removeCanvasRoom", function (canvasName: string) {
                const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(canvasName);
                
                const response = {
                    isCanvasRoomRemoved: canvasManager.removeCanvasRoom(canvasRoomId)
                };

                if (response.isCanvasRoomRemoved) {
                    io.to(canvasRoomId).emit("canvasRoomRemoved", canvasManager.getCanvasRoomsSERI());
                    console.log(socket.id + " removed canvasRoom " + canvasName);
                } else {
                    console.log(socket.id + " failed to remove canvasRoom " + canvasName);
                }

                socket.emit("removeCanvasRoomResponse", JSON.stringify(response));

                // TODO on gère cela ici? 
                // io.sockets.clients(someRoom).forEach(function(s){
                //     s.leave(someRoom);
                // });
            });

            socket.on("joinCanvasRoom", function (canvasName: string) {
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
            });

            socket.on("leaveCanvasRoom", function (canvasName: string) {
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

                socket.emit("leaveCanvasRoomResponse", JSON.stringify(response));
            });

            socket.on("saveCanvas", function (data: any) {
                // TODO  
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
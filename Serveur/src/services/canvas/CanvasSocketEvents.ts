import * as SocketEvents from "../../constants/SocketEvents";
import CanvasGallerySocketEvents from "./CanvasGallerySocketEvents";
import CanvasEditionSocketEvents from "./CanvasEditionSocketEvents";

export default class CanvasSocketEvents {
    constructor(io: any) {
        io.on('connection', function (socket: any) {
            console.log(socket.id + " connected to Canvas server");

            // initialise other socketEvents
            new CanvasGallerySocketEvents(io);
            new CanvasEditionSocketEvents(io);


            // Save Canvas 

            socket.on("getCanvas", function (data: any) { 
                const response = { data: data, isRequestSuccessul: false }; 
                socket.emit("temp", JSON.stringify(response)); 
            });

            socket.on("saveCanvas", function (data: any) { 
                const response = { data: data, isRequestSuccessul: false }; 
                socket.emit("temp", JSON.stringify(response)); 
            });


            // [**************************************************
            // Temporary for testing
            const CanvasTestRoom: string = "Canvas_test_room";

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
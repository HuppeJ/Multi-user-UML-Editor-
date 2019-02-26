import * as SocketEvents from "../../constants/SocketEvents";
import { CanvasTestRoom } from "./CanvasSocketEvents";

export default class CanvasEditionSocketEvents {
    constructor(io: any) {
        io.on('connection', function (socket: any) {
            console.log(socket.id + " connected to CanvasEditionSocketEvents");

            // Collaborative Basic Edition
            socket.on("createForm", function (data: any) { // addForm ? 
                console.log(`createForm from ${socket.id}, response:`, data);
                io.to(CanvasTestRoom).emit("formCreated", data);
            }); 

            socket.on("updateForms", function (data: any) { 
                console.log(`updateForms from ${socket.id}, response:`, data);
                io.to(CanvasTestRoom).emit("formsUpdated", data);
            });

            socket.on("deleteForms", function (data: any) { 
                console.log(`deleteForms from ${socket.id}, response:`, data);
                io.to(CanvasTestRoom).emit("formsDeleted", data);
            });

            socket.on("selectForms", function (data: any) { 
                console.log(`selectForms from ${socket.id}, response:`, data);
                io.to(CanvasTestRoom).emit("formsSelected", data); 
            });

            socket.on("reinitialiseCanvas", function (data: any) { 
                console.log(`reinitialiseCanvas from ${socket.id}, response:`, data);
                io.to(CanvasTestRoom).emit("canvasReinitialized", data); 
            });


            
            socket.on("updatefloatingText", function (data: any) { 
                const response = { data: data, isRequestSuccessul: false }; 
              //  socket.emit("temp", JSON.stringify(response)); 
            });


            socket.on("canvasResized", function (data: any) {  // Existe vraiment, le canevas n'aura pas de taille non?
                const response = { data: data, isRequestSuccessul: false }; 
              //  socket.emit("temp", JSON.stringify(response)); 
            });

        });

    }
};
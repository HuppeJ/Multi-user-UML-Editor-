import * as SocketEvents from "../../constants/SocketEvents";

export default class CanvasGallerySocketEvents {
    constructor(io: any) {
        io.on('connection', function (socket: any) {
            console.log(socket.id + " connected to Canvas GallerySocketEvents");

            // Canvas Gallery 
            socket.on("createCanvas", function (data: any) { 
                const response = { data: data, isRequestSuccessul: false }; 
                socket.emit("temp", JSON.stringify(response)); 
            });

            socket.on("getPublicCanvas", function (data: any) { 
                const response = { data: data, isRequestSuccessul: false }; 
                socket.emit("temp", JSON.stringify(response)); 
            });

            socket.on("getPrivateCanvas", function (data: any) { 
                const response = { data: data, isRequestSuccessul: false }; 
                socket.emit("temp", JSON.stringify(response)); 
            });

            socket.on("joinCanvasRoom", function (data: any) { 
                const response = { data: data, isRequestSuccessul: false }; 
                socket.emit("temp", JSON.stringify(response)); 
            });

            socket.on("joinProtectedCanvasRoom", function (data: any) { 
                const response = { data: data, isRequestSuccessul: false }; 
                socket.emit("temp", JSON.stringify(response)); 
            });
        });

    }
};
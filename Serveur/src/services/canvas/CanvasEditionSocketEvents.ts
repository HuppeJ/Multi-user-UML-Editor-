import * as SocketEvents from "../../constants/SocketEvents";

export default class CanvasEditionSocketEvents {
    constructor(io: any) {
        io.on('connection', function (socket: any) {
            console.log(socket.id + " connected to CanvasEditionSocketEvents");

            // Collaborative Basic Edition

            socket.on("createForm", function (data: any) { // addForm ? 
                const response = { data: data, isRequestSuccessul: false }; 
                socket.emit("temp", JSON.stringify(response)); 
            }); 

            socket.on("deleteForm", function (data: any) { 
                const response = { data: data, isRequestSuccessul: false };
                socket.emit("temp", JSON.stringify(response)); 
            });

            socket.on("selectForm", function (data: any) { 
                const response = { data: data, isRequestSuccessul: false };
                socket.emit("temp", JSON.stringify(response)); 
            });


            // [****************************************************************
            // Toutes ces modification ne pourraient pas seulement être ...
            //  socket.on("updateForm", function (data: any) ?
            socket.on("moveForm", function (data: any) { const response = { data: data, isRequestSuccessul: false }; socket.emit("temp", JSON.stringify(response)); });
            socket.on("resizeForm", function (data: any) { const response = { data: data, isRequestSuccessul: false }; socket.emit("temp", JSON.stringify(response)); });
            socket.on("rotateForm", function (data: any) { const response = { data: data, isRequestSuccessul: false }; socket.emit("temp", JSON.stringify(response)); });
            socket.on("changeFormOutlineColor", function (data: any) { const response = { data: data, isRequestSuccessul: false }; socket.emit("temp", JSON.stringify(response)); });
            socket.on("changeFormOutlineStyle", function (data: any) { const response = { data: data, isRequestSuccessul: false }; socket.emit("temp", JSON.stringify(response)); });
            socket.on("changeFormFillColor", function (data: any) { const response = { data: data, isRequestSuccessul: false }; socket.emit("temp", JSON.stringify(response)); }); // Existe vraiment ? 
            socket.on("changeFormBorderWeight", function (data: any) { const response = { data: data, isRequestSuccessul: false }; socket.emit("temp", JSON.stringify(response)); });
            // *****************************************************************]

            socket.on("createImage", function (data: any) { // addImage ? 
                const response = { data: data, isRequestSuccessul: false }; 
                socket.emit("temp", JSON.stringify(response)); 
            }); 

            socket.on("deleteImage", function (data: any) {  // n'est pas dans le Protocole de communication
                const response = { data: data, isRequestSuccessul: false }; 
                socket.emit("temp", JSON.stringify(response)); 
            });


            socket.on("floatingTextModified", function (data: any) { 
                const response = { data: data, isRequestSuccessul: false }; 
                socket.emit("temp", JSON.stringify(response)); 
            });

            socket.on("canvasReinitialized", function (data: any) { 
                const response = { data: data, isRequestSuccessul: false }; 
                socket.emit("temp", JSON.stringify(response)); 
            });

            socket.on("canvasResized", function (data: any) {  // Existe vraiment, le canevas n'aura pas de taille non?
                const response = { data: data, isRequestSuccessul: false }; 
                socket.emit("temp", JSON.stringify(response)); 
            });

        });

    }
};
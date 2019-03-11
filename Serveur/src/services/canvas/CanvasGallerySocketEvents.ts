import * as SocketEvents from "../../constants/SocketEvents";
import CanvasManager from "./components/CanvasManager";
import { IEditGalleryData } from "./interfaces/interfaces";

export default class CanvasGallerySocketEvents {
    constructor(io: any, canvasManager: CanvasManager) {
        io.on('connection', function (socket: any) {
            console.log(socket.id + " connected to Canvas GallerySocketEvents");

            // Canvas Gallery 
            socket.on("getPublicCanvas", function (data: any) { 
                const response = { data: data, isRequestSuccessul: false }; 
              //  socket.emit("temp", JSON.stringify(response)); 
            });

            socket.on("getPrivateCanvas", function (data: any) { 
                const response = { data: data, isRequestSuccessul: false }; 
              //  socket.emit("temp", JSON.stringify(response)); 
            });

            socket.on("getCanvas", function (dataStr: string) { 
                try {
                    const data: IEditGalleryData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    socket.emit("getCanvasResponse", canvasManager.getCanvasRoomSERI(canvasRoomId));
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("getAllCanvas", function () { 
                try {
                    socket.emit("getAllCanvasResponse", canvasManager.getCanvasRoomsSERI());
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

        });

    }
};
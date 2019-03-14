import * as SocketEvents from "../../constants/SocketEvents";
import CanvasManager from "./components/CanvasManager";
import { IEditGalleryData } from "./interfaces/interfaces";

export default class CanvasGallerySocketEvents {
    constructor(io: any, canvasManager: CanvasManager) {
        io.on('connection', function (socket: any) {
            console.log(socket.id + " connected to Canvas GallerySocketEvents");

            socket.on("accessCanvas", function (dataStr: string) { 
                try {
                    const data: IEditGalleryData = JSON.parse(dataStr);

                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);
    
                    const response = {
                        isPasswordValid: canvasManager.accessCanvas(canvasRoomId, data),
                        canvasName: data.canevasName
                    };
    
                    socket.emit("accessCanvasResponse", JSON.stringify(response));    
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("getPublicCanvas", function () { 
                try {
                    socket.emit("getPublicCanvasResponse", canvasManager.getPublicCanvasSERI());
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("getPrivateCanvas", function (username: string) { 
                try {
                    socket.emit("getPrivateCanvasResponse", canvasManager.getPrivateCanvasSERI(username));
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("getCanvas", function (dataStr: string) { 
                try {
                    const data: IEditGalleryData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    socket.emit("getCanvasResponse", canvasManager.getCanvasSERI(canvasRoomId));
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
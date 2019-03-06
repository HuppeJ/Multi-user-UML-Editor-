import * as SocketEvents from "../../constants/SocketEvents";
import { CanvasTestRoom } from "./CanvasSocketEvents";
import CanvasManager from "./components/CanvasManager";
import { IEditFormData, IEditFormsData, IEditCanevasData } from "./interfaces/interfaces";

export default class CanvasEditionSocketEvents {
    constructor(io: any, canvasManager: CanvasManager) {
        io.on('connection', function (socket: any) {

            // Collaborative Basic Edition
            socket.on("createForm", function (data: IEditFormData) {
                const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                const response = {
                    isFormCreated: canvasManager.addFormToCanvas(canvasRoomId, data.form, socket.id)
                };

                if (response.isFormCreated) {
                    console.log(socket.id + " created form " + data.form);
                    io.to(canvasRoomId).emit("formCreated", data.form);
                } else {
                    console.log(socket.id + " failed to create form " + data.form);
                }

                socket.emit("createFormResponse", JSON.stringify(response));

                // TODO à enlever
                io.to(CanvasTestRoom).emit("formCreated", data);
            });

            socket.on("updateForms", function (data: IEditFormsData) {
                const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                const response = {
                    areFormsUpdated: canvasManager.updateCanvasForms(canvasRoomId, data.forms, socket.id)
                };

                if (response.areFormsUpdated) {
                    console.log(socket.id + " updated forms " + data.forms);
                    io.to(canvasRoomId).emit("formsUpdated", data.forms);
                } else {
                    console.log(socket.id + " failed to update forms " + data.forms);
                }

                socket.emit("updateFormsResponse", JSON.stringify(response));


                // TODO à enlever
                io.to(CanvasTestRoom).emit("formsUpdated", data);
            });

            socket.on("deleteForms", function (data: IEditFormsData) {
                const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                const response = {
                    areFormsDeleted: canvasManager.deleteCanvasForms(canvasRoomId, data.forms, socket.id)
                };

                if (response.areFormsDeleted) {
                    console.log(socket.id + " deleted forms " + data.forms);
                    io.to(canvasRoomId).emit("formsDeleted", data.forms);
                } else {
                    console.log(socket.id + " failed to delete forms " + data.forms);
                }

                socket.emit("deleteFormsResponse", JSON.stringify(response));

                io.to(CanvasTestRoom).emit("formsDeleted", data);
            });

            // TODO : Renvoyer des informations plus précises si nécessaire
            socket.on("selectForms", function (data: IEditFormsData) {
                const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                const response = {
                    areFormsSelected: canvasManager.selectCanvasForms(canvasRoomId, data.forms, socket.id)
                };

                if (response.areFormsSelected) {
                    console.log(socket.id + " selected forms " + data.forms);
                    io.to(canvasRoomId).emit("formsSelected", data);
                } else {
                    console.log(socket.id + " failed to select forms " + data.forms);
                }

                socket.emit("selectFormsResponse", JSON.stringify(response));
                
                // TODO remove
                io.to(CanvasTestRoom).emit("formsSelected", data);
            });

            socket.on("deselectForms", function (data: IEditFormsData) {
                const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                const response = {
                    areFormsDeselected: canvasManager.deselectCanvasForms(canvasRoomId, data)
                };

                if (response.areFormsDeselected) {
                    console.log(socket.id + " deselected forms " + data.forms);
                    io.to(canvasRoomId).emit("formsDeselected", data);
                } else {
                    console.log(socket.id + " failed to deselect forms " + data.forms);
                }

                socket.emit("deselectFormsResponse", JSON.stringify(response));
                
            });

            socket.on("reinitializeCanvas", function (data: IEditCanevasData) {
                const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevas.name);

                const response = {
                    isCanvasReinitialized: canvasManager.reinitializeCanvas(canvasRoomId)
                };

                if (response.isCanvasReinitialized) {
                    console.log(socket.id + " reinitialize canvas " + data.canevas.name);
                    // TODO Est-ce qu'on voudrait que le serveur renvoit un canevas de base (vide avec des dimessions prédéfinies)
                    io.to(canvasRoomId).emit("canvasReinitialized");
                } else {
                    console.log(socket.id + " failed to reinitialized canvas " + data.canevas.name);
                }

                socket.emit("reinitializeCanvasResponse", JSON.stringify(response));


                // TODO à enlever
                io.to(CanvasTestRoom).emit("canvasReinitialized", data);
            });

            socket.on("canvasResized", function (data: IEditCanevasData) {
                const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevas.name);

                const response = {
                    isCanvasResized: canvasManager.resizeCanvas(canvasRoomId, data.canevas, socket.id)
                };

                if (response.isCanvasResized) {
                    console.log(socket.id + " resized canvas " + data.canevas);
                    // TODO Est-ce qu'on voudrait que le serveur renvoit un canevas de base (vide avec des dimessions prédéfinies)
                    io.to(canvasRoomId).emit("canvasResized");
                } else {
                    console.log(socket.id + " failed to resize canvas " + data.canevas.name);
                }

                socket.emit("reinitializeCanvasResponse", JSON.stringify(response));
            });

        });

    }
};
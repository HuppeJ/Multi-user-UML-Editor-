import * as SocketEvents from "../../constants/SocketEvents";
import { CanvasTestRoom } from "./CanvasSocketEvents";
import CanvasManager from "./components/CanvasManager";
import { ICreateFormData, IUpdateFormsData } from "./interfaces/interfaces";

export default class CanvasEditionSocketEvents {
    constructor(io: any, canvasManager: CanvasManager) {
        io.on('connection', function (socket: any) {

            // Collaborative Basic Edition
            socket.on("createForm", function (createFormData: ICreateFormData) {
                const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(createFormData.canevasName);

                const response = {
                    isFormCreated: canvasManager.addFormToCanvas(canvasRoomId, createFormData.form, socket.id)
                };

                if (response.isFormCreated) {
                    console.log(socket.id + " created form " + createFormData.form);
                    io.to(canvasRoomId).emit("formCreated", createFormData.form);
                } else {
                    console.log(socket.id + " failed to create form " + createFormData.form);
                }

                socket.emit("createFormResponse", JSON.stringify(response));

                // TODO à enlever
                io.to(CanvasTestRoom).emit("formCreated", createFormData);
            });

            socket.on("updateForms", function (updateFormsData: IUpdateFormsData) {
                const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(updateFormsData.canevasName);

                const response = {
                    areFormsUpdated: canvasManager.updateCanvasForms(canvasRoomId, updateFormsData.forms, socket.id)
                };

                if (response.areFormsUpdated) {
                    console.log(socket.id + " updated forms " + updateFormsData.forms);
                    io.to(canvasRoomId).emit("formsUpdated", updateFormsData.forms);
                } else {
                    console.log(socket.id + " failed to update forms " + updateFormsData.forms);
                }

                socket.emit("updateFormsResponse", JSON.stringify(response));


                // TODO à enlever
                io.to(CanvasTestRoom).emit("formsUpdated", updateFormsData);
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

            socket.on("canvasResized", function (data: any) {  // Existe vraiment, le canevas n'aura pas de taille non?
                const response = { data: data, isRequestSuccessul: false };
                //  socket.emit("temp", JSON.stringify(response)); 
            });

        });

    }
};
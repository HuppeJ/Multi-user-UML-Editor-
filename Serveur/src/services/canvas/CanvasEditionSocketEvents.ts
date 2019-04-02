import * as SocketEvents from "../../constants/SocketEvents";
import { CanvasTestRoom } from "./CanvasSocketEvents";
import CanvasManager from "./components/CanvasManager";
import { IEditCanevasData, IUpdateFormsData, IUpdateLinksData, IEditGalleryData, IResizeCanevasData } from "./interfaces/interfaces";

export default class CanvasEditionSocketEvents {
    constructor(io: any, canvasManager: CanvasManager) {
        io.on('connection', function (socket: any) {

            /***********************************************
            * Events related to Forms
            ************************************************/
            socket.on("createForm", function (dataStr: string) {
                try {
                    const data: IUpdateFormsData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    const response = {
                        isFormCreated: canvasManager.addFormToCanvas(canvasRoomId, data)
                    };

                    if (response.isFormCreated) {
                        console.log(socket.id + " created form " + data.forms[0]);
                        io.to(canvasRoomId).emit("formCreated", JSON.stringify(data));
                    } else {
                        console.log(socket.id + " failed to create form " + data.forms[0]);
                    }

                    socket.emit("createFormResponse", JSON.stringify(response));

                    // TODO à enlever
                    io.to(CanvasTestRoom).emit("formCreated", dataStr);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("updateForms", function (dataStr: string) {
                try {
                    const data: IUpdateFormsData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    const response = {
                        areFormsUpdated: canvasManager.updateCanvasForms(canvasRoomId, data)
                    };

                    if (response.areFormsUpdated) {
                        console.log(socket.id + " updated forms " + data.forms);
                        io.to(canvasRoomId).emit("formsUpdated", dataStr);
                    } else {
                        console.log(socket.id + " failed to update forms " + data.forms);
                    }

                    socket.emit("updateFormsResponse", JSON.stringify(response));


                    // TODO remove
                    io.to(CanvasTestRoom).emit("formsUpdated", dataStr);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("deleteForms", function (dataStr: string) {
                try {
                    const data: IUpdateFormsData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    const response = {
                        areFormsDeleted: canvasManager.deleteCanvasForms(canvasRoomId, data)
                    };

                    if (response.areFormsDeleted) {
                        console.log(socket.id + " deleted forms " + data.forms);
                        io.to(canvasRoomId).emit("formsDeleted", dataStr);
                    } else {
                        console.log(socket.id + " failed to delete forms " + data.forms);
                    }

                    socket.emit("deleteFormsResponse", JSON.stringify(response));

                    // TODO remove
                    io.to(CanvasTestRoom).emit("formsDeleted", dataStr);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            // TODO : Renvoyer des informations plus précises si nécessaire
            socket.on("selectForms", function (dataStr: string) {
                try {
                    const data: IUpdateFormsData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    const response = {
                        areFormsSelected: canvasManager.selectCanvasForms(canvasRoomId, data)
                    };

                    if (response.areFormsSelected) {
                        console.log(socket.id + " selected forms " + data.forms);
                        io.to(canvasRoomId).emit("formsSelected", dataStr);
                    } else {
                        console.log(socket.id + " failed to select forms " + data.forms);
                    }

                    socket.emit("selectFormsResponse", JSON.stringify(response));

                    // TODO remove
                    io.to(CanvasTestRoom).emit("formsSelected", dataStr);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("deselectForms", function (dataStr: string) {
                try {
                    const data: IUpdateFormsData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    const response = {
                        areFormsDeselected: canvasManager.deselectCanvasForms(canvasRoomId, data)
                    };

                    if (response.areFormsDeselected) {
                        console.log(socket.id + " deselected forms " + data.forms);
                        io.to(canvasRoomId).emit("formsDeselected", dataStr);
                    } else {
                        console.log(socket.id + " failed to deselect forms " + data.forms);
                    }

                    socket.emit("deselectFormsResponse", JSON.stringify(response));

                    // TODO remove
                    io.to(CanvasTestRoom).emit("formsDeselected", dataStr);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("getSelectedForms", function (canvasName: string) {
                try {
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(canvasName);
                    const selectedForms: string = canvasManager.getSelectedFormsInCanvasRoomSERI(canvasRoomId);
                    console.log("getSelectedForms")
                    console.log(selectedForms.toString())

                    socket.emit("selectedForms", selectedForms);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            /***********************************************
            * Events related to links
            ************************************************/
            socket.on("createLink", function (dataStr: string) {
                try {
                    const data: IUpdateLinksData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    const response = {
                        isLinkCreated: canvasManager.addLinkToCanvas(canvasRoomId, data)
                    };

                    if (response.isLinkCreated) {
                        console.log(socket.id + " created link " + data.links[0]);
                        console.log(data.links[0].path);
                        io.to(canvasRoomId).emit("linkCreated", dataStr);
                    } else {
                        console.log(socket.id + " failed to create link " + data.links[0]);
                    }

                    socket.emit("createLinkResponse", JSON.stringify(response));

                    // TODO remove
                    io.to(CanvasTestRoom).emit("linkCreated", dataStr);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("updateLinks", function (dataStr: string) {
                try {
                    const data: IUpdateLinksData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    const response = {
                        areLinksUpdated: canvasManager.updateCanvasLinks(canvasRoomId, data)
                    };

                    if (response.areLinksUpdated) {
                        console.log(socket.id + " updated links " + data.links);
                        io.to(canvasRoomId).emit("linksUpdated", dataStr);
                    } else {
                        console.log(socket.id + " failed to update links " + data.links);
                    }

                    socket.emit("updateLinksResponse", JSON.stringify(response));

                    // TODO remove
                    io.to(CanvasTestRoom).emit("linksUpdated", dataStr);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("deleteLinks", function (dataStr: string) {
                try {
                    const data: IUpdateLinksData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    const response = {
                        areLinksDeleted: canvasManager.deleteCanvasLinks(canvasRoomId, data)
                    };

                    if (response.areLinksDeleted) {
                        console.log(socket.id + " deleted links " + data.links);
                        io.to(canvasRoomId).emit("linksDeleted", dataStr);
                    } else {
                        console.log(socket.id + " failed to delete links " + data.links);
                    }

                    socket.emit("deleteLinksResponse", JSON.stringify(response));

                    // TODO remove
                    io.to(CanvasTestRoom).emit("linksDeleted", dataStr);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("selectLinks", function (dataStr: string) {
                try {
                    const data: IUpdateLinksData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    const response = {
                        areLinksSelected: canvasManager.selectCanvasLinks(canvasRoomId, data)
                    };

                    if (response.areLinksSelected) {
                        console.log(socket.id + " selected links " + data.links);
                        io.to(canvasRoomId).emit("linksSelected", dataStr);
                    } else {
                        console.log(socket.id + " failed to select links " + data.links);
                    }

                    socket.emit("selectLinksResponse", JSON.stringify(response));

                    // TODO remove
                    io.to(CanvasTestRoom).emit("linksSelected", dataStr);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("deselectLinks", function (dataStr: string) {
                try {
                    const data: IUpdateLinksData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    const response = {
                        areLinksDeselected: canvasManager.deselectCanvasLinks(canvasRoomId, data)
                    };

                    if (response.areLinksDeselected) {
                        console.log(socket.id + " deselected links " + data.links);
                        io.to(canvasRoomId).emit("linksDeselected", dataStr);
                    } else {
                        console.log(socket.id + " failed to deselect links " + data.links);
                    }

                    socket.emit("deselectLinksResponse", JSON.stringify(response));

                    // TODO remove
                    io.to(CanvasTestRoom).emit("linksDeselected", dataStr);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("getSelectedLinks", function (canvasName: string) {
                try {
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(canvasName);
                    const selectedLinks: string = canvasManager.getSelectedLinksInCanvasRoomSERI(canvasRoomId);
                    console.log("getSelectedLinks")
                    console.log(selectedLinks.toString())

                    socket.emit("selectedLinks", selectedLinks);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });
            
            /***********************************************
            * Events related to the Canvas
            ************************************************/
            socket.on("reinitializeCanvas", function (dataStr: string) {
                try {
                    const data: IEditCanevasData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevas.name);

                    const response = {
                        isCanvasReinitialized: canvasManager.reinitializeCanvas(canvasRoomId, data)
                    };

                    if (response.isCanvasReinitialized) {
                        console.log(socket.id + " reinitialize canvas " + data.canevas.name);
                        // TODO Est-ce qu'on voudrait que le serveur renvoit un canevas de base (vide avec des dimessions prédéfinies)?
                        io.to(canvasRoomId).emit("canvasReinitialized");
                    } else {
                        console.log(socket.id + " failed to reinitialized canvas " + data.canevas.name);
                    }

                    socket.emit("reinitializeCanvasResponse", JSON.stringify(response));


                    // TODO à enlever
                    io.to(CanvasTestRoom).emit("canvasReinitialized", dataStr);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("resizeCanvas", function (dataStr: string) {
                try {
                    const data: IResizeCanevasData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    const response = {
                        isCanvasResized: canvasManager.resizeCanvas(canvasRoomId, data)
                    };

                    if (response.isCanvasResized) {
                        console.log(socket.id + " resized canvas " + data.dimensions);
                        // TODO Est-ce qu'on voudrait que le serveur renvoit un canevas de base (vide avec des dimessions prédéfinies)
                        io.to(canvasRoomId).emit("canvasResized", dataStr);
                    } else {
                        console.log(socket.id + " failed to resize canvas " + data.canevasName);
                    }

                    socket.emit("resizeCanvasResponse", JSON.stringify(response));

                    // TODO remove
                    io.to(CanvasTestRoom).emit("canvasResized", dataStr);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("selectCanvas", function (dataStr: string) {
                try {
                    const data: IEditGalleryData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    const response = {
                        isCanvasSelected: canvasManager.selectCanvas(canvasRoomId, data)
                    };

                    if (response.isCanvasSelected) {
                        console.log(socket.id + " selected canvas " + data.canevasName);
                        io.to(canvasRoomId).emit("canvasSelected", dataStr);
                    } else {
                        console.log(socket.id + " failed to select canvas " + data.canevasName);
                    }

                    socket.emit("selectCanvasResponse", JSON.stringify(response));

                    // TODO remove
                    io.to(CanvasTestRoom).emit("canvasSelected", dataStr);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            socket.on("deselectCanvas", function (dataStr: string) {
                try {
                    const data: IEditGalleryData = JSON.parse(dataStr);
                    const canvasRoomId: string = canvasManager.getCanvasRoomIdFromName(data.canevasName);

                    const response = {
                        isCanvasDeselected: canvasManager.deselectCanvas(canvasRoomId, data)
                    };

                    if (response.isCanvasDeselected) {
                        console.log(socket.id + " deselected canvas " + data.canevasName);
                        io.to(canvasRoomId).emit("canvasDeselected", dataStr);
                    } else {
                        console.log(socket.id + " failed to deselect canvas " + data.canevasName);
                    }

                    socket.emit("deselectCanvasResponse", JSON.stringify(response));

                    // TODO remove
                    io.to(CanvasTestRoom).emit("canvasDeselected", dataStr);
                } catch (e) {
                    console.log("[Error]: ", e);
                }
            });

            

        });

    }
};
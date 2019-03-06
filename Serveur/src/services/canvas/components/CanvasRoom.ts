import { ICanevas } from "../interfaces/interfaces";
import { mapToObj } from "../../../utils/mapToObj";

export default class CanvasRoom {
    public connectedUsers: any;  // connectedUsers is a Set : [key: socketId]

    constructor(public canvas: ICanevas) {
        this.connectedUsers = new Set();
    }

    public addUser(socketId: any) {
        this.connectedUsers.add(socketId);
    }

    public removeUser(socketId: any) {
        this.connectedUsers.delete(socketId);
    }

    public hasUser(socketId: any) {
        return this.connectedUsers.has(socketId);
    }

    public getConnectedUsersSERI(): string {
        return JSON.stringify({
            connectedUsers: Array.from(this.connectedUsers)
        });
    }

    // On va probablement utiliser les , socketId: any pour log l<historique des modification
    public addForm(form: any, socketId: any): boolean {
        try {
            this.canvas.shapes.push(form);
            return true;
        } catch (e) {
            console.log("[Error] in addForm", e);
            return false;
        }
    }

    public updateForms(forms: any[], socketId: any): boolean {
        try {
            let formIsUpdated: boolean = false;

            forms.forEach((form) => {
                formIsUpdated = false;
                this.canvas.shapes.forEach((shape, index) => {
                    if (shape.id === form.id) {
                        this.canvas.shapes[index] = form;
                        formIsUpdated = true;
                    }
                });

                if (!formIsUpdated) {
                    throw new Error(`There is no form with the id: "${form.id}" in the canvas : "${this.canvas.name}".`);
                }
            });

            return true;
        } catch (e) {
            console.log("[Error] in updateForms", e);
            return false;
        }
    }

    // Note : Il ne faut pas qu'il y ait de dupliqué dans les forms à delete
    public deleteForms(forms: any[], socketId: any): boolean {
        try {
            let formIsDeleted: boolean = false;

            forms.forEach((form) => {
                formIsDeleted = false;
                this.canvas.shapes = this.canvas.shapes.filter((shape) => {
                    if (shape.id === form.id) {
                        formIsDeleted = true;
                        return false;
                    }

                    return true;
                });

                if (!formIsDeleted) {
                    throw new Error(`There is no form with the id: "${form.id}" in the canvas : "${this.canvas.name}".`);
                }
            });

            return true;
        } catch (e) {
            console.log("[Error] in deleteForms", e);
            return false;
        }
    }

    // Note : Il ne faut pas qu'il y ait de dupliqué dans les forms à selectionner
    public selectForms(forms: any[], socketId: any): boolean {
        try {
            let formIsSelected: boolean = false;

            forms.forEach((form) => {
                formIsSelected = false;
                this.canvas.shapes.forEach((shape) => {
                    if (shape.id === form.id) {
                        formIsSelected = true;
                    }
                });

                if (!formIsSelected) {
                    throw new Error(`There is no form with the id: "${form.id}" in the canvas : "${this.canvas.name}".`);
                }
            });

            return true;
        } catch (e) {
            console.log("[Error] in selectForms", e);
            return false;
        }
    }

    public resize(canvas: ICanevas, socketId: any): boolean {
        try {
            this.canvas.dimensions = canvas.dimensions;
            return true;
        } catch (e) {
            console.log("[Error] in reinitialize", e);
            return false;
        }
    }

    public reinitialize(): boolean {
        try {
            this.canvas.shapes = [];
            this.canvas.links = [];
            return true;
        } catch (e) {
            console.log("[Error] in reinitialize", e);
            return false;
        }
    }

    // toJSON is automatically used by JSON.stringify
    toJSON() {
        return Object.assign({}, this, {
            // convert fields that need converting
            connectedUsers: Array.from(this.connectedUsers)
        });
    }

    // // fromJSON is used to convert an serialized version
    // // of the User to an instance of the class
    // static fromJSON(json: ICanvasRoom | string): User {
    //     if (typeof json === 'string') {
    //         // if it's a string, parse it first
    //         return JSON.parse(json, User.reviver);
    //     } else {
    //         // create an instance of the User class
    //         let user = Object.create(User.prototype);
    //         // copy all the fields from the json object
    //         return Object.assign(user, json, {
    //             // convert fields that need converting
    //             created: new Date(json.created),
    //             account: Account.fromJSON(json.account)
    //         });
    //     }
    // }
}

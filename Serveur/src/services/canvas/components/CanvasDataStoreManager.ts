import { ICanevas } from "../interfaces/interfaces";

export const CANVAS_TABLE: string  = "Canvas";


export default class CanvasDataStoreManager {
    private datastore: any = null;

    constructor(datastore: any) {
        this.datastore = datastore;
    }

    public async addCanvas(canvas: ICanevas) {
        const canvasData = {
            id: canvas.name,
            canvasStr: JSON.stringify(canvas)
        };

        return this.datastore.save({
            key: this.datastore.key(CANVAS_TABLE),
            data: canvasData
        });
    }


    public async getAllCanvases(): Promise<string> {
        const query = this.datastore
        .createQuery(CANVAS_TABLE)

        console.log(query)
        return "";
    }
}

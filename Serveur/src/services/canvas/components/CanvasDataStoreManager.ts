import { ICanevas, IHistoryData } from "../interfaces/interfaces";

export const CANVAS_TABLE: string  = "Canvas";


export default class CanvasDataStoreManager {
    private datastore: any = null;

    constructor(datastore: any) {
        this.datastore = datastore;
    }

    public async addCanvas(canvas: ICanevas, canvasHistory: IHistoryData[]) {
        const name: string = canvas.name;

        const canvasData = {
            canvasName: name,
            canvas: canvas,
            canvasHistory: canvasHistory
        };

        return this.datastore.save({
            key: this.datastore.key([CANVAS_TABLE, name]),
            data: canvasData,
            excludeFromIndexes: ['canvas.thumbnail', 'canvasHistory[].canevas.thumbnail']
        });
    }

    public async updateCanvas(canvas: ICanevas, canvasHistory: IHistoryData[]) {
        console.log("updateCanvas", canvas, canvasHistory)
        const query = this.datastore
            .createQuery(CANVAS_TABLE)
            .filter('canvasName', '=', canvas.name)
            .limit(1);

        const canvases = await this.datastore.runQuery(query);

        if (canvases[0][0] !== undefined) {
            const newCanvasEntity = {
                canvasName: canvas.name,
                canvas: canvas,
                canvasHistory: canvasHistory
            };

            this.datastore.upsert({
                key: this.datastore.key([CANVAS_TABLE, canvas.name]),
                data: newCanvasEntity,
                excludeFromIndexes: ['canvas.thumbnail', 'canvasHistory[].canevas.thumbnail'],
            });
        }
    }


    public async getAllCanvases(): Promise<string> {
        const query = this.datastore
        .createQuery(CANVAS_TABLE)

        console.log(query)
        return "";
    }
}

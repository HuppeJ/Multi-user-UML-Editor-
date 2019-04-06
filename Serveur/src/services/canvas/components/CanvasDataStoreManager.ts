import { ICanevas, IHistoryData, ICanvasDataStore } from "../interfaces/interfaces";

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


    public async getAllCanvases(): Promise<ICanvasDataStore[]> {
        console.log("getAllCanvases1")

        const query = this.datastore.createQuery(CANVAS_TABLE);
        const canvases = await this.datastore.runQuery(query);
        
        const parsedCanvases: ICanvasDataStore[] = [];
        console.log("getAllCanvases1_canvases[0]", canvases[0])
        console.log("getAllCanvases2_canvases[0][0]", canvases[0][0])
        console.log("getAllCanvases3_canvases[0][1]", canvases[0][1])
        console.log("getAllCanvases4_canvases[1]", canvases[1])

        canvases[0].forEach((data: any) => {
            parsedCanvases.push({
                canevas: data.canevas,
                canvasHistory: data.canvasHistory,  
                canvasName: data.canvasName,
            } as ICanvasDataStore);

            console.log("DATA :" , data.canevas)
        });
        console.log("getAllCanvases_canvases", canvases)
        console.log("getAllCanvases_parsedCanvases", parsedCanvases)

        return parsedCanvases;
    }
}

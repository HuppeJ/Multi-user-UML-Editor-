import DrawingElement from "./DrawingElement";


interface IDrawing {
    name: string;
    dimensions: number[];
    forms: DrawingElement[];
    isProtected: boolean;
    isPublic: boolean;
}

export default class Drawing {
    private name: string = "";
    private dimensions: number[] = [];
    private elements: DrawingElement[] = [];
    private isProtected: boolean = false;
    private isPublic: boolean = false;

    
    constructor(name: string) {
        this.name = name;
    }

    public initialiseDrawing(name: string, dimensions: number[], elements: DrawingElement[], isProtected: boolean, isPublic: boolean) {
        this.name = name;
        this.dimensions = dimensions;
        this.elements = elements;
        this.isProtected = isProtected;
        this.isPublic = isPublic;
    }

}

import Form from "./Form";


export interface ICanvas {
    name: string;
    dimensions: number[];
    forms: Form[];
    isProtected: boolean;
    isPublic: boolean;
}

const DEFAULT_CANVAS_PROPS: ICanvas = {
    name: "",
    dimensions: [],
    forms: [],
    isProtected: false,
    isPublic: false
};

export class Canvas {
    private name: string;
    private dimensions: number[]; // TODO existe vraiment ?
    private forms: Form[];
    private isProtected: boolean;
    private isPublic: boolean;

    // private props: ICanvas
    
    constructor(props: ICanvas = DEFAULT_CANVAS_PROPS) {
        this.name = props.name;
        this.dimensions = props.dimensions;
        this.forms = props.forms;
        this.isProtected = props.isProtected;
        this.isPublic = props.isPublic;
    }

}

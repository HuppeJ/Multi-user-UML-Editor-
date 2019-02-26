interface ICanvasForm {
    id: string;
    formType: number; // enum ElementType ? 
    coordinates: number[];
    texts: string[]; // ???
    borderColor: string;
    borderStyle: number;
    borderWeight: number;
}

export default class Form {
    private id: string;
    private formType: number;
    private coordinates: number[];
    private texts: string[]; 
    private borderColor: string;
    private borderStyle: number;
    private borderWeight: number;


    constructor(props?: ICanvasForm) {
        this.id = props.id ? props.id : "";
        this.formType = props.formType ? props.formType : 0;
        this.coordinates = props.coordinates ? props.coordinates : [];
        this.texts = props.texts ? props.texts : [];
        this.borderColor = props.borderColor ? props.borderColor : "";
        this.borderStyle = props.borderStyle ? props.borderStyle : 0;
        this.borderWeight = props.borderWeight ? props.borderWeight : 0;
    }

}

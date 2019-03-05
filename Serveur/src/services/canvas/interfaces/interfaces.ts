export interface IAnchorPoint {
     formId: string, 
     anchor: number // enum AnchorPoints
}

export interface ICreateFormData {
    // username: string, // ça serait plus simple d'avoir le username directement pour log les changements, mais je ne sais pas à quel point c'est compliqué que les clients nous l'envoient
    canevasName: string,  
    form: any, // IBasicShape || IClassShape
}

export interface IUpdateFormsData {
    // username: string, // ça serait plus simple d'avoir le username directement pour log les changements, mais je ne sais pas à quel point c'est compliqué que les clients nous l'envoient
    canevasName: string,  
    forms: any[], // IBasicShape || IClassShape
}

export interface IBasicShape  { 
    id: string,  
    type: number,  // {Enum_ShapeTypes}
    name: string,  
    shapeStyle: IShapeStyle,  
    linksTo: ILink[],
    linksFrom: ILink[],
} 

export interface ICanevas { 
    id: string,  
    name: string,  
    author: string,  
    owner: string,  
    accessibility: number,  // {Enum_AccessibilityTypes},  
    password?: string,  
    shapes: IBasicShape[],  
    links: ILink[]
} 

export interface IClassShape extends IBasicShape {
    attributes: string[],  
    methods: string[]
}

export interface ICoordinates  { 
    x: number,  
    y: number
}

export interface ILink { 
    id: string,  
    from: IAnchorPoint,  
    to: IAnchorPoint,  
    type: number,  
    style: ILinkStyle,  
    path: ICoordinates[]
}

export interface ILinkStyle { 
    color: string,  
    thickness: number,  
    type: number
}

export interface IMessage  { 
    text: string,  
    sender: string,  
    createdAt: Long
} 

export interface IRoom  { 
    name: string
} 

export interface IShapeStyle  { 
    coordinates: ICoordinates,  
    width: number,  
    height: number,  
    rotation: number,  
    borderColor: string,  
    borderStyle: number, // {Enum_BorderTypes}, 
    backgroundColor: string
}

export interface IUser  { username: string}


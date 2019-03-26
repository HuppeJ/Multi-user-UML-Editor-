/***********************************************
* Interfaces for query parameters
************************************************/
export interface IEditGalleryData {
    username: string,
    canevasName: string,  
    password?: string,
}

export interface IEditCanevasData {
    username: string,
    canevas: ICanevas,  
}

export interface IUpdateFormsData {
    username: string,
    canevasName: string,  
    forms: any[], // IBasicShape || IClassShape
}

export interface IUpdateLinksData {
    username: string,
    canevasName: string,  
    links: ILink[],
}

export interface IEditChatroomData {
    username: string,
    chatroomName: string,  
    // password: string, ?
}

// TODO: vérifier format
export interface IMessageData {
    username: string,
    chatroomName: string,
    createdAt: string,
    message: string,
}

/***********************************************
* Interfaces for objects
************************************************/

export interface IAnchorPoint {
    formId: string, 
    anchor: number, // enum AnchorPoints
    multiplicity: string
}

export interface IBasicShape  { 
    id: string,  
    type: number,  // {Enum_ShapeTypes}
    name: string,  
    shapeStyle: IShapeStyle,  
    linksTo: string[],
    linksFrom: string[],
} 

export interface ICanevas { 
    id: string,  
    name: string,  
    author: string,  
    owner: string,  
    accessibility: number,  // {Enum_AccessibilityTypes},  
    password?: string,  
    shapes: IBasicShape[],  
    links: ILink[],
    dimensions: number[],
    thumbnailLeger: string,
    thumbnailLourd: string
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
    name: string,
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


export interface IAnchorPoint {
     formId: string, 
     anchor: number // enum AnchorPoints
}

export interface IBasicShape  { 
    id: string,  
    type: number,  // {Enum_ShapeTypes}
    name: string,  
    shapeStyle: IShapeStyle,  
    links: string[]
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
    id: string, 
    type: number, // (Enum_ShapeTypes) 
    name: string, 
    shapeStyle: IShapeStyle, 
    links: string[],  
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


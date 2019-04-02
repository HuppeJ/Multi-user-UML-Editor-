package com.polypaint.polypaint.Model

import java.io.Serializable
import java.util.*

class Link(var id: String, var name: String, var from: AnchorPoint, var to: AnchorPoint, var type: Int, var style: LinkStyle, var path: ArrayList<Coordinates>) : Serializable, DrawingElement(id){


    fun copy(id: String = UUID.randomUUID().toString(), name: String= this.name, from: AnchorPoint = AnchorPoint(), to: AnchorPoint = AnchorPoint(), type:Int = this.type, style: LinkStyle=this.style.copy(), path: ArrayList<Coordinates> = this.path.clone() as ArrayList<Coordinates>) = Link(id,name, from, to,type,style,path)
}
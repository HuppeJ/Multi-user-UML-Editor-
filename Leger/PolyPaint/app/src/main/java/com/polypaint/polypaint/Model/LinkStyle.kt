package com.polypaint.polypaint.Model
import java.io.Serializable

class LinkStyle(var color: String, var thickness: Int, var type: Int): Serializable{

    fun copy(color: String=this.color,  thickness: Int=this.thickness,  type: Int=this.type) = LinkStyle(color, thickness, type)
}
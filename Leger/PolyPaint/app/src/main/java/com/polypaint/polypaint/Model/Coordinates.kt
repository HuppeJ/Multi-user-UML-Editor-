package com.polypaint.polypaint.Model
import java.io.Serializable

class Coordinates (var x: Double, var y: Double): Serializable{

    fun copy(x: Double = this.x, y: Double = this.y) = Coordinates(x,y)

    override fun toString():String{ return "{"+x+","+y+"}" }
}
package com.polypaint.polypaint.Model

class Coordinates (var x: Double, var y: Double){

    fun copy(x: Double = this.x, y: Double = this.y) = Coordinates(x,y)
}
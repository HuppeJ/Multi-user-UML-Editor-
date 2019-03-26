package com.polypaint.polypaint.Model
import java.io.Serializable


class ShapeStyle (var coordinates: Coordinates, var width: Double, var height: Double, var rotation: Double, var borderColor: String, var borderStyle: Int, var backgroundColor: String): Serializable{

    fun copy(coordinates: Coordinates = this.coordinates.copy(),
             width: Double = this.width,
             height: Double = this.height,
             rotation: Double = this.rotation,
             borderColor: String = this.borderColor,
             borderStyle: Int = this.borderStyle,
             backgroundColor: String = this.backgroundColor) = ShapeStyle(coordinates,width, height, rotation,borderColor,borderStyle,backgroundColor)
}
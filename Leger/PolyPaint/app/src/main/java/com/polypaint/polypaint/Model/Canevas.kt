package com.polypaint.polypaint.Model

import java.io.Serializable

class Canevas(var id: String, var name: String, var author: String, var owner: String, var accessibility: Int, var password: String, var shapes: ArrayList<BasicShape>, var links: ArrayList<Link>): Serializable{

    fun addShape(shape : BasicShape){
        shapes.add(shape)
    }

    fun addLink(link : Link){
        links.add(link)
    }

    fun findLink(id: String): Link?{
        for(link in links){
            if(link.id == id){
                return link
            }
        }
        return null
    }

    fun findShape(id: String): BasicShape?{
        for(shape in shapes){
            if(shape.id == id){
                return shape
            }
        }
        return null
    }

    fun updateShape(basicShape: BasicShape): Boolean{
        for( i in 0 ..shapes.size-1){
            if(shapes[i].id == basicShape.id){
                shapes[i] = basicShape
                return true
            }
        }
        return false
    }

    fun removeShape(id: String): Boolean {
        for(shape in shapes){
            if(shape.id == id){
                shapes.remove(shape)
                return true
            }
        }
        return false
    }


}
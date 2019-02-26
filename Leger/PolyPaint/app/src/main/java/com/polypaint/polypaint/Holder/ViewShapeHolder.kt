package com.polypaint.polypaint.Holder

import com.google.common.collect.BiMap
import com.google.common.collect.HashBiMap
import com.polypaint.polypaint.Model.BasicShape
import com.polypaint.polypaint.Model.Canevas
import com.polypaint.polypaint.Model.Link
import com.polypaint.polypaint.View.BasicElementView

class ViewShapeHolder(){
    var map: BiMap<BasicElementView, BasicShape> = HashBiMap.create()
    var canevas : Canevas = Canevas("default","default name","aa-author", "aa-owner", 2, null, ArrayList<BasicShape>(), ArrayList<Link>())
    companion object {
        private val viewShapeHolder: ViewShapeHolder = ViewShapeHolder()

        fun getInstance(): ViewShapeHolder{
            return viewShapeHolder
        }
    }

    fun remove(basicShape: BasicShape){
        canevas.shapes.remove(basicShape)
        map.inverse().remove(basicShape)
    }
    fun remove(basicElementView: BasicElementView){
        canevas.shapes.remove(map.getValue(basicElementView))
        map.remove(basicElementView)
    }
    fun isSync(): Boolean{
        return ViewShapeHolder.getInstance().map.keys.size == ViewShapeHolder.getInstance().canevas.shapes.size
    }
}
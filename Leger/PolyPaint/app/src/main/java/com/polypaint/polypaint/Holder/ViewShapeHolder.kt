package com.polypaint.polypaint.Holder

import com.google.common.collect.BiMap
import com.google.common.collect.HashBiMap
import com.polypaint.polypaint.Model.BasicShape
import com.polypaint.polypaint.Model.Canevas
import com.polypaint.polypaint.Model.Link
import com.polypaint.polypaint.View.BasicElementView
import com.polypaint.polypaint.View.LinkView

class ViewShapeHolder(){
    var map: BiMap<BasicElementView, String> = HashBiMap.create()
    var linkMap : BiMap<LinkView, String> = HashBiMap.create()
    var canevas : Canevas = Canevas("default","default name","aa-author", "aa-owner", 2, null, ArrayList<BasicShape>(), ArrayList<Link>())
    companion object {
        private val viewShapeHolder: ViewShapeHolder = ViewShapeHolder()

        fun getInstance(): ViewShapeHolder{
            return viewShapeHolder
        }
    }

    fun remove(basicShape: BasicShape){
        canevas.removeShape(basicShape.id)
        map.inverse().remove(basicShape.id)
    }
    fun remove(basicElementView: BasicElementView){
        canevas.shapes.remove(canevas.findShape(map.getValue(basicElementView)))
        map.remove(basicElementView)
    }
    fun remove(linkView: LinkView){
        canevas.links.remove(canevas.findLink(linkMap.getValue(linkView)))
        for(shape in canevas.shapes){
            for(link in shape.linksTo){
                if(link == canevas.findLink(linkMap.getValue(linkView))?.id){
                    shape.linksTo.remove(link)
                }
            }
        }
        linkMap.remove(linkView)
    }
    fun removeAll(){
        canevas.shapes.removeAll(canevas.shapes)
        canevas.links.removeAll(canevas.links)
        map.clear()
        linkMap.clear()
    }
    fun isSync(): Boolean{
        return ViewShapeHolder.getInstance().map.keys.size == ViewShapeHolder.getInstance().canevas.shapes.size
    }
}
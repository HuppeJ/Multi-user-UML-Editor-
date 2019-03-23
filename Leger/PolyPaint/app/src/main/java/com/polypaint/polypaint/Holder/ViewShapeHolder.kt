package com.polypaint.polypaint.Holder

import com.google.common.collect.BiMap
import com.google.common.collect.HashBiMap
import com.polypaint.polypaint.Model.BasicShape
import com.polypaint.polypaint.Model.Canevas
import com.polypaint.polypaint.Model.Coordinates
import com.polypaint.polypaint.Model.Link
import com.polypaint.polypaint.View.BasicElementView
import com.polypaint.polypaint.View.LinkView
import java.util.*

class ViewShapeHolder(){
    var map: BiMap<BasicElementView, String> = HashBiMap.create()
    var linkMap : BiMap<LinkView, String> = HashBiMap.create()
    var stackShapeCreatedId : Stack<String> = Stack<String>()
    var canevas : Canevas = Canevas("default","default name","aa-author", "aa-owner", 2, "", ArrayList<BasicShape>(), ArrayList<Link>(), "", Coordinates(1100.0,800.0))
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
    fun remove(link: Link){
        val fromShape: BasicShape? = canevas.findShape(link.from.formId)
        fromShape?.linksFrom?.remove(link.id)
        val toShape: BasicShape? = canevas.findShape(link.to.formId)
        toShape?.linksTo?.remove(link.id)
        canevas.links.remove(link)

        linkMap.inverse().remove(link.id)
    }
    fun remove(linkView: LinkView){
        val link: Link ?= canevas.findLink(linkMap.getValue(linkView))
        if(link != null){
            remove(link)
        }
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
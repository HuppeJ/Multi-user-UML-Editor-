package com.polypaint.polypaint.Model

import android.content.Context
import com.polypaint.polypaint.R
import android.view.LayoutInflater
import java.io.Serializable
import java.util.*
import kotlin.collections.ArrayList


open class BasicShape (var id: String = "", var type: Int = 0, var name: String = "", var shapeStyle: ShapeStyle , var linksTo: ArrayList<String?> = ArrayList(), var linksFrom: ArrayList<String?> = ArrayList()) : Serializable, DrawingElement(id) {
//    private var inflater : LayoutInflater? = null

    init {

        //        inflater = getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater

//        val mBasicElem = BasicElementView(this)

//        val viewToAdd = inflater!!.inflate(R.layout.basic_element, null)
//        mBasicElem.addView(viewToAdd,mBasicElem.childCount - 1)
    }
    fun copy(id: String = this.id, type: Int = this.type, name: String = this.name, shapeStyle: ShapeStyle = this.shapeStyle.copy(), linksTo: ArrayList<String?> = ArrayList(), linksFrom: ArrayList<String?> = ArrayList()) = BasicShape(id, type, name, shapeStyle, linksTo, linksFrom)

}
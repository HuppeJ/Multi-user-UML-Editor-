package com.polypaint.polypaint.Model

import android.content.Context
import com.polypaint.polypaint.R
import android.view.LayoutInflater


open class BasicShape (var id: String, var type: Int, var name: String, var shapeStyle: ShapeStyle, var links: Array<String?>) {
//    private var inflater : LayoutInflater? = null

    init {
//        inflater = getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater

//        val mBasicElem = BasicElement(this)

//        val viewToAdd = inflater!!.inflate(R.layout.basic_element, null)
//        mBasicElem.addView(viewToAdd,mBasicElem.childCount - 1)
    }

}
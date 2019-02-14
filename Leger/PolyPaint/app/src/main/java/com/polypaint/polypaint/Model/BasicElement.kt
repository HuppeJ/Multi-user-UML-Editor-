package com.polypaint.polypaint.Model

import android.content.Context
import android.view.View
import android.widget.RelativeLayout

class BasicElement(context: Context) : RelativeLayout(context) {

    var xRel : Float = 0F
    var yRel : Float = 0F
    var mId : Int = -1

    override fun onLayout(changed: Boolean, l: Int, t: Int, r: Int, b: Int) {
        //TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
        super.onLayout(changed, l, t, r, b)
    }

}
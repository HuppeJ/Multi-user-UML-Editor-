package com.polypaint.polypaint.Model

import android.content.Context
import android.view.MotionEvent
import android.view.View
import android.widget.RelativeLayout
import kotlinx.android.synthetic.main.activity_drawing.view.*
import kotlinx.android.synthetic.main.basic_element.view.*

class BasicElement: RelativeLayout, View.OnTouchListener {

    var xRel : Float = 0F
    var yRel : Float = 0F
    var mId : Int = -1

    constructor(context: Context) : super(context) {
        init(context)
    }

    fun init(context: Context) {
        setOnTouchListener(this)
//        dragHandle = this.findViewById(R.id.drag_handle)
        //dragHandle = this.findViewById(R.id.resizableLayout)

    }
    override fun onTouch(view: View, motionEvent: MotionEvent): Boolean {
        first_line.text = "aa";

        // view.x = motionEvent.rawX - view.width/2// - parent_relative_layout!!.x
        // view.y = motionEvent.rawY - view.height// - parent_relative_layout!!.y
        //Set soft focus

//        view.requestFocus()

//
//        val action = motionEvent.action
//        if (action == MotionEvent.ACTION_DOWN) {
//            // detect if tap is on handles
//            downRawY = motionEvent.rawY
//            height = this.measuredHeight.toFloat()
//        } else {
//            // change layout margin inside switch
//            when (side) {
//
//            }
//            val parent = this.parent as View
//            if (downRawY < parent.height - height + 2 * dragHandle!!.height) {
//                val rawY =
//                    if (motionEvent.rawY > 20 * dragHandle!!.height) motionEvent.rawY else 20 * dragHandle!!.height
//                val p = this.layoutParams as ViewGroup.MarginLayoutParams
////                p.topMargin = rawY.toInt()
//                p.topMargin = 100
//                if (p.topMargin != 0)
//                //this.top_margine = p.topMargin
//                    this.layoutParams = p
//            }
//        }
        return false
    }

    override fun onInterceptTouchEvent(motionEvent: MotionEvent): Boolean {
        onTouch(this, motionEvent)
        return false
    }

}
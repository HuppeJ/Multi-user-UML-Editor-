package com.polypaint.polypaint.Model

import android.content.Context
import android.view.MotionEvent
import android.view.View
import android.widget.ImageButton
import android.widget.LinearLayout
import android.widget.RelativeLayout
import com.polypaint.polypaint.R
import kotlinx.android.synthetic.main.activity_drawing.*
import kotlinx.android.synthetic.main.activity_drawing.view.*
import kotlinx.android.synthetic.main.basic_element.view.*

class BasicElement: RelativeLayout, View.OnTouchListener {

    var isElementSelected : Boolean = false

    constructor(context: Context) : super(context) {
        init(context)
    }

    fun init(context: Context) {
        setOnTouchListener(this)
    }

    override fun onTouch(view: View, motionEvent: MotionEvent): Boolean {
        first_line.text = "onTouch"


        editButton.setOnTouchListener(onTouchListenerEditButton)

        deleteButton.setOnTouchListener(onTouchListenerDeleteButton)

        resizeButton.setOnTouchListener(onTouchListenerResizeButton)


        val borderResizableLayout : LinearLayout = findViewById(R.id.borderResizableLayout)
        borderResizableLayout.setBackgroundResource(R.drawable.borders)

        return false
    }

    private var onTouchListenerEditButton = View.OnTouchListener { v, event ->
        val txt = first_line.text
        first_line.text = txt.toString() + "onTouchListenerEditButton"

        v.requestFocus()
        true
    }

    private var onTouchListenerDeleteButton = View.OnTouchListener { v, event ->
        val txt = first_line.text
        first_line.text = txt.toString() + "onTouchListenerDeleteButton"

        v.requestFocus()
        true
    }

    private var onTouchListenerResizeButton = View.OnTouchListener { v, event ->
        val txt = first_line.text
        first_line.text = txt.toString() + "onTouchListenerResizeButton"


        var dx = event.rawX - this.x

        var temp = outerResizableLayout.width

//        outerResizableLayout.layoutParams().height = temp + dx

        // val parentRelativeLayout : RelativeLayout = findViewById(R.id.parent_relative_layout)
        // v.x = event.rawX - v.width - parentRelativeLayout!!.x
        // v.y = event.rawY - v.height - parentRelativeLayout!!.y
        //Set soft focus

        v.requestFocus()
        true
    }


    override fun onInterceptTouchEvent(motionEvent: MotionEvent): Boolean {
        onTouch(this, motionEvent)
        return false
    }

}
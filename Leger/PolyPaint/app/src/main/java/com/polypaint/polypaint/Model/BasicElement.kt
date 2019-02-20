package com.polypaint.polypaint.Model

import androidx.appcompat.app.AppCompatActivity
import android.content.Context
import android.os.Bundle
import android.util.Log
import android.view.MotionEvent
import android.view.View
import android.widget.LinearLayout
import android.widget.RelativeLayout
import androidx.fragment.app.DialogFragment
import com.polypaint.polypaint.Fragment.EditClassDialogFragment
import com.polypaint.polypaint.R
import kotlinx.android.synthetic.main.basic_element.view.*
import java.util.*


class BasicElement: RelativeLayout {

    var isElementSelected : Boolean = false

    var oldFrameRawX : Float = 0.0F
    var oldFrameRawY : Float = 0.0F

    constructor(context: Context) : super(context) {
        init(context)
    }

    fun init(context: Context) {
        //setOnTouchListener(onTouchListenerBody)
    }

    override fun onAttachedToWindow() {
        super.onAttachedToWindow()
        //Set all listeners
        setOnTouchListener(onTouchListenerBody)
        editButton.setOnTouchListener(onTouchListenerEditButton)
        deleteButton.setOnTouchListener(onTouchListenerDeleteButton)
        resizeButton.setOnTouchListener(onTouchListenerResizeButton)

        //DEFAULT STATE

        var parent = this.parent as RelativeLayout
        parent.dispatchSetSelected(false)
        isSelected = true
    }

    override fun setSelected(selected: Boolean) {
        if(selected){
            first_line.text = "Focus"
            borderResizableLayout.setBackgroundResource(R.drawable.borders)
            editButton.visibility = View.VISIBLE
            deleteButton.visibility = View.VISIBLE
            resizeButton.visibility = View.VISIBLE
            anchorPoint0.visibility = View.VISIBLE
            anchorPoint1.visibility = View.VISIBLE
            anchorPoint2.visibility = View.VISIBLE
            anchorPoint3.visibility = View.VISIBLE
        }else{
            first_line.text = "NoFocus"
            borderResizableLayout.setBackgroundResource(R.drawable.borders_white)
            editButton.visibility = View.INVISIBLE
            deleteButton.visibility = View.INVISIBLE
            resizeButton.visibility = View.INVISIBLE
            anchorPoint0.visibility = View.INVISIBLE
            anchorPoint1.visibility = View.INVISIBLE
            anchorPoint2.visibility = View.INVISIBLE
            anchorPoint3.visibility = View.INVISIBLE
        }
        return super.setSelected(selected)
    }

    private var onTouchListenerBody = View.OnTouchListener { v, event ->
        when(event.action){
            MotionEvent.ACTION_DOWN -> {first_line.text = "ActionDown"
                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY

                val parentView = v.parent as RelativeLayout
                parentView.dispatchSetSelected(false)
                v.isSelected = true
            }
            MotionEvent.ACTION_MOVE -> {first_line.text = "ActionMove"
                this.x = this.x + (event.rawX - oldFrameRawX )
                this.y = this.y + (event.rawY - oldFrameRawY)
                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY
            }
            MotionEvent.ACTION_UP -> {first_line.text = "ActionUp"
                //isElementSelected = false
            }
        }
        true
    }

    private var onTouchListenerEditButton = View.OnTouchListener { v, event ->
        when(event.action){
            MotionEvent.ACTION_DOWN -> {

                first_line.text = "onTouchListenerEditButton"
                var activity: AppCompatActivity = context as AppCompatActivity

                var dialog: DialogFragment = EditClassDialogFragment()
                var bundle: Bundle = Bundle()
                bundle.putString("id", "asdfasg")
                dialog.arguments = bundle

                Log.d("****", dialog.arguments.toString())
                dialog.show(activity.supportFragmentManager, "alllooooo")
            }
        }
        true
    }

    private var onTouchListenerDeleteButton = View.OnTouchListener { v, event ->
        when(event.action){
            MotionEvent.ACTION_DOWN -> {
                first_line.text = "onTouchListenerDeleteButton"
                val parentView = v.parent.parent.parent as RelativeLayout
                parentView.removeView(this)
            }
        }
        true
    }

    private var onTouchListenerResizeButton = View.OnTouchListener { v, event ->
        val txt = first_line.text
        first_line.text = txt.toString() + "onTouchListenerResizeButton"

        when(event.action){
            MotionEvent.ACTION_DOWN -> {first_line.text = "ActionDownResize"
                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY
            }
            MotionEvent.ACTION_MOVE -> {first_line.text = "ActionMoveResize"
                borderResizableLayout.layoutParams.width = (borderResizableLayout.width + (event.rawX - oldFrameRawX)).toInt()
                borderResizableLayout.layoutParams.height = (borderResizableLayout.height + (event.rawY - oldFrameRawY)).toInt()
                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY
            }
        }
        true
    }

}
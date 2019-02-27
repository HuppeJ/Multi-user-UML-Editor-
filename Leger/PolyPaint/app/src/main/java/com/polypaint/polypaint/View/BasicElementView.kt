package com.polypaint.polypaint.View

import androidx.appcompat.app.AppCompatActivity
import android.content.Context
import android.os.Bundle
import android.util.Log
import android.view.MotionEvent
import android.view.View
import android.widget.RelativeLayout
import androidx.fragment.app.DialogFragment
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson
import com.polypaint.polypaint.Activity.DrawingActivity
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Fragment.EditClassDialogFragment
import com.polypaint.polypaint.Holder.UserHolder
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.BasicShape
import com.polypaint.polypaint.R
import com.polypaint.polypaint.Socket.SocketConstants
import kotlinx.android.synthetic.main.basic_element.view.*


open class BasicElementView: RelativeLayout {

    var oldFrameRawX : Float = 0.0F
    var oldFrameRawY : Float = 0.0F
    open var mMinimumWidth : Float = 300F
    open var mMinimumHeight : Float = 100F


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
            //first_line.text = "Focus"
            borderResizableLayout.setBackgroundResource(R.drawable.borders)
            editButton.visibility = View.VISIBLE
            deleteButton.visibility = View.VISIBLE
            resizeButton.visibility = View.VISIBLE
            anchorPoint0.visibility = View.VISIBLE
            anchorPoint1.visibility = View.VISIBLE
            anchorPoint2.visibility = View.VISIBLE
            anchorPoint3.visibility = View.VISIBLE
        }else{
            //first_line.text = "NoFocus"
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
            MotionEvent.ACTION_DOWN -> {//first_line.text = "ActionDown"
                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY

                val parentView = v.parent as RelativeLayout
                parentView.dispatchSetSelected(false)
                v.isSelected = true
            }
            MotionEvent.ACTION_MOVE -> {//first_line.text = "ActionMove"
                this.x = this.x + (event.rawX - oldFrameRawX )
                this.y = this.y + (event.rawY - oldFrameRawY)
                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY
            }
            MotionEvent.ACTION_UP -> { first_line.text = "ActionUp"
                val activity: AppCompatActivity = context as AppCompatActivity
                if(activity is DrawingActivity){
                    val drawingActivity : DrawingActivity = activity as DrawingActivity
                    drawingActivity.syncCanevasFromLayout()
                }
            }
        }
        true
    }

    private var onTouchListenerEditButton = View.OnTouchListener { v, event ->
        when(event.action){
            MotionEvent.ACTION_DOWN -> {//first_line.text = "onTouchListenerEditButton"
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
            MotionEvent.ACTION_DOWN -> {//first_line.text = "onTouchListenerDeleteButton"
                val parentView = v.parent.parent.parent as RelativeLayout
                parentView.removeView(this)

                ViewShapeHolder.getInstance().remove(this)
            }
        }
        true
    }

    open protected var onTouchListenerResizeButton = View.OnTouchListener { v, event ->
        //val txt = first_line.text
        //first_line.text = txt.toString() + "onTouchListenerResizeButton"

        when(event.action){
            MotionEvent.ACTION_DOWN -> {first_line.text = "ActionDownResize"
                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY
            }
            MotionEvent.ACTION_MOVE -> {
                first_line.text = "ActionMoveResize"
                val newWidth = borderResizableLayout.width + (event.rawX - oldFrameRawX)
                val newHeight = borderResizableLayout.height + (event.rawY - oldFrameRawY)

                resize(newWidth.toInt(), newHeight.toInt())

                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY
            }
            MotionEvent.ACTION_UP -> {
                first_line.text = "ActionUpResize"
                val activity: AppCompatActivity = context as AppCompatActivity
                if(activity is DrawingActivity){
                    val drawingActivity : DrawingActivity = activity as DrawingActivity
                    drawingActivity.syncCanevasFromLayout()
                }
                emitUpdate()
            }
        }
        true
    }

    open fun resize(newWidth:Int, newHeight:Int){
        if(newWidth >= mMinimumWidth){
            borderResizableLayout.layoutParams.width = newWidth
        }
        if(newHeight >= mMinimumHeight){
            borderResizableLayout.layoutParams.height = newHeight
        }
        borderResizableLayout.requestLayout()
        requestLayout()
    }

    private fun emitUpdate(){
        val activity: AppCompatActivity = context as AppCompatActivity
        val app: PolyPaint = activity.application as PolyPaint
        val socket: Socket? = app.socket
        val basicShape: BasicShape? = ViewShapeHolder.getInstance().canevas.findShape(ViewShapeHolder.getInstance().map.getValue(this))

        if(basicShape !=null) {
            val gson = Gson()
            val response: DrawingActivity.Response =DrawingActivity.Response(UserHolder.getInstance().username, basicShape)
            val obj: String = gson.toJson(response)
            Log.d("emitingUpdate", obj)
            socket?.emit(SocketConstants.UPDATE_FORMS, obj)
        }
    }

}
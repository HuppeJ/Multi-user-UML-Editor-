package com.polypaint.polypaint.View

import androidx.appcompat.app.AppCompatActivity
import android.content.Context
import android.graphics.Rect
import android.os.Bundle
import android.util.DisplayMetrics
import android.util.Log
import android.view.MotionEvent
import android.view.View
import android.widget.ImageButton
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
import com.polypaint.polypaint.Model.Coordinates
import com.polypaint.polypaint.R
import com.polypaint.polypaint.Socket.SocketConstants
import kotlinx.android.synthetic.main.basic_element.view.*


open class BasicElementView: RelativeLayout {

    var oldFrameRawX : Float = 0.0F
    var oldFrameRawY : Float = 0.0F
    var start: Coordinates = Coordinates(0.0,0.0)
    var oldLink: LinkView? = null
    var isSelectedByOther: Boolean = false
    open var mMinimumWidth : Float = 300F
    open var mMinimumHeight : Float = 100F
    var socket: Socket? = null


    constructor(context: Context) : super(context) {
        init(context)
        val activity: AppCompatActivity = context as AppCompatActivity
        val app: PolyPaint = activity.application as PolyPaint
        this.socket = app.socket
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
        anchorPoint0.setOnTouchListener(onTouchListenerAnchor)
        anchorPoint1.setOnTouchListener(onTouchListenerAnchor)
        anchorPoint2.setOnTouchListener(onTouchListenerAnchor)
        anchorPoint3.setOnTouchListener(onTouchListenerAnchor)


        //DEFAULT STATE

        var parent = this.parent as RelativeLayout
        parent.dispatchSetSelected(false)
        isSelected = true
    }

    override fun setSelected(selected: Boolean) {
        if(selected){
            //first_line.text = "Focus"
            borderResizableLayout.setBackgroundResource(R.drawable.borders_blue)
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
//            anchorPoint0.visibility = View.INVISIBLE
//            anchorPoint1.visibility = View.INVISIBLE
//            anchorPoint2.visibility = View.INVISIBLE
//            anchorPoint3.visibility = View.INVISIBLE
        }
        return super.setSelected(selected)
    }

    private var onTouchListenerAnchor = View.OnTouchListener { v, event ->
        if(isSelected) {
            val parentView = v.parent.parent.parent as RelativeLayout
            parentView.removeView(oldLink)
            val link = LinkView(context)

            when (event.action) {
                MotionEvent.ACTION_DOWN -> {//first_line.text = "ActionDown"
                    val coord = IntArray(2)
                    v.getLocationOnScreen(coord)
                    val activity = context as AppCompatActivity
                    val toolbarView: View= activity.findViewById(R.id.toolbar)

                    start = Coordinates((coord[0]-parentView.x + v.measuredWidth/2).toDouble(), (coord[1]-toolbarView.measuredHeight - v.measuredWidth/2).toDouble())
                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY
                }
                MotionEvent.ACTION_MOVE -> {//first_line.text = "ActionMove"
                    link.start = start
                    link.end = Coordinates((start.x + (event.rawX-oldFrameRawX)), (start.y + (event.rawY-oldFrameRawY)))
                    oldLink = link
                    parentView.addView(link)
                }
                MotionEvent.ACTION_UP -> {
                    var otherAnchor : View? = null
                    val x = (start.x + (event.rawX-oldFrameRawX)).toInt()
                    val y = (start.y + (event.rawY-oldFrameRawY)).toInt()
                    for(basicView: BasicElementView in ViewShapeHolder.getInstance().map.keys){
                        if(basicView != this && !basicView.isSelectedByOther) {
                            if (isViewInBounds(basicView.anchorPoint0, x, y)) {
                                otherAnchor = basicView.anchorPoint0
                                break
                            } else if (isViewInBounds(basicView.anchorPoint1, x, y)) {
                                otherAnchor = basicView.anchorPoint1
                                break
                            } else if (isViewInBounds(basicView.anchorPoint2, x, y)) {
                                otherAnchor = basicView.anchorPoint2
                                break
                            } else if (isViewInBounds(basicView.anchorPoint3, x, y)) {
                                otherAnchor = basicView.anchorPoint3
                                break
                            }
                        }
                    }
                    if(otherAnchor != null){
                        link.start = start
                        val coord = IntArray(2)
                        otherAnchor.getLocationOnScreen(coord)
                        val activity = context as AppCompatActivity
                        val toolbarView: View= activity.findViewById(R.id.toolbar)
                        link.end = Coordinates((coord[0]-parentView.x + v.measuredWidth/2).toDouble(), (coord[1]-toolbarView.measuredHeight - v.measuredWidth/2).toDouble())
                        parentView.addView(link)
                    }
                    oldLink = null
                }
            }



        }
        true
    }



    private fun isViewInBounds(view: View, x: Int, y: Int): Boolean{
        val activity = context as AppCompatActivity
        val toolbarView: View= activity.findViewById(R.id.toolbar)
        val parentView = view.parent.parent.parent as RelativeLayout
        val outRect = Rect();
        val location = IntArray(2);
        view.getDrawingRect(outRect);
        view.getLocationOnScreen(location);
        outRect.offset((location[0] - parentView.x).toInt(), location[1] - toolbarView.measuredHeight - view.measuredWidth/2);
        return outRect.contains(x, y);
    }

   /* private fun getPositionY(view : View): Int{
        val activity = context as AppCompatActivity
        val globalView: View = activity.findViewById(android.R.id.content)
        val dm: DisplayMetrics = DisplayMetrics()
        activity.windowManager.defaultDisplay.getMetrics(dm)
        var topOffset = dm.heightPixels - globalView.measuredHeight
        val v: View= activity.findViewById(R.id.toolbar)
        topOffset += v.measuredHeight
        val coord = IntArray(2)
        view.getLocationOnScreen(coord)
        return coord[1] - topOffset
    }
    private fun getRelativePosition(view: View): Coordinates{
        return Coordinates(getRelativeLeft(view).toDouble(), getRelativeTop(view).toDouble())
    }

    private fun getRelativeTop(myView: View): Int {
        return if (myView.parent === myView.rootView)
            myView.top
        else
            myView.top + getRelativeTop(myView.parent as View)
    }

    private fun getRelativeLeft(myView: View): Int {
        return if (myView.parent === myView.rootView)
            myView.left
        else
            myView.left + getRelativeLeft(myView.parent as View)
    }*/

    private var onTouchListenerBody = View.OnTouchListener { v, event ->
        if(!isSelectedByOther) {
            when (event.action) {
                MotionEvent.ACTION_DOWN -> {//first_line.text = "ActionDown"
                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY

                    val parentView = v.parent as RelativeLayout
                    parentView.dispatchSetSelected(false)
                    v.isSelected = true
                    emitSelection()
                }
                MotionEvent.ACTION_MOVE -> {//first_line.text = "ActionMove"
                    this.x = this.x + (event.rawX - oldFrameRawX)
                    this.y = this.y + (event.rawY - oldFrameRawY)
                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY
                }
                MotionEvent.ACTION_UP -> {
                    first_line.text = "ActionUp"
                    val activity: AppCompatActivity = context as AppCompatActivity
                    if (activity is DrawingActivity) {
                        val drawingActivity: DrawingActivity = activity as DrawingActivity
                        drawingActivity.syncCanevasFromLayout()
                    }
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
                emitDelete()
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
        val response: String = this.createResponseObject()

        if(response !="") {
            Log.d("emitingUpdate", response)
            socket?.emit(SocketConstants.UPDATE_FORMS, response)
        }
    }

    private fun emitSelection(){
        val response: String = this.createResponseObject()

        if(response !="") {
            Log.d("emitingSelection", response)
            socket?.emit(SocketConstants.SELECT_FORMS, response)
        }
    }

    private fun emitDelete(){
        val response: String = this.createResponseObject()

        if(response !="") {
            Log.d("emitingDelete", response)
            socket?.emit(SocketConstants.DELETE_FORMS, response)
        }
    }

    private fun createResponseObject(): String{
        val basicShape: BasicShape? = ViewShapeHolder.getInstance().canevas.findShape(ViewShapeHolder.getInstance().map.getValue(this))

        var obj: String =""
        if(basicShape !=null) {
            val gson = Gson()
            val response: DrawingActivity.Response =DrawingActivity.Response(UserHolder.getInstance().username, basicShape)
            obj = gson.toJson(response)
        }
        return obj
    }

}
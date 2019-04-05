package com.polypaint.polypaint.View

import androidx.appcompat.app.AppCompatActivity
import android.content.Context
import android.graphics.Color
import android.graphics.Rect
import android.os.Bundle
import android.util.Log
import android.view.MotionEvent
import android.view.View
import android.widget.RelativeLayout
import androidx.constraintlayout.widget.ConstraintLayout
import androidx.fragment.app.DialogFragment
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson
import com.polypaint.polypaint.Activity.DrawingActivity
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Enum.AnchorPoints
import com.polypaint.polypaint.Fragment.EditBasicElementDialogFragment
import com.polypaint.polypaint.Holder.*
import com.polypaint.polypaint.Model.*
import com.polypaint.polypaint.R
import com.polypaint.polypaint.Socket.SocketConstants
import com.polypaint.polypaint.SocketReceptionModel.FormsUpdateEvent
import com.polypaint.polypaint.SocketReceptionModel.LinksUpdateEvent
import kotlinx.android.synthetic.main.basic_element.view.*
import java.util.*
import kotlin.collections.ArrayList


open class BasicElementView: ConstraintLayout {

    var oldFrameRawX : Float = 0.0F
    var oldFrameRawY : Float = 0.0F
    var start: Coordinates = Coordinates(0.0,0.0)
    var oldLink: LinkView? = null
    var isSelectedByOther: Boolean = false
    open var mMinimumWidth : Float = 84F
    open var mMinimumHeight : Float = 84F
    var socket: Socket? = null

    var pointerFinger1 : Int = -1
    var pointerFinger2 : Int = -1

    var fingersCoords : Array<Coordinates> = Array(4) { Coordinates(0.0,0.0) }

    var leftX = this.pivotX
    var topY = this.pivotY

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
        isSelected = false
    }

    private fun deselecteAllConnectedLinks(){
        for(id in FormsSelectionHolder.getInstance().linksSelectedId){
            ViewShapeHolder.getInstance().linkMap.inverse()[id]?.emitDeselection()
        }
        FormsSelectionHolder.getInstance().linksSelectedId.clear()
    }

    override fun setSelected(selected: Boolean) {
        if(this.isSelected && !selected){
            emitDeselection()
            deselecteAllConnectedLinks()
        }
        var realSelected = selected
        if(isSelectedByOther){
            realSelected = false
        }
        if(realSelected){
            //first_line.text = "Focus"
            borderResizableLayout.setBackgroundResource(R.drawable.borders_blue)
            editButton.visibility = View.VISIBLE
            deleteButton.visibility = View.VISIBLE
            resizeButton.visibility = View.VISIBLE
            setAnchorsVisible(true)
            emitSelection()

            val basicShapeId: String? = ViewShapeHolder.getInstance().map[this]
            if(basicShapeId != null) {
                val basicShape: BasicShape? = ViewShapeHolder.getInstance().canevas.findShape(basicShapeId)
                if(basicShape != null) {
                    for (link in basicShape.linksFrom){
                        if(link != null && link != "") {
                            FormsSelectionHolder.getInstance().linksSelectedId.add(link)
                        }
                    }
                    for (link in basicShape.linksTo){
                        if(link != null && link != "") {
                            FormsSelectionHolder.getInstance().linksSelectedId.add(link)
                        }
                    }
                    for(id in FormsSelectionHolder.getInstance().linksSelectedId){
                        ViewShapeHolder.getInstance().linkMap.inverse()[id]?.emitSelection()
                    }
                }
            }

        }else {
            //first_line.text = "NoFocus"
            if(!this.isSelectedByOther) {
                borderResizableLayout.setBackgroundResource(R.drawable.borders_white)
            }
            editButton.visibility = View.INVISIBLE
            deleteButton.visibility = View.INVISIBLE
            resizeButton.visibility = View.INVISIBLE
            setAnchorsVisible(false)
        }
        return super.setSelected(realSelected)
    }

    fun hideButtonsAndAnchors(){
        editButton.visibility = View.INVISIBLE
        deleteButton.visibility = View.INVISIBLE
        resizeButton.visibility = View.INVISIBLE
        setAnchorsVisible(false)
    }

    fun setIsSelectedByOther(isSelectedByOther: Boolean){
        this.isSelectedByOther = isSelectedByOther
        if(isSelectedByOther){
            borderResizableLayout.setBackgroundResource(R.drawable.borders_red)
        } else {
            borderResizableLayout.setBackgroundResource(R.drawable.borders_white)
        }
    }

    fun setAnchorsVisible(isVisible: Boolean){
        if(isVisible){
            anchorPoint0.visibility = View.VISIBLE
            anchorPoint1.visibility = View.VISIBLE
            anchorPoint2.visibility = View.VISIBLE
            anchorPoint3.visibility = View.VISIBLE
        } else {
            anchorPoint0.visibility = View.INVISIBLE
            anchorPoint1.visibility = View.INVISIBLE
            anchorPoint2.visibility = View.INVISIBLE
            anchorPoint3.visibility = View.INVISIBLE
        }
    }
    /*private fun getRelativePosition(parent: ViewGroup, view: View): IntArray{
        var relativePosition: IntArray = IntArray(2)
        relativePosition[0] = view.left
        relativePosition[1] = view.top
        var currentParent: ViewGroup = view.parent as ViewGroup
        while(currentParent != parent){
            relativePosition[0] += currentParent.left
            relativePosition[1] += currentParent.top
            currentParent = currentParent.parent as ViewGroup
        }
        return relativePosition
    }*/

    private var onTouchListenerAnchor = View.OnTouchListener { v, event ->
        if(isSelected) {
            val parentView = v.parent.parent.parent as RelativeLayout
            parentView.removeView(oldLink)
            val link = LinkView(context)

            when (event.action) {
                MotionEvent.ACTION_DOWN -> {//first_line.text = "ActionDown"
                    for(basicView: BasicElementView in ViewShapeHolder.getInstance().map.keys) {
                        if (basicView != this && !basicView.isSelectedByOther) {
                            basicView.setAnchorsVisible(true)
                        }
                    }
                    val coord = IntArray(2)
                    v.getLocationOnScreen(coord)
                    val activity = context as AppCompatActivity
                    val toolbarView: View= activity.findViewById(R.id.toolbar)


                    var point: Coordinates = Coordinates(0.0,0.0)
                    val centerX = (leftX + this.measuredWidth / 2.0)
                    val centerY = (topY + this.measuredHeight / 2.0)
                    var newX: Double = 0.0
                    var newY: Double = 0.0
                    when (v){
                        anchorPoint3->{
                            Log.d("Rotation", rotation.toString())
                            newX = - (borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.cos(Math.toRadians(rotation.toDouble()))
                            newY = - (borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.sin(Math.toRadians(rotation.toDouble()))
                        }
                        anchorPoint1->{
                            newX = (borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.cos(Math.toRadians(rotation.toDouble()))
                            newY = (borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.sin(Math.toRadians(rotation.toDouble()))
                        }
                        anchorPoint0->{
                            newX = (borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.sin(Math.toRadians(rotation.toDouble()))
                            newY = - (borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.cos(Math.toRadians(rotation.toDouble()))
                        }
                        anchorPoint2 ->{
                            newX = -(borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.sin(Math.toRadians(rotation.toDouble()))
                            newY = (borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.cos(Math.toRadians(rotation.toDouble()))
                        }
                    }
                    point.y = centerY + newY
                    point.x = centerX + newX

//                    start = Coordinates((coord[0]-parentView.x + v.measuredWidth/2).toDouble(), (coord[1]-toolbarView.measuredHeight - v.measuredWidth/2).toDouble())
                    start = point

                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY
                }
                MotionEvent.ACTION_MOVE -> {//first_line.text = "ActionMove"
                    link.start = start
                    link.end = Coordinates((start.x + (event.rawX-oldFrameRawX)), (start.y + (event.rawY-oldFrameRawY)))
                    oldLink = link
                    parentView.addView(link)
                    colorAnchorOnHover(link.end.x.toInt(), link.end.y.toInt())
                }
                MotionEvent.ACTION_UP -> {
                    for(basicView: BasicElementView in ViewShapeHolder.getInstance().map.keys) {
                        if (basicView != this && !basicView.isSelectedByOther) {
                            basicView.setAnchorsVisible(false)
                        }
                    }
                    var otherAnchor : View? = null
                    var anchorPointEnd: AnchorPoint = AnchorPoint()
                    var otherBasicView: BasicElementView? = null
                    val x = (start.x + (event.rawX-oldFrameRawX)).toInt()
                    val y = (start.y + (event.rawY-oldFrameRawY)).toInt()
                    for(basicView: BasicElementView in ViewShapeHolder.getInstance().map.keys){
                        if(!basicView.isSelectedByOther) {
                            val  basicShapeId: String? = ViewShapeHolder.getInstance().map[basicView]
                            if (basicView.isViewInBounds(basicView.anchorPoint0, x, y)) {
                                otherAnchor = basicView.anchorPoint0
                                otherBasicView = basicView
                                if(basicShapeId != null){
                                    anchorPointEnd = AnchorPoint(basicShapeId, AnchorPoints.TOP.ordinal)
                                    basicView.anchorPoint0.setBackgroundColor(Color.TRANSPARENT)
                                    basicView.anchorPoint0.invalidate()
                                    basicView.anchorPoint0.requestLayout()
                                }
                                break
                            } else if (basicView.isViewInBounds(basicView.anchorPoint1, x, y)) {
                                otherAnchor = basicView.anchorPoint1
                                otherBasicView = basicView
                                if(basicShapeId != null){
                                    anchorPointEnd = AnchorPoint(basicShapeId, AnchorPoints.RIGHT.ordinal)
                                    basicView.anchorPoint1.setBackgroundColor(Color.TRANSPARENT)
                                    basicView.anchorPoint1.invalidate()
                                    basicView.anchorPoint1.requestLayout()
                                }
                                break
                            } else if (basicView.isViewInBounds(basicView.anchorPoint2, x, y)) {
                                otherAnchor = basicView.anchorPoint2
                                otherBasicView = basicView
                                if(basicShapeId != null){
                                    anchorPointEnd = AnchorPoint(basicShapeId, AnchorPoints.BOTTOM.ordinal)
                                    basicView.anchorPoint2.setBackgroundColor(Color.TRANSPARENT)
                                    basicView.anchorPoint2.invalidate()
                                    basicView.anchorPoint2.requestLayout()
                                }
                                break
                            } else if (basicView.isViewInBounds(basicView.anchorPoint3, x, y)) {
                                otherAnchor = basicView.anchorPoint3
                                otherBasicView = basicView
                                if(basicShapeId != null){
                                    anchorPointEnd = AnchorPoint(basicShapeId, AnchorPoints.LEFT.ordinal)
                                    basicView.anchorPoint3.setBackgroundColor(Color.TRANSPARENT)
                                    basicView.anchorPoint3.invalidate()
                                    basicView.anchorPoint3.requestLayout()
                                }
                                break
                            }
                        }
                    }

                    link.start = start
                    if(Math.abs(x - start.x) > 50 || Math.abs(y - start.y) > 50) {
                        link.end = Coordinates(x.toDouble(), y.toDouble())
                    } else {
                        link.end = Coordinates(x + 50.0, y + 50.0)
                    }
                    val thisBasicViewId = ViewShapeHolder.getInstance().map[this]
                    val linksToUpdate: ArrayList<Link> = ArrayList()
                    val formsToUpdate: ArrayList<BasicShape> = ArrayList()
                    val username: String = UserHolder.getInstance().username
                    val canevas: Canevas = ViewShapeHolder.getInstance().canevas


                    if(otherAnchor != null && anchorPointEnd.formId != "" && otherBasicView != null){

//                        val coord = IntArray(2)
//                        otherAnchor.getLocationOnScreen(coord)
//                        val activity = context as AppCompatActivity
//                        val toolbarView: View= activity.findViewById(R.id.toolbar)
//                        link.end = Coordinates((coord[0]-parentView.x + v.measuredWidth/2).toDouble(), (coord[1]-toolbarView.measuredHeight - v.measuredWidth/2).toDouble())

                        var point: Coordinates = Coordinates(0.0,0.0)
                        val centerX = (otherBasicView.leftX + otherBasicView.measuredWidth / 2.0)
                        val centerY = (otherBasicView.topY + otherBasicView.measuredHeight / 2.0)
                        var newX: Double = 0.0
                        var newY: Double = 0.0
                        when (anchorPointEnd.anchor){
                            3->{
                                Log.d("Rotation", rotation.toString())
                                newX = - (otherBasicView.borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.cos(Math.toRadians(otherBasicView.rotation.toDouble()))
                                newY = - (otherBasicView.borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.sin(Math.toRadians(otherBasicView.rotation.toDouble()))
                            }
                            1->{
                                newX = (otherBasicView.borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.cos(Math.toRadians(otherBasicView.rotation.toDouble()))
                                newY = (otherBasicView.borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.sin(Math.toRadians(otherBasicView.rotation.toDouble()))
                            }
                            0->{
                                newX = (otherBasicView.borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.sin(Math.toRadians(otherBasicView.rotation.toDouble()))
                                newY = - (otherBasicView.borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.cos(Math.toRadians(otherBasicView.rotation.toDouble()))
                            }
                            2 ->{
                                newX = -(otherBasicView.borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.sin(Math.toRadians(otherBasicView.rotation.toDouble()))
                                newY = (otherBasicView.borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.cos(Math.toRadians(otherBasicView.rotation.toDouble()))
                            }
                        }
                        point.y = centerY + newY
                        point.x = centerX + newX
                        link.end = point

                    }

                    var anchorPointStart: AnchorPoint = AnchorPoint()
                    if(thisBasicViewId != null){
                        when (v.id) {
                            R.id.anchorPoint0 -> anchorPointStart =
                                AnchorPoint(thisBasicViewId, AnchorPoints.TOP.ordinal)
                            R.id.anchorPoint1 -> anchorPointStart =
                                AnchorPoint(thisBasicViewId, AnchorPoints.RIGHT.ordinal)
                            R.id.anchorPoint2 -> anchorPointStart =
                                AnchorPoint(thisBasicViewId, AnchorPoints.BOTTOM.ordinal)
                            R.id.anchorPoint3 -> anchorPointStart =
                                AnchorPoint(thisBasicViewId, AnchorPoints.LEFT.ordinal)
                        }
                        if (anchorPointStart.formId != "") {
                            val path: ArrayList<Coordinates> = ArrayList()
                            path.add(start)
                            path.add(link.end)
                            val linkShape: Link = Link(
                                UUID.randomUUID().toString(),
                                "Link",
                                anchorPointStart,
                                anchorPointEnd,
                                3,
                                LinkStyle("#FF000000", 10, 0),
                                path
                            )

                            formsToUpdate.add(canevas.findShape(thisBasicViewId)!!)
                            linksToUpdate.add(linkShape)


                            val otherBasicViewId = ViewShapeHolder.getInstance().map[otherBasicView]

                            if(otherBasicViewId != null) {
                                formsToUpdate.add(canevas.findShape(otherBasicViewId)!!)
                                canevas.findShape(otherBasicViewId)?.linksTo?.add(linkShape.id)
                            }

                            ViewShapeHolder.getInstance().stackDrawingElementCreatedId.push(linkShape.id)
                            ViewShapeHolder.getInstance().linkMap.forcePut(link, linkShape.id)
                            canevas.links.add(linkShape)
                            canevas.findShape(thisBasicViewId)?.linksFrom?.add(linkShape.id)

                            //Log.d("createLink", linkShape)
                            var linkObj: String =""
                            val gson = Gson()
                            val linkUpdate: LinksUpdateEvent = LinksUpdateEvent(username, canevas.name, linksToUpdate)
                            linkObj = gson.toJson(linkUpdate)
                            Log.d("emitingCreateLink", linkObj)

                            var formsObj =""
                            val fromsUpdate: FormsUpdateEvent = FormsUpdateEvent(username, canevas.name, formsToUpdate)
                            formsObj = gson.toJson(fromsUpdate)
                            Log.d("emitingUpdateForms", formsObj)
                            socket?.emit(SocketConstants.CREATE_LINK, linkObj)
                            socket?.emit(SocketConstants.UPDATE_FORMS, formsObj)

                            parentView.addView(link)
                        }
                    }
                    oldLink = null
                }
            }
        }
        true
    }

    fun isViewInBounds(view: View, x: Int, y: Int): Boolean{
//        val activity = context as AppCompatActivity
//        val toolbarView: View= activity.findViewById(R.id.toolbar)
//        val parentView = view.parent.parent.parent as RelativeLayout
//        val outRect = Rect();
//        val location = IntArray(2);
//        view.getDrawingRect(outRect);
//        view.getLocationOnScreen(location);
//        outRect.offset((location[0] - parentView.x).toInt(), location[1] - toolbarView.measuredHeight - view.measuredWidth/2 - (24 * resources.displayMetrics.density / 2).toInt() );
//        outRect.left -= 5
//        outRect.right += 5


        var point: Coordinates = Coordinates(0.0,0.0)
        val centerX = (leftX + this.measuredWidth / 2.0)
        val centerY = (topY + this.measuredHeight / 2.0)
        var newX: Double = 0.0
        var newY: Double = 0.0
        when (view){
            anchorPoint3->{
                Log.d("Rotation", rotation.toString())
                newX = - (borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.cos(Math.toRadians(rotation.toDouble()))
                newY = - (borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.sin(Math.toRadians(rotation.toDouble()))
            }
            anchorPoint1->{
                newX = (borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.cos(Math.toRadians(rotation.toDouble()))
                newY = (borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.sin(Math.toRadians(rotation.toDouble()))
            }
            anchorPoint0->{
                newX = (borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.sin(Math.toRadians(rotation.toDouble()))
                newY = - (borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.cos(Math.toRadians(rotation.toDouble()))
            }
            anchorPoint2 ->{
                newX = -(borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.sin(Math.toRadians(rotation.toDouble()))
                newY = (borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.cos(Math.toRadians(rotation.toDouble()))
            }
        }
        point.y = centerY + newY
        point.x = centerX + newX

        val left = point.x - anchorPoint0.layoutParams.width
        val right = point.x + anchorPoint0.layoutParams.width
        val top = point.y - anchorPoint0.layoutParams.height
        val bottom = point.y + anchorPoint0.layoutParams.height

        val outRect: Rect = Rect(left.toInt(), top.toInt(), right.toInt(), bottom.toInt())
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
            when (event.actionMasked) {
                MotionEvent.ACTION_DOWN -> {//first_line.text = "ActionDown"
                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY

                    var isLinkMoving = false
                    val basicShapeId = ViewShapeHolder.getInstance().map[this]
                    if(basicShapeId != null ){
                        val basicShape = ViewShapeHolder.getInstance().canevas.findShape(basicShapeId)
                        if(basicShape != null){
                            for(link in basicShape.linksTo){
                                if(link != null && link != ""){
                                    val linkView = ViewShapeHolder.getInstance().linkMap.inverse()[link]
                                    if(linkView!= null && linkView.isButtonPressed){
                                        isLinkMoving = true
                                        break
                                    }
                                }
                            }
                            for(link in basicShape.linksFrom){
                                if(link != null && link != ""){
                                    val linkView = ViewShapeHolder.getInstance().linkMap.inverse()[link]
                                    if(linkView!= null && linkView.isButtonPressed){
                                        isLinkMoving = true
                                        break
                                    }
                                }
                            }
                        }
                    }
                    if(!isLinkMoving) {
                        val parentView = v.parent as RelativeLayout
                        parentView.dispatchSetSelected(false)
                        v.isSelected = true
                        //emitSelection()
                    }

                    pointerFinger1 = event.getPointerId(event.actionIndex)
                    fingersCoords[0].x = event.getX(event.findPointerIndex(pointerFinger1)).toDouble()
                    fingersCoords[0].y = event.getY(event.findPointerIndex(pointerFinger1)).toDouble()
                }
                MotionEvent.ACTION_POINTER_DOWN -> {
                    // first_line.text = "SecondFingerActionDown"
                    pointerFinger2 = event.getPointerId(event.actionIndex)

                    fingersCoords[1].x = event.getX(event.findPointerIndex(pointerFinger2)).toDouble()
                    fingersCoords[1].y = event.getY(event.findPointerIndex(pointerFinger2)).toDouble()
                }
                MotionEvent.ACTION_MOVE -> {//first_line.text = "ActionMove"

                    if(isSelected) {
                        val deltaX = event.rawX - oldFrameRawX
                        val deltaY = event.rawY - oldFrameRawY
                        this.x = this.x + deltaX
                        this.y = this.y + deltaY
                        leftX += deltaX
                        topY += deltaY
                        oldFrameRawX = event.rawX
                        oldFrameRawY = event.rawY


                        if (pointerFinger1 != -1 && pointerFinger2 != -1) {
                            fingersCoords[2].x = event.getX(event.findPointerIndex(pointerFinger1)).toDouble()
                            fingersCoords[2].y = event.getY(event.findPointerIndex(pointerFinger1)).toDouble()
                            fingersCoords[3].x = event.getX(event.findPointerIndex(pointerFinger2)).toDouble()
                            fingersCoords[3].y = event.getY(event.findPointerIndex(pointerFinger2)).toDouble()
                            //Calculate Angle
                            val angle = calculateDeltaAngle()


                            //Rotate
                            rotation += angle.toInt()


                            //Log.d("Angle", ""+angle)
                            //Log.d("PREV COORD", ""+fingersCoords[0]+"::"+fingersCoords[1])
                            //Log.d("ACTU COORD", ""+fingersCoords[2]+"::"+fingersCoords[3])

                            //Save for next step
                            fingersCoords[0].x = fingersCoords[2].x
                            fingersCoords[0].y = fingersCoords[2].y
                            fingersCoords[1].x = fingersCoords[3].x
                            fingersCoords[1].y = fingersCoords[3].y
                        }

                        val basicShapeId = ViewShapeHolder.getInstance().map[this]
                        if (basicShapeId != null) {
                            setAllLinksPosition(basicShapeId, false)
                            setAllLinksPosition(basicShapeId, true)
                        }
                    }


                }
                MotionEvent.ACTION_UP -> {
                    // first_line.text = "ActionUp"
                    if(isSelected) {
                        val activity: AppCompatActivity = context as AppCompatActivity
                        if (activity is DrawingActivity) {
                            val drawingActivity: DrawingActivity = activity as DrawingActivity
                            drawingActivity.syncCanevasFromLayout()
                        }
                        emitUpdate()
                        emitLinkUpdate(ViewShapeHolder.getInstance().canevas.findShape(ViewShapeHolder.getInstance().map[this]!!)?.linksFrom!!)
                        emitLinkUpdate(ViewShapeHolder.getInstance().canevas.findShape(ViewShapeHolder.getInstance().map[this]!!)?.linksTo!!)
                    }
                    pointerFinger1 = -1
                }
                MotionEvent.ACTION_POINTER_UP ->{
                    pointerFinger2 = -1
                }
                MotionEvent.ACTION_CANCEL -> {
                    pointerFinger1 = -1
                    pointerFinger2 = -1
                }
            }
        }
        true
    }

    private fun colorAnchorOnHover(x: Int, y: Int) {
        for (basicView: BasicElementView in ViewShapeHolder.getInstance().map.keys) {
            if (!basicView.isSelectedByOther) {
                val basicShapeId: String? = ViewShapeHolder.getInstance().map[basicView]
                if (basicView.isViewInBounds(basicView.anchorPoint0, x, y)) {
                    if (basicShapeId != null) {
                        basicView.anchorPoint0.setBackgroundColor(Color.GREEN)
                        basicView.anchorPoint0.invalidate()
                        basicView.anchorPoint0.requestLayout()
                    }
                    break
                } else if (basicView.isViewInBounds(basicView.anchorPoint1, x, y)) {
                    if (basicShapeId != null) {
                        basicView.anchorPoint1.setBackgroundColor(Color.GREEN)
                        basicView.anchorPoint1.invalidate()
                        basicView.anchorPoint1.requestLayout()
                    }
                    break
                } else if (basicView.isViewInBounds(basicView.anchorPoint2, x, y)) {
                    if (basicShapeId != null) {
                        basicView.anchorPoint2.setBackgroundColor(Color.GREEN)
                        basicView.anchorPoint2.invalidate()
                        basicView.anchorPoint2.requestLayout()
                    }
                    break
                } else if (basicView.isViewInBounds(basicView.anchorPoint3, x, y)) {
                    if (basicShapeId != null) {
                        basicView.anchorPoint3.setBackgroundColor(Color.GREEN)
                        basicView.anchorPoint3.invalidate()
                        basicView.anchorPoint3.requestLayout()
                    }
                    break
                } else {
                    basicView.anchorPoint0.setBackgroundColor(Color.TRANSPARENT)
                    basicView.anchorPoint0.invalidate()
                    basicView.anchorPoint0.requestLayout()
                    basicView.anchorPoint1.setBackgroundColor(Color.TRANSPARENT)
                    basicView.anchorPoint1.invalidate()
                    basicView.anchorPoint1.requestLayout()
                    basicView.anchorPoint2.setBackgroundColor(Color.TRANSPARENT)
                    basicView.anchorPoint2.invalidate()
                    basicView.anchorPoint2.requestLayout()
                    basicView.anchorPoint3.setBackgroundColor(Color.TRANSPARENT)
                    basicView.anchorPoint3.invalidate()
                    basicView.anchorPoint3.requestLayout()
                }
            }
        }
    }
    private fun calculateDeltaAngle() : Float{
        val angle1 : Double = Math.atan2( (fingersCoords[1].y - fingersCoords[0].y), (fingersCoords[1].x - fingersCoords[0].x))
        val angle2 : Double = Math.atan2( (fingersCoords[3].y - fingersCoords[2].y), (fingersCoords[3].x - fingersCoords[2].x))

        var angle = (Math.toDegrees(angle2 - angle1) % 360).toFloat()

        if (angle < -180.0f){
            angle += 360.0f
        }else if (angle > 180.0f){
            angle -= 360.0f
        }

        return angle
    }

    open protected var onTouchListenerEditButton = View.OnTouchListener { v, event ->
        when(event.action){
            MotionEvent.ACTION_DOWN -> {
                editShape()
            }
            /*val activity: AppCompatActivity = context as AppCompatActivity
            if(activity is DrawingActivity){
                val drawingActivity : DrawingActivity = activity as DrawingActivity
                drawingActivity.syncCanevasFromLayout()
            }
            emitUpdate()*/
        }
        true
    }

    open fun editShape(){
        val shapeId: String? = ViewShapeHolder.getInstance().map[this]

        var activity: AppCompatActivity = context as AppCompatActivity
        var dialog: DialogFragment = EditBasicElementDialogFragment()

        var bundle: Bundle = Bundle()
        bundle.putString("shapeId", shapeId)
        dialog.arguments = bundle

        dialog.show(activity.supportFragmentManager, "alllooooo")
    }

    private var onTouchListenerDeleteButton = View.OnTouchListener { v, event ->
        when(event.action){
            MotionEvent.ACTION_DOWN -> {//first_line.text = "onTouchListenerDeleteButton"

                deselecteAllConnectedLinks()
                emitDelete()

                VFXHolder.getInstance().fireDeleteVFX(leftX+borderResizableLayout.layoutParams.width/2 ,topY+borderResizableLayout.layoutParams.height/2,context)
                val parentView = v.parent.parent.parent as RelativeLayout

                val shapeId: String? = ViewShapeHolder.getInstance().map[this]
                val shape: BasicShape? = ViewShapeHolder.getInstance().canevas.findShape(shapeId!!)
                val linksFrom: ArrayList<String?>? = shape?.linksFrom?.clone() as ArrayList<String?>
                val linksTo: ArrayList<String?>? = shape?.linksTo?.clone() as ArrayList<String?>

                for(linkId: String? in linksFrom!! ) {
                    Log.d("linkid", linkId)
                    val link: Link? = ViewShapeHolder.getInstance().canevas.findLink(linkId!!)
                    if(link!= null){
                        link.from.formId = ""
                        link.from.anchor = 0
                    }
//                    val linkView: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[linkId]
//                    if(linkView != null){
//                        parentView.removeView(linkView)
//                    }
//                    ViewShapeHolder.getInstance().remove(link!!)
                }

                for(linkId: String? in linksTo!! ) {
                    val link: Link? = ViewShapeHolder.getInstance().canevas.findLink(linkId!!)
                    if(link!= null){
                        link.to.formId = ""
                        link.to.anchor = 0
                    }
//                    val linkView: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[linkId]
//                    if(linkView != null){
//                        parentView.removeView(linkView)
//                    }
//                    ViewShapeHolder.getInstance().remove(link!!)
                }

                ViewShapeHolder.getInstance().stackDrawingElementCreatedId.remove(ViewShapeHolder.getInstance().map.getValue(this))
                parentView.removeView(this)
                ViewShapeHolder.getInstance().remove(this)

            }
        }
        true
    }

    open protected var onTouchListenerResizeButton = View.OnTouchListener { v, event ->
        when(event.action){
            MotionEvent.ACTION_DOWN -> {//first_line.text = "ActionDownResize"
                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY
            }
            MotionEvent.ACTION_MOVE -> {
                var deltaX: Int = (event.rawX - oldFrameRawX).toInt()
                var deltaY: Int = (event.rawY - oldFrameRawY).toInt()
                val newWidth = borderResizableLayout.width + deltaX
                val newHeight = borderResizableLayout.height + deltaY

                resize(newWidth, newHeight)

                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY


                val basicShapeId = ViewShapeHolder.getInstance().map[this]
                if(basicShapeId != null){
                    setAllLinksPosition(basicShapeId, false)
                    setAllLinksPosition(basicShapeId, true)
                }
            }
            MotionEvent.ACTION_UP -> {
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

    fun setAllLinksPosition(basicShapeId: String, isFrom:Boolean){
        val links = if(isFrom){
            ViewShapeHolder.getInstance().canevas.findShape(basicShapeId)?.linksFrom
        } else {
            ViewShapeHolder.getInstance().canevas.findShape(basicShapeId)?.linksTo
        }
        if(links != null){
            for(linkId in links){
                if(linkId != null) {
                    val linkView: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[linkId]
                    val linkShape: Link? = ViewShapeHolder.getInstance().canevas.findLink(linkId)
                    if (linkView != null && linkShape != null) {

                        setLinkPositionWithAnchor(linkShape, isFrom)
                        linkView.requestLayout()

                    }
                }
            }
        }
    }

    private fun setLinkPositionWithAnchor(link: Link, isFrom: Boolean){
        var anchor: Int
        var point: Coordinates
        if(isFrom){
            anchor = link.from.anchor
            point = link.path.first()
        } else {
            anchor =link.to.anchor
            point = link.path.last()
        }
        val centerX = (leftX + this.measuredWidth / 2.0)
        val centerY = (topY + this.measuredHeight / 2.0)
        var newX: Double = 0.0
        var newY: Double = 0.0
        when (anchor){
            AnchorPoints.LEFT.ordinal->{
                Log.d("Rotation", rotation.toString())
                newX = - (borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.cos(Math.toRadians(rotation.toDouble()))
                newY = - (borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.sin(Math.toRadians(rotation.toDouble()))
            }
            AnchorPoints.RIGHT.ordinal->{
                newX = (borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.cos(Math.toRadians(rotation.toDouble()))
                newY = (borderResizableLayout.layoutParams.width + anchorPoint0.layoutParams.width) / 2.0 * Math.sin(Math.toRadians(rotation.toDouble()))
            }
            AnchorPoints.TOP.ordinal->{
                newX = (borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.sin(Math.toRadians(rotation.toDouble()))
                newY = - (borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.cos(Math.toRadians(rotation.toDouble()))
            }

            AnchorPoints.BOTTOM.ordinal ->{
                newX = -(borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.sin(Math.toRadians(rotation.toDouble()))
                newY = (borderResizableLayout.layoutParams.height + anchorPoint0.layoutParams.height) / 2.0 * Math.cos(Math.toRadians(rotation.toDouble()))
            }
        }
        point.y = centerY + newY
        point.x = centerX + newX
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

    open fun outlineColor(color: String, borderType: Int){}
    open fun backgroundColor(color: String){}

    fun emitUpdate(){
        val response: String = this.createFormsUpdateEvent()

        if(response !="") {
            Log.d("emitingUpdate", response)
            socket?.emit(SocketConstants.UPDATE_FORMS, response)
        }
        SyncShapeHolder.getInstance().drawingActivity!!.saveCanevas()
    }

    fun emitSelection(){
        val response: String = this.createFormsUpdateEvent()

        if(response !="") {
            Log.d("emitingSelection", response)
            socket?.emit(SocketConstants.SELECT_FORMS, response)
        }
    }

    fun emitDeselection(){
        val response: String = this.createFormsUpdateEvent()

        if(response !="") {
            Log.d("emitingDeselection", response)
            socket?.emit(SocketConstants.DESELECT_FORMS, response)
        }
    }

    fun emitDelete(){
        val response: String = this.createFormsUpdateEvent()

        if(response !="") {
            Log.d("emitingDelete", response)
            socket?.emit(SocketConstants.DELETE_FORMS, response)
        }

        SyncShapeHolder.getInstance().drawingActivity!!.saveCanevas()
    }

    private fun createFormsUpdateEvent(): String{
        val basicShape: BasicShape? = ViewShapeHolder.getInstance().canevas.findShape(ViewShapeHolder.getInstance().map.getValue(this))
        val formsArray: ArrayList<BasicShape> = ArrayList()
        var obj: String =""
        if(basicShape !=null) {
            formsArray.add(basicShape)
            val gson = Gson()
            val response: FormsUpdateEvent = FormsUpdateEvent(UserHolder.getInstance().username, ViewShapeHolder.getInstance().canevas.name, formsArray)
            obj = gson.toJson(response)
        }
        return obj
    }

    private fun createLinksUpdateEvent(linksArray: ArrayList<Link>): String{
        var obj: String =""
        if(!linksArray.isEmpty()) {
            val gson = Gson()
            val response: LinksUpdateEvent = LinksUpdateEvent(UserHolder.getInstance().username, ViewShapeHolder.getInstance().canevas.name, linksArray)
            obj = gson.toJson(response)
        }
        return obj
    }

    private fun emitLinkUpdate(linksIdArray: ArrayList<String?>){
        val linksArray = ArrayList<Link>()
        for(id in linksIdArray) {
            if(id != null) {
                val link = ViewShapeHolder.getInstance().canevas.findLink(id)
                if(link != null){
                    linksArray.add(link)
                }
            }
        }
        val response: String = this.createLinksUpdateEvent(linksArray)

        if(response !="") {
            Log.d("emitingUpdateLinks", response)
            socket?.emit(SocketConstants.UPDATE_LINKS, response)
        }
    }

}
package com.polypaint.polypaint.View

import androidx.appcompat.app.AppCompatActivity
import android.content.Context
import android.graphics.Rect
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
import com.polypaint.polypaint.Enum.AnchorPoints
import com.polypaint.polypaint.Fragment.EditClassDialogFragment
import com.polypaint.polypaint.Holder.UserHolder
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.*
import com.polypaint.polypaint.R
import com.polypaint.polypaint.Socket.SocketConstants
import com.polypaint.polypaint.SocketReceptionModel.FormsUpdateEvent
import com.polypaint.polypaint.SocketReceptionModel.LinksUpdateEvent
import kotlinx.android.synthetic.main.basic_element.view.*
import java.util.*
import kotlin.collections.ArrayList


open class BasicElementView: RelativeLayout {

    var oldFrameRawX : Float = 0.0F
    var oldFrameRawY : Float = 0.0F
    var start: Coordinates = Coordinates(0.0,0.0)
    var oldLink: LinkView? = null
    var isSelectedByOther: Boolean = false
    open var mMinimumWidth : Float = 300F
    open var mMinimumHeight : Float = 100F
    var socket: Socket? = null

    var pointerFinger1 : Int = -1
    var pointerFinger2 : Int = -1

    var fingersCoords : Array<Coordinates> = Array(4) { Coordinates(0.0,0.0) }

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

    override fun setSelected(selected: Boolean) {
        if(selected){
            //first_line.text = "Focus"
            borderResizableLayout.setBackgroundResource(R.drawable.borders_blue)
            editButton.visibility = View.VISIBLE
            deleteButton.visibility = View.VISIBLE
            resizeButton.visibility = View.VISIBLE
            setAnchorsVisible(true)
        }else{
            //first_line.text = "NoFocus"
            borderResizableLayout.setBackgroundResource(R.drawable.borders_white)
            editButton.visibility = View.INVISIBLE
            deleteButton.visibility = View.INVISIBLE
            resizeButton.visibility = View.INVISIBLE
            setAnchorsVisible(false)
        }
        return super.setSelected(selected)
    }

    private fun setAnchorsVisible(isVisible: Boolean){
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
                    for(basicView: BasicElementView in ViewShapeHolder.getInstance().map.keys) {
                        if (basicView != this && !basicView.isSelectedByOther) {
                            basicView.setAnchorsVisible(false)
                        }
                    }
                    var otherAnchor : View? = null
                    var anchorPointEnd: AnchorPoint? = null
                    var otherBasicView: BasicElementView? = null
                    val x = (start.x + (event.rawX-oldFrameRawX)).toInt()
                    val y = (start.y + (event.rawY-oldFrameRawY)).toInt()
                    for(basicView: BasicElementView in ViewShapeHolder.getInstance().map.keys){
                        if(basicView != this && !basicView.isSelectedByOther) {
                            val  basicShapeId: String? = ViewShapeHolder.getInstance().map[basicView]
                            if (isViewInBounds(basicView.anchorPoint0, x, y)) {
                                otherAnchor = basicView.anchorPoint0
                                otherBasicView = basicView
                                if(basicShapeId != null){
                                    anchorPointEnd = AnchorPoint(basicShapeId, AnchorPoints.TOP.ordinal)
                                }
                                break
                            } else if (isViewInBounds(basicView.anchorPoint1, x, y)) {
                                otherAnchor = basicView.anchorPoint1
                                otherBasicView = basicView
                                if(basicShapeId != null){
                                    anchorPointEnd = AnchorPoint(basicShapeId, AnchorPoints.RIGHT.ordinal)
                                }
                                break
                            } else if (isViewInBounds(basicView.anchorPoint2, x, y)) {
                                otherAnchor = basicView.anchorPoint2
                                otherBasicView = basicView
                                if(basicShapeId != null){
                                    anchorPointEnd = AnchorPoint(basicShapeId, AnchorPoints.BOTTOM.ordinal)
                                }
                                break
                            } else if (isViewInBounds(basicView.anchorPoint3, x, y)) {
                                otherAnchor = basicView.anchorPoint3
                                otherBasicView = basicView
                                if(basicShapeId != null){
                                    anchorPointEnd = AnchorPoint(basicShapeId, AnchorPoints.LEFT.ordinal)
                                }
                                break
                            }
                        }
                    }
                    if(otherAnchor != null && anchorPointEnd !=null && otherBasicView != null){
                        link.start = start
                        val coord = IntArray(2)
                        otherAnchor.getLocationOnScreen(coord)
                        val activity = context as AppCompatActivity
                        val toolbarView: View= activity.findViewById(R.id.toolbar)
                        link.end = Coordinates((coord[0]-parentView.x + v.measuredWidth/2).toDouble(), (coord[1]-toolbarView.measuredHeight - v.measuredWidth/2).toDouble())


                        val thisBasicViewId = ViewShapeHolder.getInstance().map[this]
                        val otherBasicViewId = ViewShapeHolder.getInstance().map[otherBasicView]
                        var anchorPointStart: AnchorPoint ?= null
                        if(thisBasicViewId != null && otherBasicViewId != null) {
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

                            if (anchorPointStart != null) {
                                val path: ArrayList<Coordinates> = ArrayList()
                                path.add(start)
                                path.add(link.end)
                                val linkShape: Link = Link(
                                    UUID.randomUUID().toString(),
                                    "Link",
                                    anchorPointStart,
                                    anchorPointEnd,
                                    3,
                                    LinkStyle("BLACK", 10, 0),
                                    path
                                )

                                val username: String = UserHolder.getInstance().username
                                val canevas: Canevas = ViewShapeHolder.getInstance().canevas
                                val links: ArrayList<Link> = ArrayList()
                                val formsToUpdate: ArrayList<BasicShape> = ArrayList()

                                formsToUpdate.add(canevas.findShape(thisBasicViewId)!!)
                                formsToUpdate.add(canevas.findShape(otherBasicViewId)!!)
                                links.add(linkShape)

                                ViewShapeHolder.getInstance().linkMap.forcePut(link, linkShape.id)
                                canevas.links.add(linkShape)
                                canevas.findShape(thisBasicViewId)?.linksFrom?.add(linkShape.id)
                                canevas.findShape(otherBasicViewId)?.linksTo?.add(linkShape.id)

                                //Log.d("createLink", linkShape)
                                var linkObj: String =""
                                val gson = Gson()
                                val linkUpdate: LinksUpdateEvent = LinksUpdateEvent(username, canevas.name, links)
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
            when (event.actionMasked) {
                MotionEvent.ACTION_DOWN -> {//first_line.text = "ActionDown"
                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY

                    val parentView = v.parent as RelativeLayout
                    parentView.dispatchSetSelected(false)
                    v.isSelected = true
                    emitSelection()

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

                    val deltaX = event.rawX - oldFrameRawX
                    val deltaY = event.rawY - oldFrameRawY
                    this.x = this.x + deltaX
                    this.y = this.y + deltaY
                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY


                    if(pointerFinger1 != -1 && pointerFinger2 != -1) {
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
                    if(basicShapeId != null){
                        val linksTo = ViewShapeHolder.getInstance().canevas.findShape(basicShapeId)?.linksTo
                        if(linksTo != null){
                            for(linkId in linksTo){
                                if(linkId != null) {
                                    val linkView: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[linkId]
                                    val linkShape: Link? = ViewShapeHolder.getInstance().canevas.findLink(linkId)
                                    if (linkView != null && linkShape != null) {
//                                        linkView.end.x += deltaX
//                                        linkView.end.y += deltaY

                                        linkShape.path.last().x += deltaX
                                        linkShape.path.last().y += deltaY
                                        linkView.requestLayout()

                                    }
                                }
                            }
                        }
                        val linksFrom = ViewShapeHolder.getInstance().canevas.findShape(basicShapeId)?.linksFrom
                        if(linksFrom != null){
                            for(linkId in linksFrom){
                                if(linkId != null) {
                                    val linkView: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[linkId]
                                    val linkShape: Link? = ViewShapeHolder.getInstance().canevas.findLink(linkId)
                                    if (linkView != null && linkShape != null) {
//                                        linkView.start.x += deltaX
//                                        linkView.start.y += deltaY

                                        linkShape.path.first().x += deltaX
                                        linkShape.path.first().y += deltaY
                                        linkView.requestLayout()

                                    }
                                }
                            }
                        }
                    }


                }
                MotionEvent.ACTION_UP -> {
                    // first_line.text = "ActionUp"
                    val activity: AppCompatActivity = context as AppCompatActivity
                    if (activity is DrawingActivity) {
                        val drawingActivity: DrawingActivity = activity as DrawingActivity
                        drawingActivity.syncCanevasFromLayout()
                    }
                    emitUpdate()
                    emitLinkUpdate(ViewShapeHolder.getInstance().canevas.findShape(ViewShapeHolder.getInstance().map[this]!!)?.linksFrom!!)
                    emitLinkUpdate(ViewShapeHolder.getInstance().canevas.findShape(ViewShapeHolder.getInstance().map[this]!!)?.linksTo!!)
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

                val shapeId: String? = ViewShapeHolder.getInstance().map[this]
                val shape: BasicShape? = ViewShapeHolder.getInstance().canevas.findShape(shapeId!!)
                val linksFrom: ArrayList<String?>? = shape?.linksFrom?.clone() as ArrayList<String?>
                val linksTo: ArrayList<String?>? = shape?.linksTo?.clone() as ArrayList<String?>

                for(linkId: String? in linksFrom!! ) {
                    Log.d("linkid", linkId)
                    val link: Link? = ViewShapeHolder.getInstance().canevas.findLink(linkId!!)
                    val linkView: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[linkId]
                    if(linkView != null){
                        parentView.removeView(linkView)
                    }
                    ViewShapeHolder.getInstance().remove(link!!)
                }

                for(linkId: String? in linksTo!! ) {
                    val link: Link? = ViewShapeHolder.getInstance().canevas.findLink(linkId!!)
                    val linkView: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[linkId]
                    if(linkView != null){
                        parentView.removeView(linkView)
                    }
                    ViewShapeHolder.getInstance().remove(link!!)
                }

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
            MotionEvent.ACTION_DOWN -> {//first_line.text = "ActionDownResize"
                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY
            }
            MotionEvent.ACTION_MOVE -> {
                var deltaX = event.rawX - oldFrameRawX
                var deltaY = event.rawY - oldFrameRawY
                val newWidth = borderResizableLayout.width + deltaX
                val newHeight = borderResizableLayout.height + deltaY

                resize(newWidth.toInt(), newHeight.toInt())

                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY


                if(newWidth < mMinimumWidth){
                    deltaX = 0f
                }
                if(newHeight < mMinimumHeight){
                    deltaY = 0f
                }

                val basicShapeId = ViewShapeHolder.getInstance().map[this]
                if(basicShapeId != null){
                    val linksTo = ViewShapeHolder.getInstance().canevas.findShape(basicShapeId)?.linksTo
                    if(linksTo != null){
                        for(linkId in linksTo){
                            if(linkId != null) {
                                val linkView: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[linkId]
                                val linkShape: Link? = ViewShapeHolder.getInstance().canevas.findLink(linkId)
                                if (linkView != null && linkShape != null) {
//                                        linkView.end.x += deltaX
//                                        linkView.end.y += deltaY
                                    when (linkShape.to.anchor){
                                        AnchorPoints.LEFT.ordinal->linkShape.path.last().y += deltaY / 2
                                        AnchorPoints.TOP.ordinal->linkShape.path.last().x +=deltaX / 2
                                        AnchorPoints.RIGHT.ordinal->{
                                            linkShape.path.last().x += deltaX
                                            linkShape.path.last().y += deltaY / 2
                                        }
                                        AnchorPoints.BOTTOM.ordinal ->{
                                            linkShape.path.last().x += deltaX / 2
                                            linkShape.path.last().y += deltaY
                                        }
                                    }
                                    linkView.requestLayout()
                                }
                            }
                        }
                    }
                    val linksFrom = ViewShapeHolder.getInstance().canevas.findShape(basicShapeId)?.linksFrom
                    if(linksFrom != null){
                        for(linkId in linksFrom){
                            if(linkId != null) {
                                val linkView: LinkView? = ViewShapeHolder.getInstance().linkMap.inverse()[linkId]
                                val linkShape: Link? = ViewShapeHolder.getInstance().canevas.findLink(linkId)
                                if (linkView != null && linkShape != null) {
//                                        linkView.start.x += deltaX
//                                        linkView.start.y += deltaY

                                    when (linkShape.from.anchor){
                                        AnchorPoints.LEFT.ordinal->linkShape.path.first().y += deltaY / 2
                                        AnchorPoints.TOP.ordinal->linkShape.path.first().x +=deltaX / 2
                                        AnchorPoints.RIGHT.ordinal -> {
                                            linkShape.path.first().x += deltaX
                                            linkShape.path.first().y += deltaY / 2
                                        }
                                        AnchorPoints.BOTTOM.ordinal ->{
                                            linkShape.path.first().x += deltaX / 2
                                            linkShape.path.first().y += deltaY
                                        }
                                    }
                                    linkView.requestLayout()

                                }
                            }
                        }
                    }
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
        val response: String = this.createFormsUpdateEvent()

        if(response !="") {
            Log.d("emitingUpdate", response)
            socket?.emit(SocketConstants.UPDATE_FORMS, response)
        }
    }

    private fun emitSelection(){
        val response: String = this.createFormsUpdateEvent()

        if(response !="") {
            Log.d("emitingSelection", response)
            socket?.emit(SocketConstants.SELECT_FORMS, response)
        }
    }

    private fun emitDelete(){
        val response: String = this.createFormsUpdateEvent()

        if(response !="") {
            Log.d("emitingDelete", response)
            socket?.emit(SocketConstants.DELETE_FORMS, response)
        }
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
            linksArray.add(ViewShapeHolder.getInstance().canevas.findLink(id!!)!!)
        }
        val response: String = this.createLinksUpdateEvent(linksArray)

        if(response !="") {
            Log.d("emitingUpdateLinks", response)
            socket?.emit(SocketConstants.UPDATE_LINKS, response)
        }
    }

}
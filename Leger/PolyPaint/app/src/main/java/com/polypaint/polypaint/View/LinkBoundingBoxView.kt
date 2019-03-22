package com.polypaint.polypaint.View

import android.content.Context
import android.graphics.*
import android.util.Log
import android.view.MotionEvent
import android.view.View
import android.view.ViewGroup
import android.widget.ImageButton
import android.widget.RelativeLayout
import androidx.appcompat.app.AppCompatActivity
import com.polypaint.polypaint.Activity.DrawingActivity
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.Coordinates
import com.polypaint.polypaint.Model.Link
import com.polypaint.polypaint.R
import kotlinx.android.synthetic.main.basic_element.view.*

class LinkBoundingBoxView(context: Context?, var linkView: LinkView) : View(context) {
    var rect: RectF  = RectF()
    var paint: Paint = Paint()
    var resizeButton: ImageButton? = null
    var oldFrameRawX: Float = 0f
    var oldFrameRawY: Float = 0f

    var pointerFinger1 : Int = -1
    var pointerFinger2 : Int = -1

    var fingersCoords : Array<Coordinates> = Array(4) { Coordinates(0.0,0.0) }
    var downWasInBox: Boolean = false

    var initialHeight: Double = 0.0
    var initialWidth: Double = 0.0
    var initialPath: ArrayList<Coordinates> = ArrayList()
    var newPath: ArrayList<Coordinates> = ArrayList()

    init {
        paint.color = Color.DKGRAY
        paint.style = Paint.Style.STROKE
    }

    override fun onAttachedToWindow() {
        val parentView = this.parent as RelativeLayout
        setOnTouchListener(onTouchListenerBody)
        val layoutParams = ViewGroup.LayoutParams(100, 100)
        resizeButton = ImageButton(context)
        resizeButton?.setImageResource(R.drawable.ic_resize)
        resizeButton?.layoutParams = layoutParams
        resizeButton?.x = rect.right
        resizeButton?.y = rect.bottom
        resizeButton?.setOnTouchListener(onTouchListenerResizeButton)
        resizeButton?.visibility = View.INVISIBLE
        parentView.addView(resizeButton)

        super.onAttachedToWindow()
    }

    fun setVisible(isVisible: Boolean){
        if(isVisible){
            this.visibility = VISIBLE
            resizeButton?.visibility = VISIBLE
        } else {
            this.visibility = INVISIBLE
            resizeButton?.visibility = INVISIBLE
        }
    }

    fun delete(){
        val parentView = this.parent as RelativeLayout
        if(resizeButton!= null){
            parentView.removeView(resizeButton)
        }
        parentView.removeView(this)
    }
    override fun onDraw(canvas: Canvas?) {

        canvas?.drawRect(rect, paint)

        resizeButton?.x = rect.right
        resizeButton?.y = rect.bottom

        resizeButton?.visibility = INVISIBLE
        val linkId: String? = ViewShapeHolder.getInstance().linkMap[linkView]
        if(linkId != null) {
            val link: Link? = ViewShapeHolder.getInstance().canevas.findLink(linkId)
            if (link != null) {
                if (link.from.formId == "" && link.to.formId == "") {
                    resizeButton?.visibility = this.visibility
                }
            }
        }

    }

    private var onTouchListenerResizeButton = View.OnTouchListener { v, event ->
        //val txt = first_line.text
        //first_line.text = txt.toString() + "onTouchListenerResizeButton"

        val linkId: String? = ViewShapeHolder.getInstance().linkMap[linkView]
        if(linkId != null) {
            val link: Link? = ViewShapeHolder.getInstance().canevas.findLink(linkId)
            when (event.action) {
                MotionEvent.ACTION_DOWN -> {//first_line.text = "ActionDownResize"
                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY
                    initialHeight = rect.height().toDouble()
                    initialWidth = rect.width().toDouble()
                    if (link != null) {
                        initialPath = link.path.clone() as ArrayList<Coordinates>
                        newPath = link.path.clone() as ArrayList<Coordinates>
                    }


                }
                MotionEvent.ACTION_MOVE -> {
                    val deltaX: Float = (event.rawX - oldFrameRawX)
                    val deltaY: Float = (event.rawY - oldFrameRawY)
                    v.x += deltaX
                    v.y += deltaY
                    rect.right += deltaX
                    rect.bottom += deltaY



                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY

                    invalidate()
                    requestLayout()

                }
                MotionEvent.ACTION_UP -> {
                    val heightRatio = rect.height() / initialHeight
                    val widthRatio = rect.width() / initialWidth

                    if (link != null) {
                        if (link.from.formId == "" && link.to.formId == "") {
                            for (i in 0..(link.path.size - 1)) {
                                val distY = initialPath[i].y - rect.top
                                val distX = initialPath[i].x - rect.left
                                link.path[i].y = rect.top + distY * heightRatio
                                link.path[i].x = rect.left + distX * widthRatio
                            }
                            linkView.invalidate()
                            linkView.requestLayout()
                        }
                    }

                    invalidate()
                    requestLayout()
                    linkView.emitUpdate()
                }
            }
        }
        true
    }

    private var onTouchListenerBody = View.OnTouchListener { v, event ->
        val region: Region = Region(rect.left.toInt(), rect.top.toInt(), rect.right.toInt(), rect.bottom.toInt())
        var totalRotation: Float
        when (event.actionMasked) {
            MotionEvent.ACTION_DOWN -> {
                if (region.contains(event.x.toInt(), event.y.toInt())) {
                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY

                    //emitSelection()

                    downWasInBox = true
                    pointerFinger1 = event.getPointerId(event.actionIndex)
                    fingersCoords[0].x = event.getX(event.findPointerIndex(pointerFinger1)).toDouble()
                    fingersCoords[0].y = event.getY(event.findPointerIndex(pointerFinger1)).toDouble()
                    true
                } else {
                    false
                }
            }
            MotionEvent.ACTION_POINTER_DOWN -> {
                // first_line.text = "SecondFingerActionDown"
                pointerFinger2 = event.getPointerId(event.actionIndex)

                fingersCoords[1].x = event.getX(event.findPointerIndex(pointerFinger2)).toDouble()
                fingersCoords[1].y = event.getY(event.findPointerIndex(pointerFinger2)).toDouble()
                true
            }
            MotionEvent.ACTION_MOVE -> {//first_line.text = "ActionMove"

                if(downWasInBox) {

                    if (pointerFinger1 != -1 && pointerFinger2 != -1) {
                        fingersCoords[2].x = event.getX(event.findPointerIndex(pointerFinger1)).toDouble()
                        fingersCoords[2].y = event.getY(event.findPointerIndex(pointerFinger1)).toDouble()
                        fingersCoords[3].x = event.getX(event.findPointerIndex(pointerFinger2)).toDouble()
                        fingersCoords[3].y = event.getY(event.findPointerIndex(pointerFinger2)).toDouble()
                        //Calculate Angle
                        val angle = calculateDeltaAngle()

                        totalRotation = angle

                        val linkId: String? = ViewShapeHolder.getInstance().linkMap[linkView]
                        if(linkId != null){
                            val link: Link? =ViewShapeHolder.getInstance().canevas.findLink(linkId)
                            if(link != null){
                                if(link.from.formId == "" && link.to.formId == "") {
                                    val angleInRad = Math.toRadians(totalRotation.toDouble())
                                    for (point in link.path) {
                                        val distX = point.x - rect.centerX()
                                        val distY = point.y - rect.centerY()

                                        val newX = - distY * Math.sin(angleInRad) + distX * Math.cos(angleInRad)
                                        val newY = distY * Math.cos(angleInRad) + distX * Math.sin(angleInRad)

                                        point.x = rect.centerX() + newX
                                        point.y = rect.centerY() + newY
                                    }
                                    linkView.invalidate()
                                    linkView.requestLayout()
                                }
                            }
                        }

                        //Save for next step
                        fingersCoords[0].x = fingersCoords[2].x
                        fingersCoords[0].y = fingersCoords[2].y
                        fingersCoords[1].x = fingersCoords[3].x
                        fingersCoords[1].y = fingersCoords[3].y
                    }


                    val deltaX = event.rawX - oldFrameRawX
                    val deltaY = event.rawY - oldFrameRawY
                    linkView.moveLink(deltaX.toDouble(), deltaY.toDouble())

                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY


                    true
                } else {
                    false
                }
            }
            MotionEvent.ACTION_UP -> {
                if(downWasInBox){
                    linkView.emitUpdate()
                }
                downWasInBox = false
                pointerFinger1 = -1
                true
            }
            MotionEvent.ACTION_POINTER_UP -> {
                pointerFinger2 = -1
                true
            }
            MotionEvent.ACTION_CANCEL -> {
                pointerFinger1 = -1
                pointerFinger2 = -1
                true
            }
            else -> {
                false
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
}
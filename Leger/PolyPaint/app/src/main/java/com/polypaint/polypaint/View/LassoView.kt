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
import kotlinx.android.synthetic.main.activity_drawing.*
import kotlinx.android.synthetic.main.basic_element.view.*

class LassoView(context: Context?) : View(context) {
    var paint: Paint = Paint()
    var oldFrameRawX: Float = 0f
    var oldFrameRawY: Float = 0f

    var downWasInBox: Boolean = false
    var pathPoints: ArrayList<Coordinates> = ArrayList()
    var path: Path = Path()
    var region: Region = Region()
    var isPathClosed: Boolean = false
    var closingRegion: Region? = null

    var preview: LassoView? = null
    var isPointAdded: Boolean = false
    var isDragged: Boolean = false

    var viewsIn: ArrayList<BasicElementView> = ArrayList()
    var linksIn: ArrayList<LinkView> = ArrayList()


    init {
        paint.color = Color.DKGRAY
        paint.style = Paint.Style.STROKE
    }

    override fun onAttachedToWindow() {
        setOnTouchListener(onTouchListenerBody)
        super.onAttachedToWindow()
    }

    fun delete(){
        val parentView = this.parent as RelativeLayout
        parentView.removeView(this)
    }

    override fun onDraw(canvas: Canvas?) {
        canvas?.drawPath(path, paint)
    }

    private fun createPath(){
        path = Path()
        if(pathPoints.size > 0) {

            path.moveTo(pathPoints[0].x.toFloat(), pathPoints[0].y.toFloat())
            if(closingRegion == null){
                closingRegion = Region((pathPoints[0].x - 50).toInt(), (pathPoints[0].y - 50).toInt(), (pathPoints[0].x + 50).toInt(), (pathPoints[0].y + 50).toInt())
            }
            for (point in pathPoints) {
                path.lineTo(point.x.toFloat(), point.y.toFloat())
            }
            if (isPathClosed) {
                path.close()
                val rect = RectF()
                path.computeBounds(rect, true)
                Log.d("rect", rect.toString())
                region = Region()
                region.setPath(
                    path,
                    Region(rect.left.toInt(), rect.top.toInt(), rect.right.toInt(), rect.bottom.toInt())
                )
            }
        }
    }

    private var onTouchListenerBody = View.OnTouchListener { v, event ->
        val parentView = this.parent as RelativeLayout

        when (event.actionMasked) {
            MotionEvent.ACTION_DOWN -> {
                isPointAdded = false

                if (region.contains(event.x.toInt(), event.y.toInt())) {
                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY

                    //emitSelection()

                    downWasInBox = true




                } else if(!isPathClosed){

                    val localClosingRegion = closingRegion
                    if(localClosingRegion != null && localClosingRegion.contains(event.x.toInt(), event.y.toInt())){
                        isPathClosed = true
                        createPath()
                        for(view in ViewShapeHolder.getInstance().map.keys){
                            if(region.contains(view.leftX.toInt(), view.topY.toInt())){
                                viewsIn.add(view)
                                view.isSelected = true
                            }
                        }
                        for(link in ViewShapeHolder.getInstance().linkMap.keys){
                            var isAllPathIn = true
                            val linkPath = link.link?.path
                            if(linkPath != null) {
                                for (point in linkPath) {
                                    if (!region.contains(point.x.toInt(), point.y.toInt())) {
                                        isAllPathIn = false
                                    }
                                }
                            }
                            if(isAllPathIn){
                                linksIn.add(link)
                                link.isSelected = true
                            }
                        }

                    } else {
                        pathPoints.add(Coordinates(event.x.toDouble(), event.y.toDouble()))
                        createPath()
                    }

                    isPointAdded = true
                    invalidate()
                    requestLayout()
                }
                isDragged =false

                true
            }
            MotionEvent.ACTION_MOVE -> {//first_line.text = "ActionMove"


                if(downWasInBox) {

                    isDragged = true

                    val deltaX = event.rawX - oldFrameRawX
                    val deltaY = event.rawY - oldFrameRawY

                    for(point in pathPoints){
                        point.x += deltaX
                        point.y += deltaY
                    }

                    oldFrameRawX = event.rawX
                    oldFrameRawY = event.rawY

                    createPath()
                    invalidate()
                    requestLayout()

                    for(shape in viewsIn){
                        shape.leftX += deltaX
                        shape.topY += deltaY
                        shape.x += deltaX
                        shape.y += deltaY
                        shape.invalidate()
                        shape.requestLayout()
                    }
                    for(link in linksIn){
                        val linkPath = link.link?.path
                        if(linkPath != null) {
                            for(point in linkPath){
                                point.x += deltaX
                                point.y += deltaY
                            }
                        }
                        link.invalidate()
                        link.requestLayout()
                    }
                    true
                } else {
                    false
                }
            }
            MotionEvent.ACTION_UP -> {
                val activity: DrawingActivity = context as DrawingActivity

                if(!isPointAdded && !isDragged) {
                    parentView.removeView(this)
                    activity.selection_button.isChecked = false

                }
                invalidate()
                requestLayout()
                downWasInBox = false
                true
            }
            else -> {
                false
            }
        }
    }
}
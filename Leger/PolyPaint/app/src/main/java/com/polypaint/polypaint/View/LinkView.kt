package com.polypaint.polypaint.View

import android.content.Context
import android.graphics.*
import android.os.Bundle
import android.provider.ContactsContract
import android.util.Log
import android.view.MotionEvent
import android.view.View
import android.widget.ImageButton
import android.widget.RelativeLayout
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import androidx.fragment.app.DialogFragment
import com.github.nkzawa.socketio.client.Socket
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Enum.LinkTypes
import com.polypaint.polypaint.Fragment.EditLinkDialogFragment
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.Coordinates
import com.polypaint.polypaint.Model.Link
import com.polypaint.polypaint.R

class LinkView: View{
    private var socket: Socket? = null
    private val paint: Paint = Paint()
    var link: Link? = null
    var start: Coordinates = Coordinates(0.0,0.0)
    var end: Coordinates = Coordinates(0.0,0.0)
    var region: Region = Region()
    var rect :RectF = RectF()
    var multiplicityFrom: TextView? = null
    var multiplicityTo: TextView? = null
    var nameView: TextView? = null
    var dialog: DialogFragment? = null
    var editButton: ImageButton? = null
    var angleButtons: ArrayList<ImageButton> = ArrayList()
    var startAnchorButton : ImageButton? = null
    var endAnchorButton: ImageButton? = null
    var oldFrameRawX : Float = 0.0F
    var oldFrameRawY : Float = 0.0F
    val middlePoints: ArrayList<Coordinates> = ArrayList()
    var canvas: Canvas? = null
    var thickness: Float = 10f


    constructor(context: Context) : super(context) {
        initialise()
        val activity: AppCompatActivity = context as AppCompatActivity
        val app: PolyPaint = activity.application as PolyPaint
        this.socket = app.socket
    }

    private fun initialise(){
        paint.color = Color.BLACK
        paint.strokeWidth = 5f
        paint.style = Paint.Style.FILL_AND_STROKE
    }

    fun setLinkAndAnchors(link: Link){
        this.link = link
        this.start = link.path[0]
        this.end = link.path[link.path.size-1]
    }

    override fun onDraw(canvas: Canvas){
        this.canvas = canvas
        val linkId: String? =ViewShapeHolder.getInstance().linkMap[this]
        if(linkId!=null) {
            link = ViewShapeHolder.getInstance().canevas.findLink(linkId)
        }

        val parentView = this.parent as RelativeLayout
        if(multiplicityFrom != null){
            parentView.removeView(multiplicityFrom)
        }
        if(multiplicityTo != null){
            parentView.removeView(multiplicityTo)
        }
        if(nameView != null){
            parentView.removeView(nameView)
        }


        if( link?.style?.thickness != null) {
            thickness = link?.style?.thickness!!.toFloat()
        }

        val localLink: Link? = link
        if(localLink != null) {
            drawPath(paint,localLink.path, canvas)
        } else {
            val pathToDraw: ArrayList<Coordinates> = ArrayList()
            pathToDraw.add(start)
            pathToDraw.add(end)
            drawPath(paint, pathToDraw, canvas)
        }

        addTextViews(parentView)

        addButtons(parentView)


    }

    private fun addButtons(parentView: RelativeLayout){
        parentView.removeView(startAnchorButton)
        startAnchorButton = ImageButton(context)
        startAnchorButton?.setImageResource(R.drawable.ic_resize)
        startAnchorButton?.layoutParams?.height = 24
        startAnchorButton?.layoutParams?.width = 24
        startAnchorButton?.x = start.x.toFloat()
        startAnchorButton?.y = start.y.toFloat()
        startAnchorButton?.setOnTouchListener(onTouchListenerStartAnchorButton)
        startAnchorButton?.visibility = if(isSelected)View.VISIBLE else View.INVISIBLE
        parentView.addView(startAnchorButton)

        parentView.removeView(endAnchorButton)
        endAnchorButton = ImageButton(context)
        endAnchorButton?.setImageResource(R.drawable.ic_resize)
        endAnchorButton?.layoutParams?.height = 24
        endAnchorButton?.layoutParams?.width = 24
        endAnchorButton?.x = end.x.toFloat()
        endAnchorButton?.y = end.y.toFloat()
        endAnchorButton?.setOnTouchListener(onTouchListenerEndAnchorButton)
        endAnchorButton?.visibility = if(isSelected)View.VISIBLE else View.INVISIBLE
        parentView.addView(endAnchorButton)

        for(angleButton: ImageButton in angleButtons){
            parentView.removeView(angleButton)
        }
        angleButtons.clear()
        for(middlePoint: Coordinates in middlePoints){
            val angleButton = ImageButton(context)
            angleButton.setImageResource(R.drawable.ic_resize)
            angleButton.layoutParams?.height = 12
            angleButton.layoutParams?.width = 12
            angleButton.x = middlePoint.x.toFloat()
            angleButton.y = middlePoint.y.toFloat()
            angleButton.setOnTouchListener(onTouchListenerAngleButton)
            angleButton.visibility = if(isSelected)View.VISIBLE else View.INVISIBLE
            angleButtons.add(angleButton)
            parentView.addView(angleButton)
        }

        parentView.removeView(editButton)
        editButton = ImageButton(context)
        editButton?.setImageResource(R.drawable.ic_edit)
        editButton?.layoutParams?.height = 24
        editButton?.layoutParams?.width = 24
        editButton?.x = start.x.toFloat() + (end.x.toFloat() - start.x.toFloat()) /2  +15
        editButton?.y = start.y.toFloat() + (end.y.toFloat() - start.y.toFloat()) /2
        editButton?.setOnClickListener{
            showModal()
        }
        editButton?.visibility = if(isSelected)View.VISIBLE else View.INVISIBLE
        parentView.addView(editButton)
    }

    private fun drawPath(paint: Paint, pathToDraw: ArrayList<Coordinates>, canvas: Canvas){

        var angle : Double =0.0
        var angle2: Double =0.0
        var firstLineAngle: Double = 0.0
        middlePoints.clear()
        val path: Path = Path()

            var previousPoint: Coordinates = pathToDraw[0]
            for (point: Coordinates in pathToDraw) {
                if (point == start) {
                    continue
                }
                val middlePoint = Coordinates(previousPoint.x + (point.x - previousPoint.x) /2, previousPoint.y + (point.y - previousPoint.y) / 2)
                middlePoints.add(middlePoint)
                angle  = Math.atan2( (point.y - previousPoint.y), (point.x - previousPoint.x))
                angle2 =  angle -Math.PI/2
                if(previousPoint == start){
                    firstLineAngle = angle
                }
                path.moveTo(
                    previousPoint.x.toFloat() - thickness / 2 * Math.cos(angle2).toFloat(),
                    previousPoint.y.toFloat() - thickness / 2 * Math.sin(angle2).toFloat()
                )
                path.lineTo(
                    point.x.toFloat() - thickness / 2 * Math.cos(angle2).toFloat(),
                    point.y.toFloat() - thickness / 2 * Math.sin(angle2).toFloat()
                )
                path.lineTo(
                    point.x.toFloat() + thickness / 2 * Math.cos(angle2).toFloat(),
                    point.y.toFloat() + thickness / 2 * Math.sin(angle2).toFloat()
                )
                path.lineTo(
                    previousPoint.x.toFloat() + thickness / 2 * Math.cos(angle2).toFloat(),
                    previousPoint.y.toFloat() + thickness / 2 * Math.sin(angle2).toFloat()
                )
                path.close()
                previousPoint = point
            }

        canvas.drawPath(path, paint)


        rect = RectF()
        path.computeBounds(rect, true)
        Log.d("rect", rect.toString())
        region = Region()
        region.setPath(path, Region(rect.left.toInt(), rect.top.toInt(), rect.right.toInt(), rect.bottom.toInt()))



        when(link?.type){
            LinkTypes.AGGREGATION.ordinal->drawAggregation(canvas,angle, thickness)
            LinkTypes.COMPOSITION.ordinal->drawComposition(canvas,angle, thickness)
            LinkTypes.HERITAGE.ordinal -> drawHeritage(canvas, angle2, thickness)
            LinkTypes.ONE_WAY_ASSOCIATION.ordinal->drawArrowTip(canvas, angle, thickness, false)
            LinkTypes.TWO_WAY_ASSOCIATION.ordinal->{
                drawArrowTip(canvas,firstLineAngle,thickness, true)
                drawArrowTip(canvas, angle, thickness, false)
            }
        }
    }

    private fun drawComposition(canvas: Canvas, lineAngle: Double, thickness: Float){
        val diamondPaint = Paint()
        diamondPaint.color = paint.color
        diamondPaint.strokeWidth = thickness
        diamondPaint.style = Paint.Style.FILL_AND_STROKE
        drawDiamond(canvas, lineAngle, diamondPaint)
    }
    private fun drawAggregation(canvas: Canvas, lineAngle: Double, thickness: Float){
        val diamondPaint = Paint()
        diamondPaint.color = paint.color
        diamondPaint.strokeWidth = thickness
        diamondPaint.style = Paint.Style.STROKE
        drawDiamond(canvas, lineAngle, diamondPaint)
    }
    private fun drawDiamond(canvas: Canvas, lineAngle: Double, paint: Paint){
        val diamondLeftAngle: Double = lineAngle - Math.PI/4
        val diamondRightAngle: Double = lineAngle + Math.PI/4
        val leftPoint: Point = Point((end.x + 45 * Math.cos(diamondLeftAngle)).toInt(), (end.y+45 * Math.sin(diamondLeftAngle)).toInt() )
        val middlePoint: Point = Point((leftPoint.x + 45 * Math.cos(diamondRightAngle)).toInt(), (leftPoint.y+45 * Math.sin(diamondRightAngle)).toInt() )
        val rightPoint: Point = Point((end.x + 45 * Math.cos(diamondRightAngle)).toInt(), (end.y+45 * Math.sin(diamondRightAngle)).toInt() )
        val arrowPath: Path = Path()
        arrowPath.moveTo(end.x.toFloat(), end.y.toFloat())
        arrowPath.lineTo(leftPoint.x.toFloat(), leftPoint.y.toFloat())
        arrowPath.lineTo(middlePoint.x.toFloat(), middlePoint.y.toFloat())
        arrowPath.lineTo(rightPoint.x.toFloat(), rightPoint.y.toFloat())
        arrowPath.lineTo(end.x.toFloat(), end.y.toFloat())
        arrowPath.close()
        canvas.drawPath(arrowPath, paint)
    }

    private fun drawHeritage(canvas: Canvas, perpendicularAngle: Double, thickness: Float){
        val arrowPaint = Paint()
        arrowPaint.color = paint.color
        arrowPaint.strokeWidth = thickness
        arrowPaint.style = Paint.Style.STROKE
        val leftAngle: Double = perpendicularAngle + 2*Math.PI/3
       // val rightAngle: Double = perpendicularAngle + Math.PI/3
        val leftPoint: Point = Point((end.x + 30 * Math.cos(perpendicularAngle)).toInt(), (end.y+30 * Math.sin(perpendicularAngle)).toInt() )
        val middlePoint: Point = Point((leftPoint.x + 60 * Math.cos(leftAngle)).toInt(), (leftPoint.y+60 * Math.sin(leftAngle)).toInt() )
        val rightPoint: Point = Point((end.x - 30 * Math.cos(perpendicularAngle)).toInt(), (end.y - 30 * Math.sin(perpendicularAngle)).toInt() )
        val arrowPath: Path = Path()
        arrowPath.moveTo(end.x.toFloat(), end.y.toFloat())
        arrowPath.lineTo(leftPoint.x.toFloat(), leftPoint.y.toFloat())
        arrowPath.lineTo(middlePoint.x.toFloat(), middlePoint.y.toFloat())
        arrowPath.lineTo(rightPoint.x.toFloat(), rightPoint.y.toFloat())
        arrowPath.lineTo(end.x.toFloat(), end.y.toFloat())
        arrowPath.close()
        canvas.drawPath(arrowPath, arrowPaint)
    }

    private fun drawArrowTip(canvas: Canvas, lineAngle: Double, thickness: Float, isStart: Boolean){
        // draw a tip to the link

        val arrowPaint = Paint()
        arrowPaint.color = paint.color
        arrowPaint.strokeWidth = thickness
        arrowPaint.style = Paint.Style.STROKE

        var arrowLeftAngle: Double
        var arrowRightAngle: Double
        var middlePoint: Point
        if(isStart){
            arrowLeftAngle = lineAngle - Math.PI/4
            arrowRightAngle = lineAngle + Math.PI/4
            middlePoint = Point(start.x.toInt(), start.y.toInt())
        } else {
            arrowLeftAngle = lineAngle - 3*Math.PI/4
            arrowRightAngle = lineAngle + 3*Math.PI/4
            middlePoint = Point(end.x.toInt(), end.y.toInt())
        }

        val arrowPath: Path = Path()
        arrowPath.moveTo(middlePoint.x.toFloat(), middlePoint.y.toFloat())
        arrowPath.lineTo(middlePoint.x.toFloat() + 45 * Math.cos(arrowLeftAngle).toFloat(), middlePoint.y.toFloat()+45 * Math.sin(arrowLeftAngle).toFloat())
        arrowPath.moveTo(middlePoint.x.toFloat(), middlePoint.y.toFloat())
        arrowPath.lineTo(middlePoint.x.toFloat() + 45 * Math.cos(arrowRightAngle).toFloat(), middlePoint.y.toFloat()+45 * Math.sin(arrowRightAngle).toFloat())
        arrowPath.close()
        canvas.drawPath(arrowPath, arrowPaint)
    }

    private fun addTextViews(parentView: RelativeLayout){
        multiplicityFrom = TextView(context)
        multiplicityFrom?.x = start.x.toFloat() +15
        multiplicityFrom?.y = start.y.toFloat()
        multiplicityFrom?.setText(link?.from?.multiplicity)
        parentView.addView(multiplicityFrom)

        multiplicityTo = TextView(context)
        multiplicityTo?.x = end.x.toFloat() +15
        multiplicityTo?.y = end.y.toFloat()
        multiplicityTo?.setText(link?.to?.multiplicity)
        parentView.addView(multiplicityTo)

        nameView = TextView(context)
        nameView?.x =start.x.toFloat() + (end.x.toFloat() - start.x.toFloat()) /2  +15
        nameView?.y =start.y.toFloat() + (end.y.toFloat() - start.y.toFloat()) /2
        nameView?.setText(link?.name)
        parentView.addView(nameView)
    }

    override fun onDetachedFromWindow() {
        val parent = this.parent as RelativeLayout
        if(multiplicityFrom != null){
            parent.removeView(multiplicityFrom)
        }
        if(multiplicityTo != null){
            parent.removeView(multiplicityTo)
        }
        if(nameView != null){
            parent.removeView(nameView)
        }
        super.onDetachedFromWindow()
    }
    override fun onAttachedToWindow() {
        super.onAttachedToWindow()
        setOnTouchListener(onTouchListenerBody)
    }

    override fun setSelected(selected: Boolean) {
        val parentView = this.parent as RelativeLayout
        if(selected){

            paint.color = Color.BLUE

        }else{
            when(link?.style?.color){
                "BLACK"->paint.color = Color.BLACK
                "GREEN"->paint.color = Color.GREEN
                "YELLOW"->paint.color = Color.YELLOW
            }

        }
        return super.setSelected(selected)
    }

    private var onTouchListenerBody = View.OnTouchListener { v, event ->
        Log.d("event", event.x.toString()+" "+ event.y.toString())
        Log.d("region", region.bounds.toString())

        if(region.contains(event.x.toInt(), event.y.toInt())){
            val parentView = v.parent as RelativeLayout
            parentView.dispatchSetSelected(false)
            v.isSelected = true

            true
        }else{
            false
        }

    }

    private fun showModal(){
        var activity: AppCompatActivity = context as AppCompatActivity

        if(dialog == null) {
            dialog = EditLinkDialogFragment()

            var bundle: Bundle = Bundle()
            bundle.putString("linkId", ViewShapeHolder.getInstance().linkMap[this])
            dialog?.arguments = bundle

            Log.d("****", dialog?.arguments.toString())
            dialog?.show(activity.supportFragmentManager, "alllooooo")
        }
    }

    private var onTouchListenerAngleButton = View.OnTouchListener { v, event ->
        when (event.action) {
            MotionEvent.ACTION_DOWN -> {//first_line.text = "ActionDown"
                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY
            }
            MotionEvent.ACTION_MOVE -> {
                val index = angleButtons.indexOf(v)
                val deltaX = event.rawX - oldFrameRawX
                val deltaY = event.rawY - oldFrameRawY
//                val middlePoint = middlePoints[index]
//                val copyOfPath: ArrayList<Coordinates> = link?.path?.clone() as ArrayList<Coordinates>
//
//                angleButtons.indexOf(v)
//                copyOfPath.add(index + 1, Coordinates(middlePoint.x + deltaX, middlePoint.y + deltaY))
//                val previewPaint = paint
//                previewPaint.color = Color.LTGRAY
//                val localCanvas:Canvas? = canvas
//                if(localCanvas != null) {
//                    drawPath(previewPaint, copyOfPath, localCanvas)
//                }

                v.x += deltaX
                v.y += deltaY

//                val path:Path = Path()
//                val localLink: Link? = link
//                if(localLink != null) {
//                    path.moveTo(localLink.path.get(index).x.toFloat(), localLink.path.get(index).y.toFloat())
//                    path.lineTo(v.x, v.y)
//                    path.moveTo(localLink.path.get(index+1).x.toFloat(), localLink.path.get(index+1).y.toFloat())
//                }
//                val previewPaint = Paint()
//                previewPaint.style = Paint.Style.STROKE
//                previewPaint.strokeWidth = thickness
//                previewPaint.color = Color.LTGRAY
//
//                canvas?.drawPath(path, previewPaint)


                oldFrameRawY = event.rawY
                oldFrameRawX = event.rawX

            }
            MotionEvent.ACTION_UP -> {
                val index = angleButtons.indexOf(v)
                val deltaX = event.rawX - oldFrameRawX
                val deltaY = event.rawY - oldFrameRawY
                val middlePoint = middlePoints[index]
                angleButtons.indexOf(v)
                link?.path?.add(index + 1, Coordinates(v.x.toDouble(), v.y.toDouble() ))
                invalidate()
                requestLayout()
            }
        }

        true
    }

    private var onTouchListenerStartAnchorButton = View.OnTouchListener { v, event ->
        when (event.action) {
            MotionEvent.ACTION_DOWN -> {//first_line.text = "ActionDown"
                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY
            }
            MotionEvent.ACTION_MOVE -> {
                val deltaX = event.rawX - oldFrameRawX
                val deltaY = event.rawY - oldFrameRawY
                v.x += deltaX
                v.y += deltaY

                oldFrameRawY = event.rawY
                oldFrameRawX = event.rawX

            }
            MotionEvent.ACTION_UP -> {
                link?.path?.first()?.x = v.x.toDouble()
                link?.path?.first()?.y = v.y.toDouble()
                invalidate()
                requestLayout()
            }
        }

        true
    }
    private var onTouchListenerEndAnchorButton = View.OnTouchListener { v, event ->
        when (event.action) {
            MotionEvent.ACTION_DOWN -> {//first_line.text = "ActionDown"
                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY
            }
            MotionEvent.ACTION_MOVE -> {
                val deltaX = event.rawX - oldFrameRawX
                val deltaY = event.rawY - oldFrameRawY
                v.x += deltaX
                v.y += deltaY

                oldFrameRawY = event.rawY
                oldFrameRawX = event.rawX

            }
            MotionEvent.ACTION_UP -> {
                link?.path?.last()?.x = v.x.toDouble()
                link?.path?.last()?.y = v.y.toDouble()
                invalidate()
                requestLayout()
            }
        }

        true
    }
//

//    private fun emitSelection(){
//        val response: String = this.createResponseObject()
//
//        if(response !="") {
//            Log.d("emitingSelection", response)
//            socket?.emit(SocketConstants.SELECT_FORMS, response)
//        }
//    }
//
//    private fun createResponseObject(): String{
//        val basicShape: BasicShape? = ViewShapeHolder.getInstance().canevas.findShape(ViewShapeHolder.getInstance().map.getValue(this))
//
//        var obj: String =""
//        if(basicShape !=null) {
//            val gson = Gson()
//            val response: DrawingActivity.Response =
//                DrawingActivity.Response(UserHolder.getInstance().username, basicShape)
//            obj = gson.toJson(response)
//        }
//        return obj
//    }

}
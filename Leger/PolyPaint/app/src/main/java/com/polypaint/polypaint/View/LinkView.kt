package com.polypaint.polypaint.View

import android.content.Context
import android.graphics.*
import android.os.Bundle
import android.util.Log
import android.view.View
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
        val linkId: String? =ViewShapeHolder.getInstance().linkMap[this]
        if(linkId!=null) {
            link = ViewShapeHolder.getInstance().canevas.findLink(linkId)
        }

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

        val angle : Double = Math.atan2( (end.y - start.y), (end.x - start.x))
        val angle2: Double =  angle -Math.PI/2

        var thickness: Float = 10f
        if( link?.style?.thickness != null) {
            thickness = link?.style?.thickness!!.toFloat()
        }

        val path: Path = Path()
        path.moveTo(start.x.toFloat() - thickness/2 * Math.cos(angle2).toFloat(), start.y.toFloat()-thickness/2 * Math.sin(angle2).toFloat())
        path.lineTo(end.x.toFloat() - thickness/2 * Math.cos(angle2).toFloat(), end.y.toFloat()-thickness/2 * Math.sin(angle2).toFloat())
        path.lineTo(end.x.toFloat() + thickness/2 * Math.cos(angle2).toFloat(), end.y.toFloat()+thickness/2 * Math.sin(angle2).toFloat())
        path.lineTo(start.x.toFloat() + thickness/2 * Math.cos(angle2).toFloat(), start.y.toFloat()+thickness/2 * Math.sin(angle2).toFloat())
        path.lineTo(start.x.toFloat() - thickness/2 * Math.cos(angle2).toFloat(), start.y.toFloat()-thickness/2 * Math.sin(angle2).toFloat())
        path.close()
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
                drawArrowTip(canvas,angle,thickness, true)
                drawArrowTip(canvas, angle, thickness, false)
            }
        }

        addTextViews(parent)

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

    private fun addTextViews(parent: RelativeLayout){
        multiplicityFrom = TextView(context)
        multiplicityFrom?.x = start.x.toFloat() +15
        multiplicityFrom?.y = start.y.toFloat()
        multiplicityFrom?.setText(link?.from?.multiplicity)
        parent.addView(multiplicityFrom)

        multiplicityTo = TextView(context)
        multiplicityTo?.x = end.x.toFloat() +15
        multiplicityTo?.y = end.y.toFloat()
        multiplicityTo?.setText(link?.to?.multiplicity)
        parent.addView(multiplicityTo)

        nameView = TextView(context)
        nameView?.x =start.x.toFloat() + (end.x.toFloat() - start.x.toFloat()) /2  +15
        nameView?.y =start.y.toFloat() + (end.y.toFloat() - start.y.toFloat()) /2
        nameView?.setText(link?.name)
        parent.addView(nameView)
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

            var activity: AppCompatActivity = context as AppCompatActivity

            if(dialog == null) {
                dialog = EditLinkDialogFragment()

                var bundle: Bundle = Bundle()
                bundle.putString("linkId", ViewShapeHolder.getInstance().linkMap[this])
                dialog?.arguments = bundle

                Log.d("****", dialog?.arguments.toString())
                dialog?.show(activity.supportFragmentManager, "alllooooo")
            }

            true
        }else{
            false
        }

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
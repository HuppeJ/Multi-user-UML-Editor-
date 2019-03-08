package com.polypaint.polypaint.View

import android.content.Context
import android.graphics.*
import android.util.Log
import android.view.View
import android.widget.RelativeLayout
import androidx.appcompat.app.AppCompatActivity
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson
import com.polypaint.polypaint.Activity.DrawingActivity
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Holder.UserHolder
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.BasicShape
import com.polypaint.polypaint.Model.Coordinates
import com.polypaint.polypaint.R
import com.polypaint.polypaint.Socket.SocketConstants
import kotlinx.android.synthetic.main.basic_element.view.*

class LinkView: View{
    private var socket: Socket? = null
    private val paint: Paint = Paint()
    var start: Coordinates = Coordinates(0.0,0.0)
    var end: Coordinates = Coordinates(0.0,0.0)
    var region: Region = Region()
    var rect :RectF = RectF()

    constructor(context: Context) : super(context) {
        initialise()
        val activity: AppCompatActivity = context as AppCompatActivity
        val app: PolyPaint = activity.application as PolyPaint
        this.socket = app.socket
    }

    private fun initialise(){
        paint.color = Color.BLACK
        paint.strokeWidth = 5f
        paint.style = Paint.Style.STROKE
    }

    override fun onDraw(canvas: Canvas){
        val path: Path = Path()
        path.moveTo(start.x.toFloat(), start.y.toFloat())
        path.lineTo(end.x.toFloat(), end.y.toFloat())
//        path.lineTo(end.x.toFloat() + 10f, end.y.toFloat()+10f)
//        path.lineTo(start.x.toFloat() + 10f, start.y.toFloat()+10f)
//        path.lineTo(start.x.toFloat(), start.y.toFloat())
        path.close()
        canvas.drawPath(path, paint)



        rect = RectF()
        path.computeBounds(rect, true)
        Log.d("rect", rect.toString())
        region = Region()
        region.setPath(path, Region(rect.left.toInt(), rect.top.toInt(), rect.right.toInt(), rect.bottom.toInt()))
        Log.d("contains", region.contains(0,0).toString())
        Log.d("regionBound", region.bounds.toString())
        Log.d("regionBoundaryPath", region.boundaryPath.isEmpty.toString())
        //canvas.drawPath(region.boundaryPath, paint)
       // canvas.drawLine(start.x.toFloat(), start.y.toFloat(), end.x.toFloat(), end.y.toFloat(), paint)
    }

    override fun onAttachedToWindow() {
        super.onAttachedToWindow()
        setOnTouchListener(onTouchListenerBody)
    }

    override fun setSelected(selected: Boolean) {
        if(selected){
            paint.color = Color.BLUE
        }else{
            paint.color = Color.BLACK
        }
        return super.setSelected(selected)
    }

    private var onTouchListenerBody = View.OnTouchListener { v, event ->
        Log.d("event", event.x.toString()+" "+ event.y.toString())
        Log.d("region", region.bounds.toString())

        if(rect.contains(event.x, event.y)){
            val parentView = v.parent as RelativeLayout
            parentView.dispatchSetSelected(false)
            v.isSelected = true
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
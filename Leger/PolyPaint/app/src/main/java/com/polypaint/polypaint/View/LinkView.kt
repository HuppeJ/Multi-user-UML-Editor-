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
        this.end = link.path[link.path.size]
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

        var thickness: Int = 10
        if( link?.style?.thickness != null) {
            thickness = link?.style?.thickness!!.toInt()
        }

        val path: Path = Path()
        path.moveTo(start.x.toFloat(), start.y.toFloat())
        path.lineTo(end.x.toFloat(), end.y.toFloat())
        path.lineTo(end.x.toFloat() + thickness.toFloat() * Math.cos(angle2).toFloat(), end.y.toFloat()+thickness.toFloat() * Math.sin(angle2).toFloat())
        path.lineTo(start.x.toFloat() + thickness.toFloat() * Math.cos(angle2).toFloat(), start.y.toFloat()+thickness.toFloat() * Math.sin(angle2).toFloat())
        path.lineTo(start.x.toFloat(), start.y.toFloat())
        path.close()
        canvas.drawPath(path, paint)

        rect = RectF()
        path.computeBounds(rect, true)
        Log.d("rect", rect.toString())
        region = Region()
        region.setPath(path, Region(rect.left.toInt(), rect.top.toInt(), rect.right.toInt(), rect.bottom.toInt()))


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
        nameView?.x =start.x.toFloat() + Math.abs(end.x.toFloat() - start.x.toFloat()) /2  +15
        nameView?.y =start.y.toFloat() + Math.abs(end.y.toFloat() - start.y.toFloat()) /2
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
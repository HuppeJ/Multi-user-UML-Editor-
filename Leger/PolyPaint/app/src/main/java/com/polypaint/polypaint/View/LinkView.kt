package com.polypaint.polypaint.View

import android.content.Context
import android.graphics.*
import android.os.Bundle
import android.util.Log
import android.view.MotionEvent
import android.view.View
import android.view.ViewGroup
import android.widget.ImageButton
import android.widget.RelativeLayout
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import androidx.fragment.app.DialogFragment
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Enum.AnchorPoints
import com.polypaint.polypaint.Enum.BorderTypes
import com.polypaint.polypaint.Enum.LinkTypes
import com.polypaint.polypaint.Fragment.EditLinkDialogFragment
import com.polypaint.polypaint.Holder.UserHolder
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.*
import com.polypaint.polypaint.R
import com.polypaint.polypaint.Socket.SocketConstants
import com.polypaint.polypaint.SocketReceptionModel.FormsUpdateEvent
import com.polypaint.polypaint.SocketReceptionModel.LinksUpdateEvent
import kotlinx.android.synthetic.main.basic_element.view.*

class LinkView: View{
    private var socket: Socket? = null
    private val paint: Paint = Paint()

    var link: Link? = null
    var start: Coordinates = Coordinates(0.0,0.0)
    var end: Coordinates = Coordinates(0.0,0.0)

    var isSelectedByOther: Boolean = false

    var region: Region = Region()
    var rect :RectF = RectF()

    var dialog: DialogFragment? = null
    var multiplicityFrom: TextView? = null
    var multiplicityTo: TextView? = null
    var nameView: TextView? = null
    var editButton: ImageButton? = null
    var angleButtons: ArrayList<ImageButton> = ArrayList()
    var startAnchorButton : ImageButton? = null
    var endAnchorButton: ImageButton? = null
    var deleteButton: ImageButton? = null

    var oldFrameRawX : Float = 0.0F
    var oldFrameRawY : Float = 0.0F
    val middlePoints: ArrayList<Coordinates> = ArrayList()
    var canvas: Canvas? = null
    var thickness: Float = 10f

    var previewLinkView: LinkView? = null
    var oldPreviewLink: LinkView? = null

    var boundingBox : LinkBoundingBoxView? = null

    var pointerFinger1 : Int = -1
    var pointerFinger2 : Int = -1

    var fingersCoords : Array<Coordinates> = Array(4) { Coordinates(0.0,0.0) }
    var isButtonVisible: Boolean = false

    fun setIsSelectedByOther(isSelectedByOther: Boolean){
        this.isSelectedByOther = isSelectedByOther
        if(isSelectedByOther){
            paint.color = Color.RED
        } else {
            setPaintColorWithLinkStyle()
        }
    }
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
        val layoutParams = ViewGroup.LayoutParams(100, 100)
        parentView.removeView(deleteButton)
        deleteButton = ImageButton(context)
        deleteButton?.setImageResource(R.drawable.ic_delete)
        deleteButton?.layoutParams=layoutParams
        deleteButton?.x = start.x.toFloat() + 50
        deleteButton?.y = start.y.toFloat() - 50
        deleteButton?.setOnClickListener{
            deleteLink()
        }
        deleteButton?.visibility = if(isButtonVisible)View.VISIBLE else View.INVISIBLE
        parentView.addView(deleteButton)

        parentView.removeView(startAnchorButton)
        startAnchorButton = ImageButton(context)
        startAnchorButton?.setImageResource(R.drawable.ic_resize)
        startAnchorButton?.layoutParams = layoutParams
        startAnchorButton?.x = start.x.toFloat() - startAnchorButton?.layoutParams?.width!! / 2
        startAnchorButton?.y = start.y.toFloat() - startAnchorButton?.layoutParams?.height!! / 2
        startAnchorButton?.setOnTouchListener(onTouchListenerStartAnchorButton)
        startAnchorButton?.visibility = if (isButtonVisible) View.VISIBLE else View.INVISIBLE
        parentView.addView(startAnchorButton)

        parentView.removeView(endAnchorButton)
        endAnchorButton = ImageButton(context)
        endAnchorButton?.setImageResource(R.drawable.ic_resize)
        endAnchorButton?.layoutParams = layoutParams
        endAnchorButton?.x = end.x.toFloat() - endAnchorButton?.layoutParams?.width!! / 2
        endAnchorButton?.y = end.y.toFloat() - endAnchorButton?.layoutParams?.height!! / 2
        endAnchorButton?.setOnTouchListener(onTouchListenerEndAnchorButton)
        endAnchorButton?.visibility = if(isButtonVisible)View.VISIBLE else View.INVISIBLE
        parentView.addView(endAnchorButton)

        for(angleButton: ImageButton in angleButtons){
            parentView.removeView(angleButton)
        }
        angleButtons.clear()
        for(middlePoint: Coordinates in middlePoints){
            val angleButton = ImageButton(context)
            angleButton.setImageResource(R.drawable.ic_resize)
            angleButton.layoutParams = layoutParams
            angleButton.x = middlePoint.x.toFloat() - angleButton.layoutParams?.width!! / 2
            angleButton.y = middlePoint.y.toFloat() - angleButton.layoutParams?.height!! / 2
            angleButton.setOnTouchListener(onTouchListenerAngleButton)
            angleButton.visibility = if(isButtonVisible)View.VISIBLE else View.INVISIBLE
            angleButtons.add(angleButton)
            parentView.addView(angleButton)
        }

        parentView.removeView(editButton)
        editButton = ImageButton(context)
        editButton?.setImageResource(R.drawable.ic_edit)
        editButton?.layoutParams = layoutParams
        var point: Coordinates =Coordinates(0.0,0.0)
        val localLink = link
        if(localLink != null) {
            if (localLink.path.size > 1 && localLink.path.size % 2 != 0) {
                point = localLink.path[(localLink.path.size - 1) / 2]
            } else {
                val firstPoint = localLink.path[(localLink.path.size - 1 )/ 2]
                val secondPoint = localLink.path[(localLink.path.size - 1 )/ 2 + 1]
                point = Coordinates(
                    firstPoint.x + (secondPoint.x - firstPoint.x) / 2.0 + 40,
                    firstPoint.y + (secondPoint.y - firstPoint.y) / 2.0
                )
            }
        }
        editButton?.x = point.x.toFloat()
        editButton?.y = point.y.toFloat()
        editButton?.setOnClickListener{
            showModal()
        }
        editButton?.visibility = if(isButtonVisible)View.VISIBLE else View.INVISIBLE
        parentView.addView(editButton)
    }

    private fun drawPath(paint: Paint, pathToDraw: ArrayList<Coordinates>, canvas: Canvas){

        var angle : Double =0.0
        var angle2: Double =0.0
        var firstLineAngle: Double = 0.0
        middlePoints.clear()
        val path: Path = Path()
        val linePath: Path = Path()

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
                previousPoint.x.toFloat() - 20 * Math.cos(angle2).toFloat(),
                previousPoint.y.toFloat() - 20 * Math.sin(angle2).toFloat()
            )
            path.lineTo(
                point.x.toFloat() - 20  * Math.cos(angle2).toFloat(),
                point.y.toFloat() - 20  * Math.sin(angle2).toFloat()
            )
            path.lineTo(
                point.x.toFloat() + 20 * Math.cos(angle2).toFloat(),
                point.y.toFloat() + 20 * Math.sin(angle2).toFloat()
            )
            path.lineTo(
                previousPoint.x.toFloat() + 20 * Math.cos(angle2).toFloat(),
                previousPoint.y.toFloat() + 20 * Math.sin(angle2).toFloat()
            )
            path.close()

            linePath.moveTo(
                previousPoint.x.toFloat(),
                previousPoint.y.toFloat()
            )
            linePath.lineTo(
                point.x.toFloat(),
                point.y.toFloat()
            )
            previousPoint = point
        }

        if(link?.style?.type == BorderTypes.DOTED.ordinal){
            val array: FloatArray = FloatArray(2)
            array[0] = 10f
            array[1] = 5f
            paint.pathEffect = DashPathEffect(array, 1f)
        }
        paint.strokeWidth = thickness


        canvas.drawPath(linePath, paint)


        rect = RectF()
        path.computeBounds(rect, true)
        Log.d("rect", rect.toString())
        region = Region()
        region.setPath(path, Region(rect.left.toInt(), rect.top.toInt(), rect.right.toInt(), rect.bottom.toInt()))


        val parentView = this.parent as RelativeLayout

        if(this.isSelected) {
            boundingBox?.setVisible(true)
            boundingBox?.rect = rect
            boundingBox?.invalidate()
            boundingBox?.requestLayout()
        } else {
            boundingBox?.setVisible(false)
        }

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
        var point: Coordinates =Coordinates(0.0,0.0)
        val localLink = link
        if(localLink != null) {
            if (localLink.path.size > 1 && localLink.path.size % 2 != 0) {
                point = localLink.path[(localLink.path.size - 1) / 2]
            } else {
                val firstPoint = localLink.path[(localLink.path.size - 1 )/ 2]
                val secondPoint = localLink.path[(localLink.path.size - 1 )/ 2 + 1]
                point = Coordinates(
                    firstPoint.x + (secondPoint.x - firstPoint.x) / 2.0,
                    firstPoint.y + (secondPoint.y - firstPoint.y) / 2.0
                )
            }
        }
        nameView?.x =point.x.toFloat() + 40
        nameView?.y =point.y.toFloat() - 80
        nameView?.setText(link?.name)
        parentView.addView(nameView)
    }

    fun deleteLink(){
        emitDelete()
        ViewShapeHolder.getInstance().stackDrawingElementCreatedId.remove(link?.id)

        val fromId = link?.from?.formId
        if(fromId != null && fromId != ""){
            val fromShape = ViewShapeHolder.getInstance().canevas.findShape(fromId)
            fromShape?.linksFrom?.remove(link?.id)
        }
        link?.from = AnchorPoint()
        val toId = link?.to?.formId
        if(toId != null && toId != ""){
            val toShape = ViewShapeHolder.getInstance().canevas.findShape(toId)
            toShape?.linksTo?.remove(link?.id)
        }
        link?.to = AnchorPoint()

        removeButtonsAndTexts()
        val localLink: Link? = link
        if(localLink != null){
            ViewShapeHolder.getInstance().remove(localLink)
        }
        val parent = this.parent as RelativeLayout
        parent.removeView(this)

    }

    fun hideButtons(){
        isButtonVisible = false
    }

    private fun removeButtonsAndTexts(){
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
        if(deleteButton != null){
            parent.removeView(deleteButton)
        }
        if(editButton != null){
            parent.removeView(editButton)
        }
        if(startAnchorButton != null){
            parent.removeView(startAnchorButton)
        }
        if(endAnchorButton != null){
            parent.removeView(endAnchorButton)
        }
        if(boundingBox != null){
            boundingBox?.delete()
        }
        for(angleButton in angleButtons) {
            if (angleButton != null) {
                parent.removeView(angleButton)
            }
        }
    }
    override fun onAttachedToWindow() {
        super.onAttachedToWindow()
        setOnTouchListener(onTouchListenerBody)
        val parentView = this.parent as RelativeLayout
        boundingBox = LinkBoundingBoxView(context, this)
        boundingBox?.setVisible(false)
        parentView.addView(boundingBox)
    }

    override fun setSelected(selected: Boolean) {
        if(this.isSelected && !selected){
            emitDeselection()
        }
        if(selected){
            isButtonVisible = true
            emitSelection()
            paint.color = Color.BLUE
        }else if(!this.isSelectedByOther){
            setPaintColorWithLinkStyle()
            isButtonVisible = false
        } else {
            isButtonVisible = false
        }
        return super.setSelected(selected)
    }

    fun setPaintColorWithLinkStyle(){
        paint.color = Color.parseColor(link?.style?.color)
//        when(link?.style?.color){
//            "BLACK"->paint.color = Color.BLACK
//            "GREEN"->paint.color = Color.GREEN
//            "YELLOW"->paint.color = Color.YELLOW
//            else -> paint.color = Color.BLACK
//        }
    }

    private var onTouchListenerBody = View.OnTouchListener { v, event ->
        Log.d("event", event.x.toString()+" "+ event.y.toString())
        Log.d("region", region.bounds.toString())

          val parentView = v.parent as RelativeLayout

        when (event.action) {
            MotionEvent.ACTION_DOWN -> {
                if(region.contains(event.x.toInt(), event.y.toInt())){
                    if(!this.isSelectedByOther) {

                        parentView.dispatchSetSelected(false)

                        v.isSelected = true
                    }

                    true
                }else{
                    false
                }
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

    fun moveLink(deltaX: Double, deltaY: Double){
        val localLink = link
        if(localLink != null) {
            for (point in localLink.path) {
                if((point == localLink.path.first() && localLink.from.formId != "")
                    ||(point == localLink.path.last() && localLink.to.formId != "")) {
                    continue
                }
                point.x += deltaX
                point.y += deltaY
            }
        }
        invalidate()
        requestLayout()
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
        val parentView = this.parent as RelativeLayout
        if(oldPreviewLink != null){
            parentView.removeView(oldPreviewLink)
        }
//        if(boundingBox != null){
//            parentView.removeView(boundingBox)
//        }
        when (event.action) {
            MotionEvent.ACTION_DOWN -> {//first_line.text = "ActionDown"
                previewLinkView = LinkView(context)
                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY
//                boundingBox = LinkBoundingBoxView(context, rect)
//                parentView.addView(boundingBox)
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


                val localLink: Link? = link
                val previewLink = Link("","", AnchorPoint(), AnchorPoint(), 0, LinkStyle("BLACK",10,0), ArrayList())

                if(localLink != null){
                    previewLink.path.add(localLink.path[index])
                    previewLink.path.add(Coordinates(v.x.toDouble() + v.layoutParams.width / 2, v.y.toDouble() + v.layoutParams.height / 2))
                    previewLink.path.add(localLink.path[index+1])
                    previewLinkView?.setLinkAndAnchors(previewLink)
                }

                oldPreviewLink = previewLinkView
                parentView.addView(previewLinkView)

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
                link?.path?.add(index + 1, Coordinates(v.x.toDouble() + v.layoutParams.width / 2, v.y.toDouble() + v.layoutParams.height / 2))


                emitUpdate()
                invalidate()
                requestLayout()

//                boundingBox = LinkBoundingBoxView(context, rect)
//                parentView.addView(boundingBox)
            }
        }

        true
    }

    private var onTouchListenerStartAnchorButton = View.OnTouchListener { v, event ->
        val parentView = this.parent as RelativeLayout
        if(oldPreviewLink != null){
            parentView.removeView(oldPreviewLink)
        }
//        if(boundingBox != null){
//            parentView.removeView(boundingBox)
//        }
        when (event.action) {
            MotionEvent.ACTION_DOWN -> {
                previewLinkView = LinkView(context)

                for(basicView: BasicElementView in ViewShapeHolder.getInstance().map.keys) {
                    if (!basicView.isSelectedByOther) {
                        basicView.setAnchorsVisible(true)
                    }
                }

                oldFrameRawX = event.rawX
                oldFrameRawY = event.rawY
                v.visibility = View.INVISIBLE
            }
            MotionEvent.ACTION_MOVE -> {
                val deltaX = event.rawX - oldFrameRawX
                val deltaY = event.rawY - oldFrameRawY
                v.x += deltaX
                v.y += deltaY

                colorAnchorOnHover((v.x + v.layoutParams.width / 2.0).toInt(), (v.y + v.layoutParams.height / 2).toInt())

                oldFrameRawY = event.rawY
                oldFrameRawX = event.rawX

                val localLink: Link? = link
                if(localLink != null){
                    previewLinkView?.end = localLink.path[1]
                }
                previewLinkView?.start = Coordinates(v.x.toDouble() + v.layoutParams.width / 2, v.y.toDouble() + v.layoutParams.height / 2)
                oldPreviewLink = previewLinkView
                parentView.addView(previewLinkView)
            }
            MotionEvent.ACTION_UP -> {
                v.visibility = View.VISIBLE
                for(basicView: BasicElementView in ViewShapeHolder.getInstance().map.keys) {
                    basicView.setAnchorsVisible(false)
                }
                var anchorPointStart: AnchorPoint = AnchorPoint()
                var otherBasicView: BasicElementView? = null
                val x = (v.x.toDouble() + v.layoutParams.width / 2).toInt()
                val y = (v.y.toDouble() + v.layoutParams.height / 2).toInt()
                for(basicView: BasicElementView in ViewShapeHolder.getInstance().map.keys){
                    if(!basicView.isSelectedByOther) {
                        val  basicShapeId: String? = ViewShapeHolder.getInstance().map[basicView]
                        if (basicView.isViewInBounds(basicView.anchorPoint0, x, y)) {
                            otherBasicView = basicView
                            if(basicShapeId != null){
                                anchorPointStart = AnchorPoint(basicShapeId, AnchorPoints.TOP.ordinal)
                                basicView.anchorPoint0.setBackgroundColor(Color.TRANSPARENT)
                                basicView.anchorPoint0.invalidate()
                                basicView.anchorPoint0.requestLayout()
                            }
                            break
                        } else if (basicView.isViewInBounds(basicView.anchorPoint1, x, y)) {
                            otherBasicView = basicView
                            if(basicShapeId != null){
                                anchorPointStart = AnchorPoint(basicShapeId, AnchorPoints.RIGHT.ordinal)
                                basicView.anchorPoint1.setBackgroundColor(Color.TRANSPARENT)
                                basicView.anchorPoint1.invalidate()
                                basicView.anchorPoint1.requestLayout()
                            }
                            break
                        } else if (basicView.isViewInBounds(basicView.anchorPoint2, x, y)) {
                            otherBasicView = basicView
                            if(basicShapeId != null){
                                anchorPointStart = AnchorPoint(basicShapeId, AnchorPoints.BOTTOM.ordinal)
                                basicView.anchorPoint2.setBackgroundColor(Color.TRANSPARENT)
                                basicView.anchorPoint2.invalidate()
                                basicView.anchorPoint2.requestLayout()
                            }
                            break
                        } else if (basicView.isViewInBounds(basicView.anchorPoint3, x, y)) {
                            otherBasicView = basicView
                            if(basicShapeId != null){
                                anchorPointStart = AnchorPoint(basicShapeId, AnchorPoints.LEFT.ordinal)
                                basicView.anchorPoint3.setBackgroundColor(Color.TRANSPARENT)
                                basicView.anchorPoint3.invalidate()
                                basicView.anchorPoint3.requestLayout()
                            }
                            break
                        }
                    }
                }

                val formsToUpdate = ArrayList<BasicShape>()
                val formId = link?.from?.formId
                if(formId != null && formId != ""){
                    val shape = ViewShapeHolder.getInstance().canevas.findShape(formId)
                    if(shape != null){
                        if(formId != anchorPointStart.formId){
                            shape.linksFrom.remove(link?.id)

                            formsToUpdate.add(shape)
                        }
                    }
                }
                link?.from?.formId = anchorPointStart.formId
                link?.from?.anchor = anchorPointStart.anchor

                if(formId != anchorPointStart.formId) {
                    val otherBasicViewId = ViewShapeHolder.getInstance().map[otherBasicView]
                    val canevas: Canevas = ViewShapeHolder.getInstance().canevas
                    if (otherBasicViewId != null) {
                        val otherShape: BasicShape? = canevas.findShape(otherBasicViewId)
                        if (otherShape != null) {
                            otherShape.linksFrom.add(link?.id)
                            formsToUpdate.add(otherShape)
                        }
                    }
                }
                Log.d("AnchorFormId","Forms id" + link!!.from.formId)
                link?.path?.first()?.x = v.x.toDouble() + v.layoutParams.width / 2
                link?.path?.first()?.y = v.y.toDouble() + v.layoutParams.height / 2



                emitFormsUpdate(formsToUpdate)
                emitUpdate()
                invalidate()
                requestLayout()

//                boundingBox = LinkBoundingBoxView(context, rect)
//                parentView.addView(boundingBox)
            }
        }

        true
    }

    private var onTouchListenerEndAnchorButton = View.OnTouchListener { v, event ->
        val parentView = this.parent as RelativeLayout
        if(oldPreviewLink != null){
            parentView.removeView(oldPreviewLink)
        }
//        if(boundingBox != null){
//            parentView.removeView(boundingBox)
//        }
        when (event.action) {

            MotionEvent.ACTION_DOWN -> {
                previewLinkView = LinkView(context)

                v.visibility = View.INVISIBLE
                for(basicView: BasicElementView in ViewShapeHolder.getInstance().map.keys) {
                    if (!basicView.isSelectedByOther) {
                        basicView.setAnchorsVisible(true)
                    }
                }

//                boundingBox = LinkBoundingBoxView(context, rect)
//                parentView.addView(boundingBox)

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

                colorAnchorOnHover(v.x.toInt(), v.y.toInt())

                val localLink: Link? = link
                if(localLink != null){
                    previewLinkView?.start = localLink.path[localLink.path.size - 2]
                }
                previewLinkView?.end = Coordinates(v.x.toDouble() + v.layoutParams.width / 2, v.y.toDouble() + v.layoutParams.height / 2)
                oldPreviewLink = previewLinkView
                parentView.addView(previewLinkView)

            }
            MotionEvent.ACTION_UP -> {
                v.visibility = View.VISIBLE
                for(basicView: BasicElementView in ViewShapeHolder.getInstance().map.keys) {
                    basicView.setAnchorsVisible(false)
                }

                var anchorPointEnd: AnchorPoint = AnchorPoint()
                var otherBasicView: BasicElementView? = null
                val x = (v.x.toDouble() + v.layoutParams.width / 2).toInt()
                val y = (v.y.toDouble() + v.layoutParams.height / 2).toInt()
                for(basicView: BasicElementView in ViewShapeHolder.getInstance().map.keys){
                    if(!basicView.isSelectedByOther) {
                        val  basicShapeId: String? = ViewShapeHolder.getInstance().map[basicView]
                        if (basicView.isViewInBounds(basicView.anchorPoint0, x, y)) {
                            otherBasicView = basicView
                            if(basicShapeId != null){
                                anchorPointEnd = AnchorPoint(basicShapeId, AnchorPoints.TOP.ordinal)
                                basicView.anchorPoint0.setBackgroundColor(Color.TRANSPARENT)
                                basicView.anchorPoint0.invalidate()
                                basicView.anchorPoint0.requestLayout()
                            }
                            break
                        } else if (basicView.isViewInBounds(basicView.anchorPoint1, x, y)) {
                            otherBasicView = basicView
                            if(basicShapeId != null){
                                anchorPointEnd = AnchorPoint(basicShapeId, AnchorPoints.RIGHT.ordinal)
                                basicView.anchorPoint1.setBackgroundColor(Color.TRANSPARENT)
                                basicView.anchorPoint1.invalidate()
                                basicView.anchorPoint1.requestLayout()
                            }
                            break
                        } else if (basicView.isViewInBounds(basicView.anchorPoint2, x, y)) {
                            otherBasicView = basicView
                            if(basicShapeId != null){
                                anchorPointEnd = AnchorPoint(basicShapeId, AnchorPoints.BOTTOM.ordinal)
                                basicView.anchorPoint2.setBackgroundColor(Color.TRANSPARENT)
                                basicView.anchorPoint2.invalidate()
                                basicView.anchorPoint2.requestLayout()
                            }
                            break
                        } else if (basicView.isViewInBounds(basicView.anchorPoint3, x, y)) {
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

                val formsToUpdate = ArrayList<BasicShape>()
                val formId = link?.to?.formId
                if(formId != null && formId != ""){
                    val shape = ViewShapeHolder.getInstance().canevas.findShape(formId)
                    if(shape != null){
                        if(formId != anchorPointEnd.formId) {
                            shape.linksTo.remove(link?.id)
                            formsToUpdate.add(shape)
                        }
                    }
                }
                link?.to?.formId = anchorPointEnd.formId
                link?.to?.anchor = anchorPointEnd.anchor

                if(formId != anchorPointEnd.formId) {
                    val otherBasicViewId = ViewShapeHolder.getInstance().map[otherBasicView]
                    val canevas: Canevas = ViewShapeHolder.getInstance().canevas
                    if (otherBasicViewId != null) {
                        val otherShape: BasicShape? = canevas.findShape(otherBasicViewId)
                        if (otherShape != null) {
                            otherShape.linksTo.add(link?.id)
                            formsToUpdate.add(otherShape)
                        }
                    }
                }
                Log.d("AnchorFormId","Forms id" + link!!.to.formId)
                link?.path?.last()?.x = v.x.toDouble() + v.layoutParams.width / 2.0
                link?.path?.last()?.y = v.y.toDouble() + v.layoutParams.height / 2.0



                emitFormsUpdate(formsToUpdate)
                emitUpdate()
                invalidate()
                requestLayout()
//
//                boundingBox = LinkBoundingBoxView(context, rect)
//                parentView.addView(boundingBox)
            }
        }

        true
    }

    private fun colorAnchorOnHover(x: Int, y: Int){
        for(basicView: BasicElementView in ViewShapeHolder.getInstance().map.keys){
            if(!basicView.isSelectedByOther) {
                val  basicShapeId: String? = ViewShapeHolder.getInstance().map[basicView]
                if (basicView.isViewInBounds(basicView.anchorPoint0, x, y)) {
                    if(basicShapeId != null){
                        basicView.anchorPoint0.setBackgroundColor(Color.GREEN)
                        basicView.anchorPoint0.invalidate()
                        basicView.anchorPoint0.requestLayout()
                    }
                    break
                } else if (basicView.isViewInBounds(basicView.anchorPoint1, x, y)) {
                    if(basicShapeId != null){
                        basicView.anchorPoint1.setBackgroundColor(Color.GREEN)
                        basicView.anchorPoint1.invalidate()
                        basicView.anchorPoint1.requestLayout()
                    }
                    break
                } else if (basicView.isViewInBounds(basicView.anchorPoint2, x, y)) {
                    if(basicShapeId != null){
                        basicView.anchorPoint2.setBackgroundColor(Color.GREEN)
                        basicView.anchorPoint2.invalidate()
                        basicView.anchorPoint2.requestLayout()
                    }
                    break
                } else if (basicView.isViewInBounds(basicView.anchorPoint3, x, y)) {
                    if(basicShapeId != null){
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



    private fun emitFormsUpdate(formsArray: ArrayList<BasicShape>){
        var obj: String =""
        if(formsArray.size>0) {
            val gson = Gson()
            val response: FormsUpdateEvent = FormsUpdateEvent(UserHolder.getInstance().username, ViewShapeHolder.getInstance().canevas.name, formsArray)
            obj = gson.toJson(response)
        }
        if(obj !="") {
            Log.d("emitingFormsUpdate", obj)
            socket?.emit(SocketConstants.UPDATE_FORMS, obj)
        }
    }

    fun emitUpdate(){
        val response: String = this.createLinksUpdateEvent()
        if(response !="") {
            Log.d("emitingUpdate", response)
            socket?.emit(SocketConstants.UPDATE_LINKS, response)
        }
    }
    fun emitDelete(){
        val response: String = this.createLinksUpdateEvent()

        if(response !="") {
            Log.d("emitingDeletion", response)
            socket?.emit(SocketConstants.DELETE_LINKS, response)
        }
    }
    private fun emitSelection(){
        val response: String = this.createLinksUpdateEvent()

        if(response !="") {
            Log.d("emitingSelection", response)
            socket?.emit(SocketConstants.SELECT_LINKS, response)
        }
    }
    private fun emitDeselection(){
        val response: String = this.createLinksUpdateEvent()

        if(response !="") {
            Log.d("emitingDeselection", response)
            socket?.emit(SocketConstants.DESELECT_LINKS, response)
        }
    }
    private fun createLinksUpdateEvent(): String{
        val link: Link? = ViewShapeHolder.getInstance().canevas.findLink(ViewShapeHolder.getInstance().linkMap.getValue(this))
        val linksArray: ArrayList<Link> = ArrayList()
        var obj: String =""
        if(link !=null) {
            linksArray.add(link)
            val gson = Gson()
            val response: LinksUpdateEvent = LinksUpdateEvent(UserHolder.getInstance().username, ViewShapeHolder.getInstance().canevas.name, linksArray)
            obj = gson.toJson(response)
        }
        return obj
    }

}
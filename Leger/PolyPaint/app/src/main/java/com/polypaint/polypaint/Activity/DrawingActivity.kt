package com.polypaint.polypaint.Activity

import android.annotation.SuppressLint
import android.content.Context
import android.content.Intent
import android.content.pm.ActivityInfo
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import co.zsmb.materialdrawerkt.builders.drawer
import co.zsmb.materialdrawerkt.builders.footer
import co.zsmb.materialdrawerkt.draweritems.badgeable.primaryItem
import co.zsmb.materialdrawerkt.draweritems.badgeable.secondaryItem
import com.github.nkzawa.emitter.Emitter
import com.github.nkzawa.socketio.client.Socket
import com.github.salomonbrys.kotson.fromJson
import com.google.gson.Gson
import com.mikepenz.materialdrawer.Drawer
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Enum.ShapeTypes
import com.polypaint.polypaint.Holder.UserHolder
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.*
import com.polypaint.polypaint.View.BasicElementView
import com.polypaint.polypaint.R
import com.polypaint.polypaint.Socket.SocketConstants
import com.polypaint.polypaint.View.ClassView
import com.polypaint.polypaint.View.LinkView
import kotlinx.android.synthetic.main.activity_drawing.*
import kotlinx.android.synthetic.main.basic_element.view.*
import java.util.*
import kotlin.collections.ArrayList


class DrawingActivity : AppCompatActivity(){
    private var inflater : LayoutInflater? = null

    private var drawer: Drawer? = null
    private var socket: Socket? = null

    private var clipboard: ArrayList<BasicShape> = ArrayList<BasicShape>()
    private var stackBasicShape: Stack<BasicShape> = Stack<BasicShape>()

    private fun defaultInit() : Canevas{
        return Canevas("default","default name","aa-author", "aa-owner",
                    2, null, ArrayList<BasicShape>(), ArrayList<Link>())
    }

    @SuppressLint("ClickableViewAccessibility")
    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE
        setContentView(R.layout.activity_drawing)

        val activityToolbar : Toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(activityToolbar)
        drawer = drawer {
            primaryItem("Gallery") {
                icon = R.drawable.message_rectangle_r
                onClick { _ ->
                    false
                }
            }
            primaryItem("Chat") {
                icon = R.drawable.message_rectangle_r
                onClick { _ ->
                    val intent = Intent(this@DrawingActivity, ChatActivity::class.java)
                    startActivity(intent)
                    true
                }
                selectable = false
            }
            footer{
                secondaryItem("Settings") {
                    icon = R.drawable.message_rectangle_r
                }
            }

            toolbar = activityToolbar
        }

        ViewShapeHolder.getInstance().canevas = intent.getSerializableExtra("canevas") as Canevas

        inflater = getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater

        add_button.setOnClickListener {
            addOnCanevas(ShapeTypes.DEFAULT)
            //TODO: Send to all others the event here
        }

        class_button.setOnClickListener {
            addOnCanevas(ShapeTypes.CLASS_SHAPE)
        }

        clear_canvas_button.setOnClickListener {
            emitClearCanvas()
            parent_relative_layout?.removeAllViews()
        }

        duplicate_button.setOnClickListener{
            duplicateView()
        }
        cut_button.setOnClickListener{
            cutView()
        }
        stack_button.setOnClickListener{
            stackView()
        }
        unstack_button.setOnClickListener{
            unstackView()
        }

        phase_button.setOnClickListener {
            syncCanevasFromLayout()
        }
        free_text_button.setOnClickListener {
            syncLayoutFromCanevas()
        }
    }


    override fun onResume() {
        super.onResume()
        val app = application as PolyPaint
        socket = app.socket
        socket?.on(SocketConstants.CANVAS_UPDATE_TEST_RESPONSE, onCanvasUpdate)
        socket?.on(SocketConstants.JOIN_CANVAS_TEST_RESPONSE, onJoinCanvas)
        socket?.on(SocketConstants.FORMS_UPDATED, onFormsUpdated)
        socket?.on(SocketConstants.FORMS_SELECTED, onFormsSelected)
        socket?.on(SocketConstants.FORMS_DELETED, onFormsDeleted)
        socket?.on(SocketConstants.CANVAS_REINITIALIZED, onCanvasReinitialized)
        socket?.on(SocketConstants.FORM_CREATED, onFormsCreated)

        socket?.emit(SocketConstants.JOIN_CANVAS_TEST)
    }

    private fun addOnCanevas(shapeType: ShapeTypes){
        var shape: BasicShape = newShapeOnCanevas(ShapeTypes.DEFAULT)
        var view: BasicElementView = newViewOnCanevas(ShapeTypes.DEFAULT)

        when(shapeType){
            ShapeTypes.DEFAULT -> {}
            ShapeTypes.CLASS_SHAPE -> {
                shape = newShapeOnCanevas(ShapeTypes.CLASS_SHAPE)
                view = newViewOnCanevas(ShapeTypes.CLASS_SHAPE)
            }
        }
        //addViewToLayout
        parent_relative_layout?.addView(view)
        //addShapeToCanevas
        ViewShapeHolder.getInstance().canevas.addShape(shape)
        //mapViewAndShapeId
        ViewShapeHolder.getInstance().map.put(view, shape.id)

        //EMIT
        /*
        val gson = Gson()
        val response :Response = Response(UserHolder.getInstance().username, shape)
        val obj: String = gson.toJson(response)
        Log.d("sending", obj)
        socket?.emit(SocketConstants.CANVAS_UPDATE_TEST, obj)
        */
        emitAddForm(shape)

        syncLayoutFromCanevas()
    }

    private fun addOnCanevas(basicShape: BasicShape){
        when(basicShape.type){
            ShapeTypes.DEFAULT.value()-> {
                val viewType = newViewOnCanevas(ShapeTypes.DEFAULT)
                parent_relative_layout?.addView(viewType)

                //For Sync
                ViewShapeHolder.getInstance().map.put(viewType, basicShape.id)
            }
            ShapeTypes.CLASS_SHAPE.value()-> {
                val viewType = newViewOnCanevas(ShapeTypes.CLASS_SHAPE)
                parent_relative_layout?.addView(viewType)

                //For Sync
                ViewShapeHolder.getInstance().map.put(viewType, basicShape.id)
            }
        }

        syncLayoutFromCanevas()
    }

    private fun newShapeOnCanevas(shapeType: ShapeTypes) : BasicShape{
        var shapeStyle = ShapeStyle(Coordinates(0.0,0.0), 300.0, 100.0, 0.0, "white", 0, "white")
        var shape = BasicShape(UUID.randomUUID().toString(), shapeType.value(), "defaultShape1", shapeStyle, ArrayList<String?>(), ArrayList<String?>())

        when (shapeType) {
            ShapeTypes.DEFAULT -> {}
            ShapeTypes.CLASS_SHAPE -> {
                shape = ClassShape(UUID.randomUUID().toString(), shapeType.value(), "classShape1", shapeStyle, ArrayList<String?>(), ArrayList<String?>(),ArrayList<String?>(), ArrayList<String?>())
            }
        }

        return shape
    }

    private fun newViewOnCanevas(shapeType : ShapeTypes) : BasicElementView{
        var viewType : BasicElementView = BasicElementView(this)
        val viewContainer = inflater!!.inflate(R.layout.basic_element, null)

        when(shapeType){
            ShapeTypes.DEFAULT->{
                viewType = BasicElementView(this)

                val link = LinkView(this)
                parent_relative_layout?.addView(link)
            }
            ShapeTypes.CLASS_SHAPE->{
                viewType = ClassView(this)
            }
        }
        viewType.addView(viewContainer)

        return viewType
    }

    private fun duplicateView(){
        if(clipboard.isEmpty()){
            //Copying list to avoid ConcurrentModificationException
            val list = ViewShapeHolder.getInstance().map.keys.toMutableList()
            for (view in list){
                if(view.isSelected && !view.isSelectedByOther) {
                    view.isSelected = false
                    val shapeToDuplicate = ViewShapeHolder.getInstance().canevas.findShape(
                        ViewShapeHolder.getInstance().map.getValue(view)
                    )
                    if (shapeToDuplicate != null) {

                        val shapeDuplicated = shapeToDuplicate.copy()
                        shapeDuplicated.id = UUID.randomUUID().toString()
                        ViewShapeHolder.getInstance().canevas.addShape(shapeDuplicated)
                        addOnCanevas(shapeDuplicated)

                        emitAddForm(shapeDuplicated)
                    }
                }
            }
        }else{
            for(shape in clipboard){
                ViewShapeHolder.getInstance().canevas.addShape(shape)
                addOnCanevas(shape)
                emitAddForm(shape)
            }
            clipboard.clear()
        }
    }

    private fun cutView(){
        val list = ViewShapeHolder.getInstance().map.keys.toMutableList()
        for (view in list){
            if(view.isSelected && !view.isSelectedByOther){
                val shapeToCut = ViewShapeHolder.getInstance().canevas.findShape(ViewShapeHolder.getInstance().map.getValue(view))
                clipboard.add(shapeToCut!!)
                emitDeleteForm(shapeToCut!!)
                parent_relative_layout.removeView(view)
                ViewShapeHolder.getInstance().remove(view)
            }
        }
    }

    private fun stackView(){
        val list = ViewShapeHolder.getInstance().map.keys.toMutableList()
        for (view in list){
            if(view.isSelected && !view.isSelectedByOther){
                val shapeToStack = ViewShapeHolder.getInstance().canevas.findShape(ViewShapeHolder.getInstance().map.getValue(view))

                stackBasicShape.push(shapeToStack)
                emitDeleteForm(shapeToStack!!)

                parent_relative_layout.removeView(view)
                ViewShapeHolder.getInstance().remove(view)
            }
        }
    }
    private fun unstackView(){
        try {
            val shapeUnstacked = stackBasicShape.pop()
            ViewShapeHolder.getInstance().canevas.addShape(shapeUnstacked)
            addOnCanevas(shapeUnstacked)

            emitAddForm(shapeUnstacked)
        }catch (e : EmptyStackException){}
    }
    private fun syncLayoutFromCanevas(){
        for (view in ViewShapeHolder.getInstance().map.keys){
            val basicShapeId:  String = ViewShapeHolder.getInstance().map.getValue(view)
            val basicShape: BasicShape? = ViewShapeHolder.getInstance().canevas.findShape(basicShapeId)
            if(basicShape != null) {
                view.x = (basicShape.shapeStyle.coordinates.x).toFloat()
                view.y = (basicShape.shapeStyle.coordinates.y).toFloat()
                view.resize(basicShape.shapeStyle.width.toInt(), basicShape.shapeStyle.height.toInt())
                view.rotation = basicShape.shapeStyle.rotation.toFloat()
            }
        }
    }

    public fun syncCanevasFromLayout(){
        for (shape in ViewShapeHolder.getInstance().canevas.shapes){
            val basicElem = ViewShapeHolder.getInstance().map.inverse().getValue(shape.id)
            shape.shapeStyle.coordinates.x = (basicElem.x).toDouble()
            shape.shapeStyle.coordinates.y = (basicElem.y).toDouble()
            shape.shapeStyle.width = basicElem.borderResizableLayout.width.toDouble()
            shape.shapeStyle.height = basicElem.borderResizableLayout.height.toDouble()
            shape.shapeStyle.rotation = basicElem.rotation.toDouble()
        }

    }

    private fun emitClearCanvas(){
        val gson = Gson()
        val response: UserResponse = UserResponse(UserHolder.getInstance().username)
        val obj: String = gson.toJson(response)

        Log.d("emitingClearCanvas", obj)
        socket?.emit(SocketConstants.REINITIALIZE_CANVAS, obj)
    }

    private fun emitAddForm(basicShape: BasicShape){
        var obj: String =""
        val gson = Gson()
        val response: DrawingActivity.Response =DrawingActivity.Response(UserHolder.getInstance().username, basicShape)
        obj = gson.toJson(response)
        Log.d("emitingCreateForm", obj)
        socket?.emit(SocketConstants.CREATE_FORM, obj)

    }

    private fun emitDeleteForm(basicShape: BasicShape){
        var obj: String =""
        val gson = Gson()
        val response: DrawingActivity.Response =DrawingActivity.Response(UserHolder.getInstance().username, basicShape)
        obj = gson.toJson(response)
        Log.d("emitingDelete", obj)
        socket?.emit(SocketConstants.DELETE_FORMS, obj)

    }


    public class Response(var username: String, var basicShape: BasicShape){}
    public class UserResponse(var username: String){}

    private var onCanvasUpdate: Emitter.Listener = Emitter.Listener {

        val gson = Gson()
        val obj: Response = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            Log.d("canvasUpdate", obj.username + obj.basicShape.name)
            runOnUiThread {
                addOnCanevas(obj.basicShape)
            }
        }

    }

    private var onJoinCanvas: Emitter.Listener = Emitter.Listener {
        Log.d("joinCanvas", it.get(0).toString())
    }

    private var onFormsUpdated: Emitter.Listener = Emitter.Listener {
        Log.d("onFormsUpdated", "alllooo")

        val gson = Gson()
        val obj: Response = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            Log.d("formsUpdate", obj.username + obj.basicShape.name)
            runOnUiThread {
                ViewShapeHolder.getInstance().canevas.updateShape(obj.basicShape)
                syncLayoutFromCanevas()
            }
        }

    }

    private var onFormsSelected: Emitter.Listener = Emitter.Listener {
        Log.d("onFormsSelected", "alllooo")

        val gson = Gson()
        val obj: Response = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            Log.d("formsSelect", obj.username + obj.basicShape.name)
            runOnUiThread {
                val view: BasicElementView? = ViewShapeHolder.getInstance().map.inverse()[obj.basicShape.id]
                if(view != null) {
                    view.borderResizableLayout?.setBackgroundResource(R.drawable.borders_red)
                    view.isSelectedByOther = true
                }

                syncLayoutFromCanevas()
            }
        }

    }

    private var onFormsDeleted: Emitter.Listener = Emitter.Listener {
        Log.d("onFormsDeleted", "alllooo")

        val gson = Gson()
        val obj: Response = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            Log.d("formsDeleted", obj.username + obj.basicShape.name)
            runOnUiThread {
                ViewShapeHolder.getInstance().remove(obj.basicShape)
                syncLayoutFromCanevas()
            }
        }

    }

    private var onCanvasReinitialized: Emitter.Listener = Emitter.Listener {
        Log.d("onCanvasReinitialized", "alllooo")

        val gson = Gson()
        val obj: UserResponse = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            Log.d("canvasReinitialized", obj.username)
            runOnUiThread {
                ViewShapeHolder.getInstance().removeAll()
                syncLayoutFromCanevas()
            }
        }

    }

    private var onFormsCreated: Emitter.Listener = Emitter.Listener {
        Log.d("onFormsCreated", "alllooo")

        val gson = Gson()
        val obj: Response = gson.fromJson(it[0].toString())
        if(obj.username != UserHolder.getInstance().username) {
            Log.d("formsCreated", obj.username + obj.basicShape.name)
            runOnUiThread {
                ViewShapeHolder.getInstance().canevas.addShape(obj.basicShape)
                addOnCanevas(obj.basicShape)
            }
        }

    }

    override fun onPause(){
        socket?.off(SocketConstants.CANVAS_UPDATE_TEST_RESPONSE, onCanvasUpdate)
        socket?.off(SocketConstants.JOIN_CANVAS_TEST_RESPONSE, onJoinCanvas)
        socket?.off(SocketConstants.FORMS_UPDATED, onFormsUpdated)
        socket?.off(SocketConstants.FORMS_SELECTED, onFormsSelected)
        socket?.off(SocketConstants.FORMS_DELETED, onFormsDeleted)
        socket?.off(SocketConstants.CANVAS_REINITIALIZED, onCanvasReinitialized)
        socket?.off(SocketConstants.FORM_CREATED, onFormsCreated)
        super.onPause()
    }

    /*override fun onBackPressed() {
        socket?.off(SocketConstants.CANVAS_UPDATE_TEST_RESPONSE, onCanvasUpdate)
        socket?.off(SocketConstants.JOIN_CANVAS_TEST_RESPONSE, onJoinCanvas)
        socket?.off(SocketConstants.FORMS_UPDATED, onFormsUpdated)
        super.onBackPressed()
    }*/
    /*
    override fun onBackPressed() {
        val intent = Intent(this, GalleryActivity::class.java)
        startActivity(intent)
        val app = application as PolyPaint
        val socket: Socket? = app.socket
        socket?.disconnect()
        val intent = Intent(this, LoginActivity::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TOP
        startActivity(intent)
        finish()

    }*/
}
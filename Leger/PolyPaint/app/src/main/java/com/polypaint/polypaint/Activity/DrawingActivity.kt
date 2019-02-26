package com.polypaint.polypaint.Activity

import android.annotation.SuppressLint
import android.content.Context
import android.content.Intent
import android.content.pm.ActivityInfo
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.widget.Button
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import co.zsmb.materialdrawerkt.builders.drawer
import co.zsmb.materialdrawerkt.builders.footer
import co.zsmb.materialdrawerkt.draweritems.badgeable.primaryItem
import co.zsmb.materialdrawerkt.draweritems.badgeable.secondaryItem
import com.github.nkzawa.emitter.Emitter
import com.github.nkzawa.socketio.client.Socket
import com.github.salomonbrys.kotson.fromJson
import com.github.salomonbrys.kotson.jsonObject
import com.google.gson.Gson
import com.google.gson.JsonObject
import com.mikepenz.materialdrawer.Drawer
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Enum.AccessibilityTypes
import com.polypaint.polypaint.Holder.UserHolder
import com.polypaint.polypaint.Model.*
import com.polypaint.polypaint.View.BasicElementView
import com.polypaint.polypaint.R
import com.polypaint.polypaint.Socket.SocketConstants
import com.polypaint.polypaint.View.ClassView
import kotlinx.android.synthetic.main.activity_drawing.*
import kotlinx.android.synthetic.main.basic_element.view.*


class DrawingActivity : AppCompatActivity(){
    private var inflater : LayoutInflater? = null
    private var canevas : Canevas = defaultInit()
    private var mapElemShape : HashMap<BasicElementView, BasicShape> = hashMapOf()
    private var drawer: Drawer? = null
    private var socket: Socket? = null


    private fun defaultInit() : Canevas{

        /*var shapeArray = ArrayList<BasicShape>()
        var mShapeStyle = ShapeStyle(Coordinates(100.0,100.0), 300.0, 100.0, 0.0, "white", 0, "white")
        var mBasicShape1 = BasicShape("1", 0, "defaultShape1", mShapeStyle, ArrayList<String?>())

        shapeArray.add(mBasicShape1)*/

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

        canevas = intent.getSerializableExtra("canevas") as Canevas



        inflater = getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater

        add_button.setOnClickListener {
            addBasicElementOnCanevas()
            //TODO: Send to all others the event here


        }

        class_button.setOnClickListener {
            addClassViewOnCanevas()
        }

        clear_canvas_button.setOnClickListener {
            parent_relative_layout?.removeAllViews()
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

        socket?.emit(SocketConstants.JOIN_CANVAS_TEST)
    }

    private fun addBasicElementOnCanevas(){

        var mShapeStyle = ShapeStyle(Coordinates(100.0,100.0), 400.0, 300.0, 0.0, "white", 0, "white")
        //TODO : Request uuid
        var mBasicShape = BasicShape("1", 0, "defaultShape1", mShapeStyle, ArrayList<String?>())

        val mBasicElem = BasicElementView(this)
        val viewToAdd = inflater!!.inflate(R.layout.basic_element, null)
        mBasicElem.addView(viewToAdd)

        mapElemShape.put(mBasicElem, mBasicShape)

        canevas.addShape(mBasicShape)
        parent_relative_layout?.addView(mBasicElem)



        val gson = Gson()
        val response :Response = Response(UserHolder.getInstance().username, mBasicShape)
        val obj: String = gson.toJson(response)
        /*val obj: JsonObject = jsonObject(
            "username" to UserHolder.getInstance().username,
            "basicShape" to jsonObject(
                "id" to mBasicShape.id,
                "name" to mBasicShape.name
            )
        )
*/
        Log.d("sending", obj)
        socket?.emit(SocketConstants.CANVAS_UPDATE_TEST, obj)
    }

    private fun addClassViewOnCanevas(){
        val mClassView = ClassView(this)
        val viewToAdd = inflater!!.inflate(R.layout.basic_element, null)
        mClassView.addView(viewToAdd)

        parent_relative_layout?.addView(mClassView)
    }


    private fun syncLayoutFromCanevas(){
        Log.d("canvas",""+canevas.shapes.size)
        Log.d("layout",""+parent_relative_layout.childCount)
        
        for (view in mapElemShape.keys){
            val basicShape = mapElemShape.getValue(view)
            view.x = (basicShape.shapeStyle.coordinates.x).toFloat()
            view.y = (basicShape.shapeStyle.coordinates.y).toFloat()
            view.resize(basicShape.shapeStyle.width.toInt(), basicShape.shapeStyle.height.toInt())
        }
    }

    private var onCanvasUpdate: Emitter.Listener = Emitter.Listener {
        val gson = Gson()
        val obj: Response = gson.fromJson(it[0].toString())
        Log.d("canvasUpdate", obj.username+obj.basicShape.name)
    }

    private class Response(var username: String, var basicShape: BasicShape){}

    private var onJoinCanvas: Emitter.Listener = Emitter.Listener {
        Log.d("joinCanvas", it.get(0).toString())
    }

    override fun onBackPressed() {
        socket?.off(SocketConstants.CANVAS_UPDATE_TEST_RESPONSE, onCanvasUpdate)
        socket?.off(SocketConstants.JOIN_CANVAS_TEST_RESPONSE, onJoinCanvas)
    }

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
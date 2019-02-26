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
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import co.zsmb.materialdrawerkt.builders.drawer
import co.zsmb.materialdrawerkt.builders.footer
import co.zsmb.materialdrawerkt.draweritems.badgeable.primaryItem
import co.zsmb.materialdrawerkt.draweritems.badgeable.secondaryItem
import com.mikepenz.materialdrawer.Drawer
import com.polypaint.polypaint.Enum.AccessibilityTypes
import com.polypaint.polypaint.Model.*
import com.polypaint.polypaint.View.BasicElementView
import com.polypaint.polypaint.R
import com.polypaint.polypaint.View.ClassView
import kotlinx.android.synthetic.main.activity_drawing.*
import kotlinx.android.synthetic.main.basic_element.view.*


class DrawingActivity : AppCompatActivity(){
    private var inflater : LayoutInflater? = null
    private var canevas : Canevas = defaultInit()
    private var mapElemShape : HashMap<BasicElementView, BasicShape> = hashMapOf()
    private var drawer: Drawer? = null


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
        //if(canevas.shapes.size == parent_relative_layout.childCount)

        //sync view based on view layout params
        for (view in mapElemShape.keys){

            //TODO : Width and Height Not refreshing auto when called
            /*
            view.borderResizableLayout.layoutParams.width = (mapElemShape.getValue(view).shapeStyle.width).toInt()
            view.borderResizableLayout.layoutParams.height = (mapElemShape.getValue(view).shapeStyle.height).toInt()
            view.borderResizableLayout.invalidate()
            view.invalidate()
            */

            view.x = (mapElemShape.getValue(view).shapeStyle.coordinates.x).toFloat()
            view.y = (mapElemShape.getValue(view).shapeStyle.coordinates.y).toFloat()
            //view.borderResizableLayout.layoutParams.width = 500
            //view.borderResizableLayout.layoutParams.height = 500
        }
        //parent_relative_layout.invalidate()
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
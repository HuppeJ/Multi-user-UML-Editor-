package com.polypaint.polypaint.Activity

import android.annotation.SuppressLint
import android.content.Context
import android.content.pm.ActivityInfo
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.widget.Button
import androidx.appcompat.app.AppCompatActivity
import com.polypaint.polypaint.View.BasicElementView
import com.polypaint.polypaint.R
import com.polypaint.polypaint.View.ClassView
import kotlinx.android.synthetic.main.activity_drawing.*
import kotlinx.android.synthetic.main.basic_element.view.*

class DrawingActivity : AppCompatActivity(){
    // private var parentRelativeLayout : RelativeLayout? = null
    private var inflater : LayoutInflater? = null
    private var childIndexCount : Int = 0
    public var childSelected : Int = 0



    //TODO: déplacer dans object contenant tous les eventListeners possible
    private var onTouchListener = View.OnTouchListener { v, event ->
        //v.x = event.rawX - v.width/2 - parent_relative_layout!!.x
        //v.y = event.rawY - v.height/2 - parent_relative_layout!!.y

        childSelected = v.id

        v.requestFocus()
        true
    }


    @SuppressLint("ClickableViewAccessibility")
    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE
        setContentView(R.layout.activity_drawing)

        inflater = getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater

        var addButton: Button = findViewById(R.id.add_button)
        addButton.setOnClickListener {

            val mBasicElem = BasicElementView(this)

            mBasicElem.id = childIndexCount
            val viewToAdd = inflater!!.inflate(R.layout.basic_element, null)
            mBasicElem.addView(viewToAdd,mBasicElem.childCount - 1)

            mBasicElem.setOnTouchListener(onTouchListener)
            parent_relative_layout?.addView(mBasicElem, childIndexCount)
            childIndexCount++
        }

        var addClassButton: Button = findViewById(R.id.class_button)
        addClassButton.setOnClickListener {

            val mBasicElem = ClassView(this)

            mBasicElem.id = childIndexCount
            val viewToAdd = inflater!!.inflate(R.layout.basic_element, null)
            mBasicElem.addView(viewToAdd,mBasicElem.childCount - 1)

            mBasicElem.setOnTouchListener(onTouchListener)
            parent_relative_layout?.addView(mBasicElem, childIndexCount)
            childIndexCount++
        }

        move_button.setOnClickListener {
            val txt = parent_relative_layout?.getChildAt(childSelected)!!.first_line.text
            parent_relative_layout?.getChildAt(childSelected )!!.first_line.text = txt.toString() + "3"
        }
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
package com.polypaint.polypaint.Activity

import android.annotation.SuppressLint
import android.content.Context
import android.content.Intent
import android.content.pm.ActivityInfo
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.RelativeLayout
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import com.github.nkzawa.socketio.client.Socket
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Model.BasicElement
import com.polypaint.polypaint.R

class DrawingActivity : AppCompatActivity(){
    private var parentRelativeLayout : RelativeLayout? = null
    private var inflater : LayoutInflater? = null
    private var childIndexCount : Int = 0
    private var childSelected : Int = 0
    //TODO: déplacer dans object contenant tous les eventListeners possible
    private var onTouchListener = View.OnTouchListener { v, event ->
        //
        v.x = event.rawX - v.width/2 - parentRelativeLayout!!.x
        v.y = event.rawY - v.height - parentRelativeLayout!!.y
        //Set soft focus
        childSelected = v.id

        v.requestFocus()
        // Return true means this listener has complete process this event successfully.
        true
    }
    private var onLongClickListener = View.OnLongClickListener { v ->
        v.findViewById<TextView>(R.id.first_line).text = "LONGCLICK"
        v.findViewById<TextView>(R.id.first_line).text = "ACTION"
        true
    }
    /*
    private var onTouchListenerRelLayout = View.OnClickListener { v ->
        childSelected = parentRelativeLayout!!.indexOfChild(parentRelativeLayout!!.focusedChild)
        for (i in 0..parentRelativeLayout!!.childCount)
            Log.d("focus : ", ""+i+childSelected)

        // Return true means this listener has complete process this event successfully.
        true
    }
    */
    @SuppressLint("ClickableViewAccessibility")
    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE
        setContentView(R.layout.activity_drawing)

        parentRelativeLayout = findViewById(R.id.parent_layout)
        //parentRelativeLayout!!.setOnClickListener(onTouchListenerRelLayout)

        inflater = getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater



        var addButton: Button = findViewById(R.id.add_button)
        addButton.setOnClickListener {

            val mBasicElem = BasicElement(this)

            mBasicElem.id = childIndexCount
            val viewToAdd = inflater!!.inflate(R.layout.basic_element, null)
            mBasicElem.addView(viewToAdd,mBasicElem.childCount - 1)

            mBasicElem.setOnTouchListener(onTouchListener)
            mBasicElem.setOnLongClickListener(onLongClickListener)
            parentRelativeLayout?.addView(mBasicElem, childIndexCount)
            childIndexCount++
        }
        var moveButton: Button = findViewById(R.id.move_button)
        moveButton.setOnClickListener {
            val txt = parentRelativeLayout?.getChildAt(childSelected)!!.findViewById<TextView>(R.id.first_line).text
            parentRelativeLayout?.getChildAt(childSelected )!!.findViewById<TextView>(R.id.first_line).text = txt.toString() + "3"
            parentRelativeLayout?.getChildAt(childSelected )!!.findViewById<TextView>(R.id.second_line).text = "TEXT4"
        }
    }

    override fun onBackPressed() {
        val app = application as PolyPaint
        val socket: Socket? = app.socket
        socket?.disconnect()
        val intent = Intent(this, LoginActivity::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TOP
        startActivity(intent)
        finish()
    }
}
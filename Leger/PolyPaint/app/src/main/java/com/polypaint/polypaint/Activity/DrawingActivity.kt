package com.polypaint.polypaint.Activity

import android.content.Context
import android.content.Intent
import android.content.pm.ActivityInfo
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.RelativeLayout
import androidx.appcompat.app.AppCompatActivity
import com.github.nkzawa.socketio.client.Socket
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Model.BasicElement
import com.polypaint.polypaint.R

class DrawingActivity : AppCompatActivity(){
    private var parentRelativeLayout : RelativeLayout? = null
    private var inflater : LayoutInflater? = null

    //TODO: déplacer dans object contenant tous les eventListeners possible
    private var onTouchListener = View.OnTouchListener { v, event ->
        //
        v.x = event.rawX - v.width/2 - parentRelativeLayout!!.x
        v.y = event.rawY - v.height - parentRelativeLayout!!.y
        // Return true means this listener has complete process this event successfully.
        true
    }

    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE
        setContentView(R.layout.activity_drawing)

        parentRelativeLayout = findViewById(R.id.parent_layout)
        inflater = getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater

        var addButton: Button = findViewById(R.id.add_button)
        addButton.setOnClickListener {

            val mBasicElem = BasicElement(this)

            val viewToAdd = inflater!!.inflate(R.layout.basic_element, null)
            val viewToAdd2 = inflater!!.inflate(R.layout.basic_element, null)
            viewToAdd2.y += 24

            val rel = RelativeLayout.LayoutParams(
                ViewGroup.LayoutParams.WRAP_CONTENT,
                ViewGroup.LayoutParams.WRAP_CONTENT)

            mBasicElem.addView(viewToAdd,mBasicElem.childCount - 1)
            mBasicElem.addView(viewToAdd2,mBasicElem.childCount - 1)
            mBasicElem.layoutParams = rel

            mBasicElem.setOnTouchListener(onTouchListener)

            parentRelativeLayout?.addView(mBasicElem, parentRelativeLayout!!.childCount - 1)
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
package com.polypaint.polypaint.Activity

import android.content.Context
import android.content.Intent
import android.content.pm.ActivityInfo
import android.graphics.Bitmap
import android.graphics.Canvas
import android.graphics.Paint
import android.os.Bundle
import android.view.LayoutInflater
import android.view.MotionEvent
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.ImageView
import android.widget.LinearLayout
import androidx.appcompat.app.AppCompatActivity
import com.github.nkzawa.socketio.client.Socket
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Model.BasicShape
import com.polypaint.polypaint.R



class DrawingActivityView : AppCompatActivity(){
    private var parentLinearLayout : LinearLayout? = null

    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE
        setContentView(R.layout.activity_drawing_view)
        parentLinearLayout = findViewById(R.id.parent_linear_layout)

        var addButton: Button = findViewById(R.id.add_button)
        addButton.setOnClickListener {
            val inflater = getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater
            val viewToAdd = inflater.inflate(R.layout.basic_element,null)
            viewToAdd.setOnClickListener {it ->
                it.left += 10
            }
            parentLinearLayout?.addView(viewToAdd,parentLinearLayout!!.childCount - 1)

        }
        /*
        var moveButton: Button = findViewById(R.id.add_button)
        moveButton.setOnClickListener {

        }
        */
        /*
        var drawButton: Button = findViewById(R.id.draw_button)
        drawButton.setOnClickListener {
            drawSomething()
        }*/
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
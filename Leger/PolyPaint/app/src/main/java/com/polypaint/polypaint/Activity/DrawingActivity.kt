package com.polypaint.polypaint.Activity

import android.content.Intent
import android.content.pm.ActivityInfo
import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.github.nkzawa.socketio.client.Socket
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.R

class DrawingActivity : AppCompatActivity(){

    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE
        setContentView(R.layout.activity_drawing)
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
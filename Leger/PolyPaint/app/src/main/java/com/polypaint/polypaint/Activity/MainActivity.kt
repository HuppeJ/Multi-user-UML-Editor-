package com.polypaint.polypaint.Activity

import android.content.Intent
import android.support.v7.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import com.github.nkzawa.socketio.client.IO
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.R

class MainActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
    }

    fun onGo(view: View) {
        val app = application as PolyPaint
        app.setSocketUri("http://192.168.0.194:3000")
        val intent = Intent(this, LoginActivity::class.java)
        startActivity(intent)
    }
}

package com.polypaint.polypaint.Activity

import android.content.Intent
import android.content.pm.ActivityInfo
import android.support.v7.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.EditText
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.R

class ServerActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_server)
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE
    }

    fun onGo(view: View) {
        val app = application as PolyPaint
        val serverUrlView: EditText = findViewById(R.id.edittext_server_url)

        app.setSocketUri(serverUrlView.text.toString().trim())

        val intent = Intent(this, LoginActivity::class.java)
        startActivity(intent)
    }
}
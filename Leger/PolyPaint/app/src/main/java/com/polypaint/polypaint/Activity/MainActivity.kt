package com.polypaint.polypaint.Activity

import android.content.Intent
import android.support.v7.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import com.polypaint.polypaint.R

class MainActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
    }

    fun onGo(view: View) {
        val intent = Intent(this, ChatActivity::class.java)
        startActivity(intent)
    }
}

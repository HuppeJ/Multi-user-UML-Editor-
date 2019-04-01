package com.polypaint.polypaint.Activity

import android.content.Intent
import android.content.pm.ActivityInfo
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.EditText
import androidx.appcompat.widget.Toolbar
import co.zsmb.materialdrawerkt.builders.drawer
import co.zsmb.materialdrawerkt.draweritems.badgeable.primaryItem
import co.zsmb.materialdrawerkt.draweritems.badgeable.secondaryItem
import co.zsmb.materialdrawerkt.draweritems.divider
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
        val serverUrlView: EditText = this.findViewById(R.id.edittext_server_url)

        app.uri = serverUrlView.text.toString().trim()

        val intent = Intent(this, LoginActivity::class.java)
//        startActivityForResult(intent, 0)
        startActivity(intent)
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        //finish()
    }
}

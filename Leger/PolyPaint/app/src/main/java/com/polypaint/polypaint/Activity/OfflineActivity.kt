package com.polypaint.polypaint.Activity

import android.content.Intent
import android.content.pm.ActivityInfo
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.EditText
import androidx.appcompat.widget.Toolbar
import co.zsmb.materialdrawerkt.builders.drawer
import co.zsmb.materialdrawerkt.draweritems.badgeable.primaryItem
import co.zsmb.materialdrawerkt.draweritems.badgeable.secondaryItem
import co.zsmb.materialdrawerkt.draweritems.divider
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Holder.UserHolder
import com.polypaint.polypaint.Model.Canevas
import com.polypaint.polypaint.Model.Coordinates
import com.polypaint.polypaint.R
import kotlinx.android.synthetic.main.activity_offline.*
import java.util.*

class OfflineActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_offline)
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE
    }

    override fun onAttachedToWindow() {
        super.onAttachedToWindow()
        offline_button?.setOnClickListener {
            val intent = Intent(this, DrawingActivity::class.java)
            val canevas = Canevas(UUID.randomUUID().toString(), "", UserHolder.getInstance().username, UserHolder.getInstance().username, 1, "",  ArrayList(), ArrayList(), "", Coordinates(1100.0,800.0))

            intent.putExtra("canevas", canevas)
            //ViewShapeHolder.getInstance().canevas = selectedCanevas!!
            startActivityForResult(intent, 0)
        }
        online_button?.setOnClickListener {
            val intent = Intent(this, ServerActivity::class.java)
            startActivity(intent)
        }
    }

}

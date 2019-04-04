package com.polypaint.polypaint.Activity

import android.app.Activity
import android.content.pm.ActivityInfo
import android.os.Bundle
import android.content.Intent
import android.text.TextUtils
import android.util.Log
import android.view.MenuItem
import android.view.View
import android.widget.Button
import android.widget.EditText
import android.widget.RadioButton
import android.widget.RadioGroup
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import androidx.fragment.app.DialogFragment
import co.zsmb.materialdrawerkt.builders.drawer
import co.zsmb.materialdrawerkt.builders.footer
import co.zsmb.materialdrawerkt.draweritems.badgeable.primaryItem
import co.zsmb.materialdrawerkt.draweritems.badgeable.secondaryItem
import com.github.nkzawa.emitter.Emitter
import com.github.nkzawa.socketio.client.Socket
import com.github.salomonbrys.kotson.fromJson
import com.google.gson.Gson
import com.mikepenz.materialdrawer.Drawer
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Enum.AccessibilityTypes
import com.polypaint.polypaint.Fragment.EditClassDialogFragment
import com.polypaint.polypaint.Holder.UserHolder
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.BasicShape
import com.polypaint.polypaint.Model.Canevas
import com.polypaint.polypaint.Model.Coordinates
import com.polypaint.polypaint.Model.ShapeStyle
import com.polypaint.polypaint.R
import com.polypaint.polypaint.ResponseModel.CanvasCreationResponse
import com.polypaint.polypaint.ResponseModel.CanvasJoinResponse
import com.polypaint.polypaint.Socket.SocketConstants
import com.polypaint.polypaint.SocketReceptionModel.CanvasEvent
import com.polypaint.polypaint.SocketReceptionModel.GalleryEditEvent
import kotlinx.android.synthetic.main.toolbar.*
import java.util.*

class CreateDrawingActivity: AppCompatActivity(){
    private var drawer: Drawer? = null
    private var isDrawingPrivate: Boolean = false
    private var isPasswordProtected: Boolean = false
    private var nameView: EditText? = null
    private var passwordView: EditText? = null
    private var canevas: Canevas? = null
    private var socket: Socket? = null

    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE

        setContentView(R.layout.activity_create_drawing)

        val app = application as PolyPaint
        socket = app.socket

        val activityToolbar : Toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(activityToolbar)
        help_button.visibility = View.GONE

        drawer = drawer {
            primaryItem("Gallery") {
                icon = R.drawable.ic_picture
                onClick { _ ->
                    false
                }
            }
            primaryItem("Chat") {
                icon = R.drawable.ic_chat
                onClick { _ ->
                    val intent = Intent(this@CreateDrawingActivity, ChatActivity::class.java)
                    startActivity(intent)
                    true
                }
                selectable = false
            }
            footer{
                secondaryItem("Settings") {
                    icon = R.drawable.ic_settings
                }
            }

            toolbar = activityToolbar
        }


        val radioPublic: RadioButton = findViewById(R.id.radio_public)
        val radioPrivate: RadioButton = findViewById(R.id.radio_private)
        radioPublic.isChecked = true
        radioPublic.setOnClickListener { _->
            isDrawingPrivate = false
        }
        radioPrivate.setOnClickListener { _->
            isDrawingPrivate = true
        }

        nameView = findViewById(R.id.drawing_name_text)
        passwordView = findViewById(R.id.drawing_password_text)
        var createDrawingButton: Button = findViewById(R.id.create_drawing_button)
        createDrawingButton.setOnClickListener { _->
            createDrawing()
        }

    }

    override fun onResume() {
        super.onResume()
        val app = application as PolyPaint
        socket = app.socket

        val localSocket = socket
//        toolbar_login_button.visibility = View.VISIBLE
//        if(localSocket != null && localSocket.connected()){
//            toolbar_login_button.visibility = View.INVISIBLE
//        }
    }

    private fun createDrawing(){

        var name = nameView?.text.toString().trim()
        if(TextUtils.isEmpty(name)){
            nameView?.requestFocus()
            nameView?.error = "Enter a name"
            name = ""
            return
        }
        var password = passwordView?.text.toString().trim()
        isPasswordProtected = TextUtils.isEmpty(password)

        var accessibility: Int

        if(isDrawingPrivate) {
            accessibility = AccessibilityTypes.PRIVATE.ordinal
        } else {
            accessibility = AccessibilityTypes.PUBLIC.ordinal
        }

        canevas = Canevas(UUID.randomUUID().toString(), name, UserHolder.getInstance().username, UserHolder.getInstance().username, accessibility, password,  ArrayList(), ArrayList(), "", Coordinates(1050.0,798.0))


        val gson = Gson()
        if(canevas != null) {
            val canvasEvent: CanvasEvent = CanvasEvent(UserHolder.getInstance().username, canevas!!)
            val sendObj = gson.toJson(canvasEvent)
            Log.d("createObj", sendObj)
            socket?.emit(SocketConstants.CREATE_CANVAS, sendObj)


            val intent = Intent(this, GalleryActivity::class.java)
            startActivityForResult(intent, 0)

        }

    }


    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        setResult(Activity.RESULT_CANCELED)
        finish()
    }

    /*
    private var onJoinCanvasResponse: Emitter.Listener = Emitter.Listener {
        Log.d("onJoinCanvasResponse", "alllooo11111")

        val gson = Gson()
        val obj: CanvasJoinResponse = gson.fromJson(it[0].toString())

        if(obj.isCanvasRoomJoined && obj.canvasName == canevas?.name) {
            Log.d("canvasJoined", "created " + canevas?.name)
            val intent = Intent(this, DrawingActivity::class.java)
            intent.putExtra("canevas", canevas)
            //ViewShapeHolder.getInstance().canevas = canevas!!
            startActivity(intent)
        }
    }
    */
}
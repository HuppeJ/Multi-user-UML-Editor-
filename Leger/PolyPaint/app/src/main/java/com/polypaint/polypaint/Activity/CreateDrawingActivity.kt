package com.polypaint.polypaint.Activity

import android.content.pm.ActivityInfo
import android.os.Bundle
import android.content.Intent
import android.text.TextUtils
import android.util.Log
import android.view.MenuItem
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
import com.polypaint.polypaint.Model.Canevas
import com.polypaint.polypaint.R
import com.polypaint.polypaint.ResponseModel.CanvasCreationResponse
import com.polypaint.polypaint.ResponseModel.CanvasJoinResponse
import com.polypaint.polypaint.Socket.SocketConstants
import com.polypaint.polypaint.SocketReceptionModel.CanvasEvent
import com.polypaint.polypaint.SocketReceptionModel.GalleryEditEvent
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
        drawer = drawer {
            primaryItem("Gallery") {
                icon = R.drawable.message_rectangle_r
                onClick { _ ->
                    false
                }
            }
            primaryItem("Chat") {
                icon = R.drawable.message_rectangle_r
                onClick { _ ->
                    val intent = Intent(this@CreateDrawingActivity, ChatActivity::class.java)
                    startActivity(intent)
                    true
                }
                selectable = false
            }
            footer{
                secondaryItem("Settings") {
                    icon = R.drawable.message_rectangle_r
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



        socket?.on(SocketConstants.CREATE_CANVAS_RESPONSE, onCreateCanvasResponse)
        socket?.on(SocketConstants.JOIN_CANVAS_ROOM_RESPONSE, onJoinCanvasResponse)

        /*val ft = supportFragmentManager.beginTransaction()
        //ft.add(R.id.list_container, RoomsListFragment())
        ft.add(R.id.details_container, MessageListFragment(), MessageListFragment.TAG)
        ft.commit()*/

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

        canevas = Canevas(UUID.randomUUID().toString(), name, UserHolder.getInstance().username, UserHolder.getInstance().username, AccessibilityTypes.PUBLIC.ordinal, password, ArrayList(), ArrayList())



        val gson = Gson()
        if(canevas != null) {
            val canvasEvent: CanvasEvent = CanvasEvent(UserHolder.getInstance().username, canevas!!)
            val sendObj = gson.toJson(canvasEvent)
            Log.d("createObj", sendObj)
            socket?.emit(SocketConstants.CREATE_CANVAS, sendObj)

        }


//
//        var dialog: DialogFragment = EditClassDialogFragment()
//        var bundle: Bundle = Bundle()
//        bundle.putString("id", "asdfasg")
//        dialog.arguments = bundle
//
//        Log.d("****", dialog.arguments.toString())
//        dialog.show(supportFragmentManager, "alllooooo")

    }

    private var onCreateCanvasResponse: Emitter.Listener = Emitter.Listener {
        Log.d("onCreateCanvasResponse", "alllooo")

        val gson = Gson()
        val obj: CanvasCreationResponse = gson.fromJson(it[0].toString())
        if(obj.isCreated) {
            Log.d("canvasCreated", "created" + canevas?.name)
            val galleryEditEvent: GalleryEditEvent = GalleryEditEvent(UserHolder.getInstance().username, canevas?.name!!, canevas?.password!!)
            val sendObj = gson.toJson(galleryEditEvent)
            Log.d("joinObj", sendObj)
            socket?.emit(SocketConstants.JOIN_CANVAS_ROOM, sendObj)


        }
    }

    private var onJoinCanvasResponse: Emitter.Listener = Emitter.Listener {
        Log.d("onJoinCanvasResponse", "alllooo")

        val gson = Gson()
        val obj: CanvasJoinResponse = gson.fromJson(it[0].toString())
        if(obj.isCanvasRoomJoined) {
            Log.d("canvasJoined", "created" + canevas?.name)
            val intent = Intent(this, DrawingActivity::class.java)
            intent.putExtra("canevas", canevas)
            startActivity(intent)
        }
    }

}
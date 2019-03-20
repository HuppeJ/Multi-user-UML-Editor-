package com.polypaint.polypaint.Fragment

import android.app.Dialog
import android.content.DialogInterface
import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.Button
import android.widget.EditText
import androidx.appcompat.app.AlertDialog
import androidx.fragment.app.DialogFragment
import com.github.nkzawa.emitter.Emitter
import com.github.nkzawa.socketio.client.Socket
import com.github.salomonbrys.kotson.fromJson
import com.google.gson.Gson
import com.polypaint.polypaint.Activity.DrawingActivity
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Holder.UserHolder
import com.polypaint.polypaint.Model.Canevas
import com.polypaint.polypaint.R
import com.polypaint.polypaint.ResponseModel.CanvasJoinResponse
import com.polypaint.polypaint.Socket.SocketConstants
import com.polypaint.polypaint.SocketReceptionModel.GalleryEditEvent
import kotlinx.android.synthetic.main.dialog_enter_drawing.*

class EnterDrawingPasswordDialogFragment : DialogFragment(){
    var canevas : Canevas? = null
    private var socket: Socket? = null

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {
        canevas = arguments?.getSerializable("canevas") as Canevas
        Log.d("onCreateDialog", canevas!!.name )



        return activity?.let {
            // Use the Builder class for convenient dialog construction
            val builder = AlertDialog.Builder(it)
            val view: View = it.layoutInflater.inflate(R.layout.dialog_enter_drawing,null)

            val app = it.application as PolyPaint
            socket = app.socket

            // socket?.on(SocketConstants.JOIN_CANVAS_ROOM_RESPONSE, onJoinCanvasResponse)

            val passwordText : EditText = view.findViewById(R.id.password_text)
            val passwordButton : Button = view.findViewById(R.id.password_button)
            passwordButton.setOnClickListener {
                validatePassword(passwordText)
            }
            builder.setView(view)
                .setPositiveButton("Close",
                    DialogInterface.OnClickListener { dialog, id ->

                    })
            // Create the AlertDialog object and return it
            builder.create()
        } ?: throw IllegalStateException("Activity cannot be null")
    }
    private fun validatePassword(passwordText : EditText){
        if(passwordText!!.text.toString().trim() == canevas!!.password){
            passwordText!!.text.clear()

            val gson = Gson()
            val galleryEditEvent: GalleryEditEvent =
                GalleryEditEvent(UserHolder.getInstance().username, canevas!!.name, canevas!!.password)
            val sendObj = gson.toJson(galleryEditEvent)
            Log.d("joinObj", sendObj)
            socket?.emit(SocketConstants.JOIN_CANVAS_ROOM, sendObj)

        }else{
            passwordText!!.text.clear()
            passwordText!!.error = "Wrong password, try again"
        }
    }

    private var onJoinCanvasResponse: Emitter.Listener = Emitter.Listener {
        Log.d("onJoinCanvasResponse", "alllooo")

        val gson = Gson()
        val obj: CanvasJoinResponse = gson.fromJson(it[0].toString())
        Log.d("onJoinCanvasResponse", obj.isCanvasRoomJoined.toString()+ " " + obj.canvasName)

        if(obj.isCanvasRoomJoined) {
            if(canevas!!.name == obj.canvasName) {
                Log.d("canvasJoined", "created" + canevas?.name)
                val intent = Intent(context, DrawingActivity::class.java)
                intent.putExtra("canevas", canevas!!)
                startActivity(intent)
            } else {
                Log.d("Erreur", "selectionCanevas")
            }
        }
    }
}
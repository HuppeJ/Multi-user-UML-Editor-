package com.polypaint.polypaint.Activity

import android.app.Activity
import android.content.Intent
import android.os.Bundle
import android.text.TextUtils
import android.util.Log
import android.view.View
import android.widget.Button
import android.widget.EditText
import android.widget.Toast
import com.github.nkzawa.emitter.Emitter
import com.github.nkzawa.socketio.client.Socket
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.R
import com.github.salomonbrys.kotson.*
import com.google.gson.JsonObject
import com.polypaint.polypaint.Socket.SocketConstants

class CreateUserActivity:Activity(){
    private var usernameView : EditText?= null
    private var passwordView : EditText?= null
    private var username: String = ""
    private var socket: Socket? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContentView(R.layout.activity_create_user)
        var app = application as PolyPaint
        socket = app.getSocket()

        usernameView = findViewById(R.id.username_text)
        passwordView = findViewById(R.id.password_text)

        var createButton: Button = findViewById(R.id.create_button)
        createButton.setOnClickListener { view: View? ->
            createUser()
        }

        socket?.on(SocketConstants.CREATE_USER_RESPONSE, onUserCreated)

    }

    override fun onDestroy() {
        super.onDestroy()
        socket?.off(SocketConstants.CREATE_USER_RESPONSE, onUserCreated)
    }

    private fun createUser() {
        usernameView?.error= null
        passwordView?.error = null

        username = usernameView?.text.toString().trim()
        if(TextUtils.isEmpty(username)){
            usernameView?.requestFocus()
            usernameView?.error = "Enter a username"
            username = ""
            return
        }

        var password = passwordView?.text.toString().trim()
        if(TextUtils.isEmpty(password)){
            passwordView?.requestFocus()
            passwordView?.error = "Enter a password"
            password = ""
            return
        }


        val obj: JsonObject = jsonObject(
            "username" to username,
            "password" to password
        )

        Log.d("*******", obj.toString())

        socket?.emit(SocketConstants.CREATE_USER, obj)

    }

    private var onUserCreated: Emitter.Listener = Emitter.Listener{
        val intent = Intent(this, ChatActivity::class.java)
        intent.putExtra("username", username)
        startActivity(intent)
    }
}
package com.polypaint.polypaint.Activity

import android.app.Activity
import android.os.Bundle
import android.text.TextUtils
import android.view.View
import android.widget.Button
import android.widget.EditText
import com.github.nkzawa.emitter.Emitter
import com.github.nkzawa.socketio.client.Socket
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.R

class LoginActivity:Activity(){
    private var usernameView : EditText?= null
    private var username: String = ""
    private var socket: Socket? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContentView(R.layout.activity_login)
        var app = application as PolyPaint
        socket = app.getSocket()

        usernameView = findViewById(R.id.username_text)
        usernameView?.setOnEditorActionListener { textView, id, keyEvent ->
            if(textView.id == R.id.username_text){
                login()
                return@setOnEditorActionListener true
            }
            false
        }

        var loginButton: Button = findViewById(R.id.login_button)
        loginButton.setOnClickListener { view: View? ->
            login()
        }

        socket?.on("newLogin", onNewLogin)

    }

    override fun onDestroy() {
        super.onDestroy()
        socket?.off("newLogin", onNewLogin)
    }

    private fun login() {
        usernameView?.error= null

        username = usernameView?.text.toString().trim()
        if(TextUtils.isEmpty(username)){
            usernameView?.requestFocus()
            usernameView?.error = "Enter a username"
            username = ""
            return
        }

        socket?.emit("logUser", username)
    }

    private var onNewLogin: Emitter.Listener = Emitter.Listener{
        //todo: do something on new login
    }
}
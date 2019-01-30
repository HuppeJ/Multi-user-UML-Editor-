package com.polypaint.polypaint.Activity

import android.app.Activity
import android.app.AlertDialog
import android.content.Intent
import android.content.pm.ActivityInfo
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
import com.google.gson.Gson
import com.google.gson.JsonObject
import com.polypaint.polypaint.Socket.SocketConstants

class LoginActivity:Activity(){
    private var usernameView : EditText?= null
    private var passwordView : EditText?= null
    private var username: String = ""
    private var socket: Socket? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        val app = application as PolyPaint
        socket = app.getSocket()
        socket?.connect()
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE


        setContentView(R.layout.activity_login)


        usernameView = findViewById(R.id.username_text)
        passwordView = findViewById(R.id.password_text)

        var loginButton: Button = findViewById(R.id.login_button)
        loginButton.setOnClickListener {
            login()
        }

        var createUserButton: Button = findViewById(R.id.create_user_button)
        createUserButton.setOnClickListener {
            createUser()
        }

        socket?.on(SocketConstants.LOGIN_USER_RESPONSE, onLogin)

    }

    override fun onDestroy() {
        super.onDestroy()
        socket?.off(SocketConstants.LOGIN_USER_RESPONSE, onLogin)
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

        socket?.emit(SocketConstants.LOGIN_USER, obj)


    }

    private fun createUser(){
        val intent = Intent(this, CreateUserActivity::class.java)
        startActivity(intent)
    }

    private var onLogin: Emitter.Listener = Emitter.Listener{
        val gson = Gson()
        val obj: Response = gson.fromJson(it[0].toString())
        if(obj.isLoginSuccessful){
            val intent = Intent(this, ChatActivity::class.java)
            intent.putExtra("username", username)
            startActivity(intent)
        } else {
            runOnUiThread{
                AlertDialog.Builder(this).setMessage("Mauvais mot de passe").show()
            }

        }
    }

    private inner class Response internal constructor(var isLoginSuccessful: Boolean)
}
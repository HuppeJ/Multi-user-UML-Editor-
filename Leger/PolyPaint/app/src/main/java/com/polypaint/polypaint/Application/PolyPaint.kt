package com.polypaint.polypaint.Application

import android.app.Application
import android.util.Log
import com.github.nkzawa.socketio.client.IO
import com.github.nkzawa.socketio.client.Socket
import com.polypaint.polypaint.Socket.SocketConstants
import java.lang.RuntimeException
import java.net.URISyntaxException

class PolyPaint: Application(){
    var socket: Socket?= null
    var uri: String = ""

    override fun onCreate() {
        super.onCreate()

    }

    fun setSocketUri (uri: String){
        this.uri = uri
//        try {
//            this.socket = IO.socket(uri)
//            Log.d("******", "**************************************")
//        } catch (e: URISyntaxException){
//            throw RuntimeException(e)
//        }
    }



}
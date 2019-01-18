package com.polypaint.polypaint.Application

import android.app.Application
import com.github.nkzawa.socketio.client.IO
import com.github.nkzawa.socketio.client.Socket
import com.polypaint.polypaint.Socket.SocketConstants
import java.lang.RuntimeException
import java.net.URISyntaxException

class PolyPaint: Application(){
    private var socket: Socket?= null

    override fun onCreate() {
        super.onCreate()
        try {
            socket = IO.socket(SocketConstants.SERVER_URL)
        } catch (e: URISyntaxException){
            throw RuntimeException(e)
        }
    }

    fun getSocket(): Socket? {
        return socket
    }



}
package com.polypaint.polypaint.Application

import android.app.Application
import android.util.Log
import com.github.nkzawa.emitter.Emitter
import com.github.nkzawa.socketio.client.IO
import com.github.nkzawa.socketio.client.Socket
import com.github.salomonbrys.kotson.fromJson
import com.google.gson.Gson
import com.polypaint.polypaint.Holder.MessagesHolder
import com.polypaint.polypaint.Model.Message
import com.polypaint.polypaint.Socket.SocketConstants
import com.polypaint.polypaint.SocketReceptionModel.MessageEvent
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

    fun triggerListeningOnMessage(){
        socket?.on(SocketConstants.MESSAGE_SENT, onNewMessage)
    }

    private fun addMessage(message: Message){
        MessagesHolder.getInstance().messages.add(message)
    }

    private var onNewMessage: Emitter.Listener = Emitter.Listener {

            Log.d("*-*****", it[0].toString())
            val gson = Gson()
            val messageEvent: MessageEvent = gson.fromJson(it[0].toString())
            val message: Message = Message(messageEvent.message, messageEvent.username, messageEvent.createdAt.toLong())

            addMessage(message)

    }



}
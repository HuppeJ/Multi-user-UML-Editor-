package com.polypaint.polypaint.Fragment

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.os.Handler
import androidx.fragment.app.Fragment
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import android.text.TextUtils
import android.text.TextWatcher
import android.util.Log
import android.view.KeyEvent
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.inputmethod.EditorInfo
import android.widget.Button
import android.widget.EditText
import android.widget.Toast
import com.github.nkzawa.emitter.Emitter
import com.github.nkzawa.socketio.client.Socket
import com.github.nkzawa.socketio.client.Socket.EVENT_DISCONNECT
import com.github.nkzawa.socketio.client.Socket.EVENT_CONNECT
import com.github.salomonbrys.kotson.fromJson
import com.github.salomonbrys.kotson.jsonObject
import com.google.gson.Gson
import com.google.gson.JsonObject
import com.polypaint.polypaint.Activity.LoginActivity
import com.polypaint.polypaint.Adapter.MessageListAdapter
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Holder.MessagesHolder
import com.polypaint.polypaint.Holder.UserHolder
import com.polypaint.polypaint.Holder.ViewShapeHolder
import com.polypaint.polypaint.Model.Link
import com.polypaint.polypaint.Model.Message
import com.polypaint.polypaint.Model.Room
import com.polypaint.polypaint.Model.User
import com.polypaint.polypaint.R
import com.polypaint.polypaint.Socket.SocketConstants
import com.polypaint.polypaint.SocketReceptionModel.ChatroomEvent
import com.polypaint.polypaint.SocketReceptionModel.LinksUpdateEvent
import com.polypaint.polypaint.SocketReceptionModel.MessageEvent
import org.json.JSONException
import org.json.JSONObject


class MessageListFragment: Fragment(){
    companion object {
        const val TAG: String = "MessageListFragment"
    }
    private var socket: Socket? = null
    private var adapter: MessageListAdapter?=null
    private var messages: MutableList<Message> = mutableListOf()
    private var messageRecyclerView : androidx.recyclerview.widget.RecyclerView? = null
    private var messageEditTextView : EditText? = null
    private var username: String? = null
    private var isTyping: Boolean = false
    private val typingHandler: Handler = Handler()
    private var isConnected: Boolean = true

    override fun onAttach(context: Context) {
        super.onAttach(context)
        username = UserHolder.getInstance().username
        adapter = MessageListAdapter(context, messages, username!!)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)


        setHasOptionsMenu(true)

//        val app = activity!!.application as PolyPaint
//        socket = app.socket
//        socket?.on(Socket.EVENT_CONNECT, onConnect)
//        socket?.on(Socket.EVENT_DISCONNECT, onDisconnect)
//        socket?.on(Socket.EVENT_CONNECT_ERROR, onConnectError)
//        socket?.on(Socket.EVENT_CONNECT_TIMEOUT, onConnectError)
//        socket?.on("messageSent", onNewMessage)
//
//        socket?.emit("joinChatroom")
//        socket?.on("user joined", onUserJoined)
//        socket?.on("user left", onUserLeft)
//        socket?.on("typing", onTyping)
//        socket?.on("stop typing", onStopTyping)
//        socket?.connect()
//
//        startSignIn()
    }

    override fun onResume() {
        super.onResume()
        val app = activity!!.application as PolyPaint
        socket = app.socket
        socket?.on(Socket.EVENT_CONNECT, onConnect)
        socket?.on(Socket.EVENT_DISCONNECT, onDisconnect)
        socket?.on(Socket.EVENT_CONNECT_ERROR, onConnectError)
        socket?.on(Socket.EVENT_CONNECT_TIMEOUT, onConnectError)

        // TODO: verifier l'event et la room qui joined.
        socket?.on(SocketConstants.MESSAGE_SENT, onNewMessage)

        socket?.emit(SocketConstants.JOIN_CHATROOM, ChatroomEvent(UserHolder.getInstance().username, "MainRoom"))
    }

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        return inflater.inflate(R.layout.fragment_message_list, container, false);
    }

    override fun onDestroy() {
        super.onDestroy()
        socket?.disconnect();
        socket?.off(Socket.EVENT_CONNECT, onConnect);
        socket?.off(Socket.EVENT_DISCONNECT, onDisconnect);
        socket?.off(Socket.EVENT_CONNECT_ERROR, onConnectError);
        socket?.off(Socket.EVENT_CONNECT_TIMEOUT, onConnectError);
        socket?.off("messageSent", onNewMessage);
//        socket?.off("user joined", onUserJoined);
//        socket?.off("user left", onUserLeft);
//        socket?.off("typing", onTyping);
//        socket?.off("stop typing", onStopTyping);
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        messageRecyclerView = view.findViewById(R.id.reyclerview_message_list)
        messageRecyclerView?.layoutManager = androidx.recyclerview.widget.LinearLayoutManager(activity)
        messageRecyclerView?.adapter = adapter

        messageEditTextView = view.findViewById(R.id.edittext_chatbox)
        messageEditTextView?.setOnEditorActionListener { textView, actionId, keyEvent ->
            return@setOnEditorActionListener when (actionId) {
                EditorInfo.IME_ACTION_SEND -> {
                    trySend()
                    true
                }
                else -> false
            }
        }

        var sendButton: Button = view.findViewById(R.id.button_chatbox_send)
        sendButton.setOnClickListener {
                trySend()
        }

    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if(Activity.RESULT_OK != resultCode){
            activity?.finish()
            return
        }
       // user = User(data!!.getStringExtra("username"))
    }

    private fun trySend(){
        val localSocket = socket

        if(localSocket == null || !localSocket.connected()){
            Toast.makeText(activity?.applicationContext, "not  connected", Toast.LENGTH_SHORT).show()
            return
        }

        isTyping = false

        val message: String = messageEditTextView?.text.toString().trim()
        if(TextUtils.isEmpty(message)){
            messageEditTextView?.requestFocus()
            return
        }

        messageEditTextView?.setText("")
        Log.d("*****", username)
        val messageObject = Message(message, username!!, System.currentTimeMillis() )
        //addMessage(messageObject)

//        val obj: JsonObject = jsonObject(
//            "sender" to username,
//            "text" to message,
//            "createdAt" to System.currentTimeMillis()
//        )


        var obj: String =""
        val gson = Gson()
        val response: MessageEvent = MessageEvent(UserHolder.getInstance().username, "MainRoom", messageObject.createdAt.toString(), messageObject.text)
        obj = gson.toJson(response)


        if(obj !="") {
            Log.d("emitingSendMessage", obj)
            socket?.emit(SocketConstants.SEND_MESSAGE, obj)
        }


    }

    private fun addMessage(message: Message){
        messages.add(message)
        adapter?.notifyItemInserted(messages.size -1)
        scrollToBottom()
    }

    private fun scrollToBottom(){
        messageRecyclerView?.scrollToPosition(adapter!!.itemCount-1)
    }

    private var onConnect:Emitter.Listener = Emitter.Listener {
        activity?.runOnUiThread {
            if(!isConnected){
                socket?.emit("add user", username)
                Toast.makeText(activity?.applicationContext, "Connected", Toast.LENGTH_SHORT).show()
                isConnected = true
            }
        }
    }

    private var onDisconnect:Emitter.Listener = Emitter.Listener {
        activity?.runOnUiThread {
            isConnected = false
            Toast.makeText(activity?.applicationContext, "Disconnected", Toast.LENGTH_SHORT).show()
        }
    }

    private var onConnectError:Emitter.Listener = Emitter.Listener {
        activity?.runOnUiThread {
            Toast.makeText(activity?.applicationContext, "Connection error", Toast.LENGTH_SHORT).show()
        }
    }

    private var onNewMessage:Emitter.Listener = Emitter.Listener {
        activity?.runOnUiThread {
            Log.d("*-*****", it[0].toString())
            val gson = Gson()
            val messageEvent: MessageEvent = gson.fromJson(it[0].toString())
            val message: Message = Message(messageEvent.message, messageEvent.username, messageEvent.createdAt.toLong())
            //val data: JSONObject = it[0] as JSONObject
            /*var username: String? = null
            var message: String? = null
            var createdAt: Long? = null
            try {
                username = data.getString("username")
                message = data.getString("message")
                createdAt = data.getLong("createdAt")
            } catch (e: JSONException){
                Log.e(TAG, e.message)
            }

            val messageObject = Message(message!!, User(username!!), createdAt!!)*/
            addMessage(message)
        }
    }

    fun changeRoom(room: Room){
        Log.d("------", room.name)
        messages = MessagesHolder.getInstance().messagesByRoom[room.name]!!
        adapter?.messageList = messages
        adapter?.notifyDataSetChanged()
    }
}

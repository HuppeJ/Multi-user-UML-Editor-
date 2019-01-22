package com.polypaint.polypaint.Fragment

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.os.Handler
import android.support.v4.app.Fragment
import android.support.v7.widget.LinearLayoutManager
import android.support.v7.widget.RecyclerView
import android.text.TextUtils
import android.text.TextWatcher
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.EditText
import android.widget.Toast
import com.github.nkzawa.emitter.Emitter
import com.github.nkzawa.socketio.client.Socket
import com.github.nkzawa.socketio.client.Socket.EVENT_DISCONNECT
import com.github.nkzawa.socketio.client.Socket.EVENT_CONNECT
import com.polypaint.polypaint.Adapter.MessageListAdapter
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Model.Message
import com.polypaint.polypaint.Model.User
import com.polypaint.polypaint.R
import org.json.JSONException
import org.json.JSONObject


class MessageListFragment: Fragment(){
    companion object {
        private const val TAG: String = "MessageListFragment"
    }
    private var socket: Socket? = null
    private var adapter: RecyclerView.Adapter<RecyclerView.ViewHolder>?=null
    private val messages: MutableList<Message> = mutableListOf()
    private var messageRecyclerView : RecyclerView? = null
    private var messageEditTextView : EditText? = null
    private var username: String? = null
    private var isTyping: Boolean = false
    private val typingHandler: Handler = Handler()
    private var isConnected: Boolean = true

    override fun onAttach(context: Context?) {
        super.onAttach(context)
        super.onAttach(context)
        username = activity?.intent?.getStringExtra("username")
        adapter = MessageListAdapter(context!!, messages, User(username!!))
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)


        setHasOptionsMenu(true)

        val app = activity!!.application as PolyPaint
        socket = app.getSocket()
        socket?.on(Socket.EVENT_CONNECT, onConnect)
        socket?.on(Socket.EVENT_DISCONNECT, onDisconnect)
        socket?.on(Socket.EVENT_CONNECT_ERROR, onConnectError)
        socket?.on(Socket.EVENT_CONNECT_TIMEOUT, onConnectError)
        socket?.on("new message", onNewMessage)
//        socket?.on("user joined", onUserJoined)
//        socket?.on("user left", onUserLeft)
//        socket?.on("typing", onTyping)
//        socket?.on("stop typing", onStopTyping)
//        socket?.connect()
//
//        startSignIn()
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
        socket?.off("new message", onNewMessage);
//        socket?.off("user joined", onUserJoined);
//        socket?.off("user left", onUserLeft);
//        socket?.off("typing", onTyping);
//        socket?.off("stop typing", onStopTyping);
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        messageRecyclerView = view.findViewById(R.id.reyclerview_message_list)
        messageRecyclerView?.layoutManager = LinearLayoutManager(activity)
        messageRecyclerView?.adapter = adapter

        messageEditTextView = view.findViewById(R.id.edittext_chatbox)
        messageEditTextView?.setOnEditorActionListener { textView, i, keyEvent ->
            if(id == R.id.edittext_chatbox){
                trySend()
                return@setOnEditorActionListener true
            }
            false
        }

        var sendButton: Button = view.findViewById(R.id.button_chatbox_send)
        sendButton.setOnClickListener() {view->
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
        if(! socket!!.connected()){
            Toast.makeText(activity?.applicationContext, "not  connected", Toast.LENGTH_LONG).show()
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
        val messageObject = Message(message, User(username!!), System.currentTimeMillis() )
        addMessage(messageObject)

        socket?.emit("new message", messageObject)
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
                Toast.makeText(activity?.applicationContext, "Connected", Toast.LENGTH_LONG).show()
                isConnected = true
            }
        }
    }

    private var onDisconnect:Emitter.Listener = Emitter.Listener {
        activity?.runOnUiThread {
            isConnected = false
            Toast.makeText(activity?.applicationContext, "Disconnected", Toast.LENGTH_LONG).show()
        }
    }

    private var onConnectError:Emitter.Listener = Emitter.Listener {
        activity?.runOnUiThread {
            Toast.makeText(activity?.applicationContext, "Connection error", Toast.LENGTH_LONG).show()
        }
    }

    private var onNewMessage:Emitter.Listener = Emitter.Listener {
        activity?.runOnUiThread {
            val data: JSONObject = it[0] as JSONObject
            var username: String? = null
            var message: String? = null
            var createdAt: Long? = null
            try {
                username = data.getString("username")
                message = data.getString("message")
                createdAt = data.getLong("createdAt")
            } catch (e: JSONException){
                Log.e(TAG, e.message)
            }

            val messageObject = Message(message!!, User(username!!), createdAt!!)
            addMessage(messageObject)
        }
    }
}

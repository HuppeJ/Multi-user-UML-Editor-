package com.polypaint.polypaint.Fragment

import android.content.Context
import android.os.Bundle
import android.support.v4.app.Fragment
import android.support.v7.widget.LinearLayoutManager
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import com.github.nkzawa.socketio.client.Socket
import com.github.nkzawa.socketio.client.Socket.EVENT_DISCONNECT
import com.github.nkzawa.socketio.client.Socket.EVENT_CONNECT
import com.polypaint.polypaint.Adapter.MessageListAdapter
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Model.Message


class MessageListFragment: Fragment(){
    private var socket: Socket? = null
    private var adapter: RecyclerView.Adapter<RecyclerView.ViewHolder>?=null
    private var messages: List<Message>? = ArrayList()
    private var messageRecyclerView : RecyclerView? = null

    override fun onAttach(context: Context?) {
        super.onAttach(context)
        super.onAttach(context);
        adapter = MessageListAdapter(context!!, messages!!);
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
        socket?.on("user joined", onUserJoined)
        socket?.on("user left", onUserLeft)
        socket?.on("typing", onTyping)
        socket?.on("stop typing", onStopTyping)
        socket?.connect()

        startSignIn()
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
        socket?.off("user joined", onUserJoined);
        socket?.off("user left", onUserLeft);
        socket?.off("typing", onTyping);
        socket?.off("stop typing", onStopTyping);
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        messageRecyclerView = view.findViewById(R.id.reyclerview_message_list)
        messageRecyclerView?.layoutManager = LinearLayoutManager(activity)
        messageRecyclerView?.adapter = adapter
    }
}

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
import com.polypaint.polypaint.Adapter.RoomsListAdapter
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Model.Message
import com.polypaint.polypaint.Model.Room
import com.polypaint.polypaint.Model.User
import com.polypaint.polypaint.R
import org.json.JSONException
import org.json.JSONObject


class RoomsListFragment: Fragment(){
    companion object {
        private const val TAG: String = "RoomsListFragment"
    }
    private var socket: Socket? = null
    private var adapter: RecyclerView.Adapter<RecyclerView.ViewHolder>?=null
    private var roomsRecyclerView : RecyclerView? = null
    private var username: String? = null
    private var isTyping: Boolean = false
    private val typingHandler: Handler = Handler()
    private var isConnected: Boolean = true

    override fun onAttach(context: Context?) {
        super.onAttach(context)
        super.onAttach(context)
        var roomsList: MutableList<Room> = mutableListOf()
        roomsList.add(Room("room"))
        roomsList.add(Room("room2"))
        roomsList.add(Room("room3"))
        username = activity?.intent?.getStringExtra("username")
        adapter = RoomsListAdapter(context!!, roomsList, User(username!!))
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)


        setHasOptionsMenu(true)
    }

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        return inflater.inflate(R.layout.fragment_rooms_list, container, false);
    }

    override fun onDestroy() {
        super.onDestroy()
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        roomsRecyclerView = view.findViewById(R.id.reyclerview_rooms_list)
        roomsRecyclerView?.layoutManager = LinearLayoutManager(activity)
        roomsRecyclerView?.adapter = adapter


    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if(Activity.RESULT_OK != resultCode){
            activity?.finish()
            return
        }
       // user = User(data!!.getStringExtra("username"))
    }

}

package com.polypaint.polypaint.Fragment

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.os.Handler
import androidx.fragment.app.Fragment
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ListView
import android.widget.Toast
import com.github.nkzawa.socketio.client.Socket
import com.polypaint.polypaint.Adapter.RoomsListAdapter
import com.polypaint.polypaint.Model.Room
import com.polypaint.polypaint.Model.User
import com.polypaint.polypaint.R


class RoomsListFragment: Fragment(){
    companion object {
        private const val TAG: String = "RoomsListFragment"
    }
    private var socket: Socket? = null
    private var adapter: androidx.recyclerview.widget.RecyclerView.Adapter<androidx.recyclerview.widget.RecyclerView.ViewHolder>?=null
    private var roomsRecyclerView : androidx.recyclerview.widget.RecyclerView? = null
    private var username: String? = null
    private var isTyping: Boolean = false
    private val typingHandler: Handler = Handler()
    private var isConnected: Boolean = true

    var callback: OnRoomSelectedListener? = null

    fun setOnRoomSelectedListener(activity: Activity) {
        callback = activity as OnRoomSelectedListener
    }

    // Container Activity must implement this interface
    interface OnRoomSelectedListener {
        fun onRoomSelected(room: Room)
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)
        var roomsList: MutableList<Room> = mutableListOf()
        roomsList.add(Room("room"))
        roomsList.add(Room("room2"))
        roomsList.add(Room("room3"))
        username = activity?.intent?.getStringExtra("username")
        adapter = RoomsListAdapter(context, roomsList, User(username!!), object: RoomsListAdapter.OnItemClickListener{
            override fun onItemClick(room: Room) {
                callback?.onRoomSelected(room)
            }
        })
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
        roomsRecyclerView?.layoutManager = androidx.recyclerview.widget.LinearLayoutManager(activity)
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

    /*override fun onListItemClick(l: ListView?, v: View?, position: Int, id: Long) {
        super.onListItemClick(l, v, position, id)
        callback?.onRoomSelected(position)
    }*/

}

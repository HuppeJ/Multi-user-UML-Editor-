package com.polypaint.polypaint.Activity

import android.app.Fragment
import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.widget.LinearLayoutManager
import android.support.v7.widget.RecyclerView
import android.view.View
import android.widget.EditText
import android.widget.Toast
import com.polypaint.polypaint.Model.Message
import com.polypaint.polypaint.Adapter.MessageListAdapter
import com.polypaint.polypaint.Adapter.RoomsListAdapter
import com.polypaint.polypaint.Fragment.MessageListFragment
import com.polypaint.polypaint.Fragment.RoomsListFragment
import com.polypaint.polypaint.Model.Room
import com.polypaint.polypaint.R
import com.polypaint.polypaint.Model.User


class ChatActivity : AppCompatActivity(), RoomsListFragment.OnRoomSelectedListener{
    private var mMessageRecycler: RecyclerView? = null
    private var mMessageAdapter: MessageListAdapter? = null
    private var messageList: ArrayList<Message> = ArrayList()

    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContentView(R.layout.activity_chat)

        val ft = fragmentManager.beginTransaction()
        ft.add(R.id.list_container, RoomsListFragment())
        ft.add(R.id.details_container, MessageListFragment(), MessageListFragment.TAG)
        ft.commit()

    }

    override fun onAttachFragment(fragment: Fragment) {
        if (fragment is RoomsListFragment) {
            fragment.setOnRoomSelectedListener(this)
        }
    }

    override fun onRoomSelected(room: Room) {
        //Toast.makeText(this, room.name, Toast.LENGTH_LONG).show()
        val messageListFragment = fragmentManager.findFragmentByTag(MessageListFragment.TAG) as MessageListFragment
        //Toast.makeText(this, messageListFragment.id, Toast.LENGTH_LONG).show()

        messageListFragment.changeRoom(room)
    }
}
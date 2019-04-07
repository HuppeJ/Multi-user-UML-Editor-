package com.polypaint.polypaint.Activity

import androidx.fragment.app.Fragment
import android.content.Intent
import android.content.pm.ActivityInfo
import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import android.view.View
import android.widget.EditText
import android.widget.Toast
import androidx.appcompat.widget.Toolbar
import co.zsmb.materialdrawerkt.builders.drawer
import co.zsmb.materialdrawerkt.builders.footer
import co.zsmb.materialdrawerkt.draweritems.badgeable.primaryItem
import co.zsmb.materialdrawerkt.draweritems.badgeable.secondaryItem
import co.zsmb.materialdrawerkt.draweritems.divider
import com.github.nkzawa.socketio.client.Socket
import com.mikepenz.materialdrawer.Drawer
import com.polypaint.polypaint.Model.Message
import com.polypaint.polypaint.Adapter.MessageListAdapter
import com.polypaint.polypaint.Adapter.RoomsListAdapter
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Fragment.MessageListFragment
import com.polypaint.polypaint.Fragment.RoomsListFragment
import com.polypaint.polypaint.Model.Room
import com.polypaint.polypaint.R
import com.polypaint.polypaint.Model.User
import kotlinx.android.synthetic.main.toolbar.*

class ChatActivity : AppCompatActivity(), RoomsListFragment.OnRoomSelectedListener{
    private var mMessageRecycler: androidx.recyclerview.widget.RecyclerView? = null
    private var mMessageAdapter: MessageListAdapter? = null
    private var messageList: ArrayList<Message> = ArrayList()
    private var drawer: Drawer? = null

    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE

        setContentView(R.layout.activity_chat)

        val activityToolbar : Toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(activityToolbar)
        help_button.visibility = View.GONE


        drawer = drawer {
            primaryItem("Gallery") {
                icon = R.drawable.ic_picture
                onClick { _ ->
                    val intent = Intent(this@ChatActivity, GalleryActivity::class.java)
                    startActivity(intent)
                    true
                }
            }
            primaryItem("Chat") {
                icon = R.drawable.ic_chat
                onClick { _ ->
                    false
                }
            }

            toolbar = activityToolbar
        }
        drawer?.setSelectionAtPosition(2, false)

        val ft = supportFragmentManager.beginTransaction()
        //ft.add(R.id.list_container, RoomsListFragment())
        ft.add(R.id.details_container, MessageListFragment(), MessageListFragment.TAG)
        ft.commit()

    }

    override fun onAttachFragment(fragment: Fragment) {
        if (fragment is RoomsListFragment) {
            fragment.setOnRoomSelectedListener(this)
        }
    }

    override fun onRoomSelected(room: Room) {
        //Toast.makeText(this, room.name, Toast.LENGTH_SHORT).show()
        val messageListFragment = supportFragmentManager.findFragmentByTag(MessageListFragment.TAG) as MessageListFragment
        //Toast.makeText(this, messageListFragment.id, Toast.LENGTH_SHORT).show()

        messageListFragment.changeRoom(room)
    }

    override fun onBackPressed() {
        if(drawer!!.isDrawerOpen){
            drawer?.closeDrawer()
        } else {
            super.onBackPressed()
            /*val app = application as PolyPaint
            val socket: Socket? = app.socket
            socket?.disconnect()
            val intent = Intent(this, LoginActivity::class.java)
            intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TOP
            startActivity(intent)
            finish()*/
        }
    }
}
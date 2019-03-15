package com.polypaint.polypaint.Activity

import android.content.Intent
import android.content.pm.ActivityInfo
import android.os.Bundle
import android.util.Log
import android.widget.Button
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import co.zsmb.materialdrawerkt.builders.drawer
import co.zsmb.materialdrawerkt.builders.footer
import co.zsmb.materialdrawerkt.draweritems.badgeable.primaryItem
import co.zsmb.materialdrawerkt.draweritems.badgeable.secondaryItem
import com.github.nkzawa.emitter.Emitter
import com.github.nkzawa.socketio.client.Socket
import com.github.salomonbrys.kotson.fromJson
import com.google.gson.Gson
import com.mikepenz.materialdrawer.Drawer
import com.polypaint.polypaint.Adapter.ImageListAdapter
import com.polypaint.polypaint.Adapter.RoomsListAdapter
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Holder.UserHolder
import com.polypaint.polypaint.Model.BasicShape
import com.polypaint.polypaint.Model.Canevas
import com.polypaint.polypaint.Model.Link
import com.polypaint.polypaint.Model.Room
import com.polypaint.polypaint.R
import com.polypaint.polypaint.ResponseModel.CanvasJoinResponse
import com.polypaint.polypaint.Socket.SocketConstants
import com.polypaint.polypaint.SocketReceptionModel.GalleryEditEvent
import kotlinx.android.synthetic.main.activity_gallery.*

class GalleryActivity:AppCompatActivity(){
    private var drawer: Drawer? = null
    private var adapterPrivate: ImageListAdapter? = null
    private var adapterPublic: ImageListAdapter? = null
    private var canevasPrivate: MutableList<Canevas> = mutableListOf()
    private var canevasPublic: MutableList<Canevas> = mutableListOf()
    private var socket: Socket? = null
    private var selectedCanevas: Canevas? = null

    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        requestedOrientation = ActivityInfo.SCREEN_ORIENTATION_LANDSCAPE

        setContentView(R.layout.activity_gallery)

        val app = application as PolyPaint
        socket = app.socket

        socket?.on(SocketConstants.JOIN_CANVAS_ROOM_RESPONSE, onJoinCanvasResponse)

        val activityToolbar : Toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(activityToolbar)


        drawer = drawer {
            primaryItem("Gallery") {
                icon = R.drawable.message_rectangle_r
                onClick { _ ->
                    false
                }
            }
            primaryItem("Chat") {
                icon = R.drawable.message_rectangle_r
                onClick { _ ->
                    val intent = Intent(this@GalleryActivity, ChatActivity::class.java)
                    startActivity(intent)
                    true
                }
                selectable = false
            }
            footer{
                secondaryItem("Settings") {
                    icon = R.drawable.message_rectangle_r
                }
            }

            toolbar = activityToolbar
        }

        var newDrawing: Button = findViewById(R.id.create_drawing_button)
        newDrawing.setOnClickListener {
            val intent = Intent(this@GalleryActivity, CreateDrawingActivity::class.java)
            startActivity(intent)
        }

        /*val ft = supportFragmentManager.beginTransaction()
        //ft.add(R.id.list_container, RoomsListFragment())
        ft.add(R.id.details_container, MessageListFragment(), MessageListFragment.TAG)
        ft.commit()*/



        requestPrivateCanevas()
        requestPublicCanevas()


        adapterPrivate = ImageListAdapter(this, canevasPrivate, UserHolder.getInstance().username, object: ImageListAdapter.OnItemClickListener{
            override fun onItemClick(canevas: Canevas) {

                // TODO : pop the modal


                selectedCanevas = canevas
                val gson = Gson()
                val galleryEditEvent: GalleryEditEvent = GalleryEditEvent(UserHolder.getInstance().username, canevas.name, canevas.password)
                val sendObj = gson.toJson(galleryEditEvent)
                Log.d("joinObj", sendObj)
                socket?.emit(SocketConstants.JOIN_CANVAS_ROOM, sendObj)
            }
        })
        adapterPublic = ImageListAdapter(this, canevasPublic, UserHolder.getInstance().username, object: ImageListAdapter.OnItemClickListener{
            override fun onItemClick(canevas: Canevas) {


                selectedCanevas = canevas
                val gson = Gson()
                val galleryEditEvent: GalleryEditEvent = GalleryEditEvent(UserHolder.getInstance().username, canevas.name, canevas.password)
                val sendObj = gson.toJson(galleryEditEvent)
                Log.d("joinObj", sendObj)
                socket?.emit(SocketConstants.JOIN_CANVAS_ROOM, sendObj)
            }
        })

        private_gallery_recycler_view?.layoutManager = androidx.recyclerview.widget.LinearLayoutManager(this)
        private_gallery_recycler_view?.adapter = adapterPrivate

        public_gallery_recycler_view?.layoutManager = androidx.recyclerview.widget.LinearLayoutManager(this)
        public_gallery_recycler_view?.adapter = adapterPublic
    }

    private fun requestPrivateCanevas(){
        canevasPrivate.add(Canevas("ID","qwe","AUTHOR","aa",0,"", ArrayList<BasicShape>(), ArrayList<Link>()))
        canevasPrivate.add(Canevas("ID","qwe","AUTHOR","aa",0,"", ArrayList<BasicShape>(), ArrayList<Link>()))

    }

    private fun requestPublicCanevas(){
        canevasPublic.add(Canevas("ID","qwe","AUTHOR","aa",1,"", ArrayList<BasicShape>(), ArrayList<Link>()))
        canevasPublic.add(Canevas("ID","qwe","AUTHOR","p",1,"", ArrayList<BasicShape>(), ArrayList<Link>()))

    }

    private var onJoinCanvasResponse: Emitter.Listener = Emitter.Listener {
        Log.d("onJoinCanvasResponse", "alllooo")

        val gson = Gson()
        val obj: CanvasJoinResponse = gson.fromJson(it[0].toString())
        Log.d("onJoinCanvasResponse", obj.isCanvasRoomJoined.toString()+ " " + obj.canvasName)

        if(obj.isCanvasRoomJoined) {
            if(selectedCanevas != null && selectedCanevas?.name == obj.canvasName) {
                Log.d("canvasJoined", "created" + obj.canvasName)
                val intent = Intent(this, DrawingActivity::class.java)
                intent.putExtra("canevas", selectedCanevas)
                startActivity(intent)
            } else {
                Log.d("Erreur", "selectionCanevas")
            }
        }
    }

    override fun onBackPressed() {
        if(drawer!!.isDrawerOpen){
            drawer?.closeDrawer()
        } else {
            socket?.off()
            socket?.disconnect()
            val intent = Intent(this, LoginActivity::class.java)
            intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TOP
            startActivity(intent)
            finish()
        }
    }
}
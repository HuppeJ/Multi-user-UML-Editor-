package com.polypaint.polypaint

import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.widget.LinearLayoutManager
import android.support.v7.widget.RecyclerView

class MessageListActivity : AppCompatActivity(){
    private var mMessageRecycler: RecyclerView? = null
    private var mMessageAdapter: MessageListAdapter? = null

    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContentView(R.layout.activity_message_list)

        var messageList: ArrayList<Message> = ArrayList();
        messageList.add(Message("allo", User("bob", 1),System.currentTimeMillis()))
        messageList.add(Message("allo", User("bob", 1),System.currentTimeMillis()))
        messageList.add(Message("allo", User("bobby", 2),System.currentTimeMillis()))
        messageList.add(Message("allo", User("bob", 1),System.currentTimeMillis()))



        mMessageRecycler = findViewById(R.id.reyclerview_message_list)
        mMessageAdapter = MessageListAdapter(this, messageList)
        mMessageRecycler?.layoutManager = LinearLayoutManager(this)
        mMessageRecycler?.adapter = mMessageAdapter
    }

}
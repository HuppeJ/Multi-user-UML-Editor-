package com.polypaint.polypaint

import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.widget.LinearLayoutManager
import android.support.v7.widget.RecyclerView
import android.view.View
import android.widget.EditText

class MessageListActivity : AppCompatActivity(){
    private var mMessageRecycler: RecyclerView? = null
    private var mMessageAdapter: MessageListAdapter? = null
    private var messageList: ArrayList<Message> = ArrayList()

    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContentView(R.layout.activity_message_list)

        messageList.add(Message("allo", User("bob", 1),System.currentTimeMillis()))
        messageList.add(Message("allo", User("bob", 1),System.currentTimeMillis()))
        messageList.add(Message("allo", User("bobby", 2),System.currentTimeMillis()))
        messageList.add(Message("allo", User("bob", 1),System.currentTimeMillis()))



        mMessageRecycler = findViewById(R.id.reyclerview_message_list)
        mMessageAdapter = MessageListAdapter(this, messageList)
        mMessageRecycler?.layoutManager = LinearLayoutManager(this)
        mMessageRecycler?.adapter = mMessageAdapter
    }

    fun sendMessage(view: View){

        var editText = findViewById<EditText>(R.id.edittext_chatbox)
        messageList.add(Message( editText.text.toString() , User("bob", 1), System.currentTimeMillis()))
        mMessageAdapter?.notifyDataSetChanged()
    }
}
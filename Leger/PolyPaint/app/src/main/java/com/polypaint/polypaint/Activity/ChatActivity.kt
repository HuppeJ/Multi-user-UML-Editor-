package com.polypaint.polypaint.Activity

import android.os.Bundle
import android.support.v7.app.AppCompatActivity
import android.support.v7.widget.LinearLayoutManager
import android.support.v7.widget.RecyclerView
import android.view.View
import android.widget.EditText
import com.polypaint.polypaint.Model.Message
import com.polypaint.polypaint.Adapter.MessageListAdapter
import com.polypaint.polypaint.R
import com.polypaint.polypaint.Model.User

class ChatActivity : AppCompatActivity(){
    private var mMessageRecycler: RecyclerView? = null
    private var mMessageAdapter: MessageListAdapter? = null
    private var messageList: ArrayList<Message> = ArrayList()

    override fun onCreate (savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContentView(R.layout.activity_chat)

        messageList.add(
            Message(
                "allo",
                User("bob"),
                System.currentTimeMillis()
            )
        )
        messageList.add(
            Message(
                "allo",
                User("bob"),
                System.currentTimeMillis()
            )
        )
        messageList.add(
            Message(
                "allo",
                User("bobby"),
                System.currentTimeMillis()
            )
        )
        messageList.add(
            Message(
                "allo",
                User("bob"),
                System.currentTimeMillis()
            )
        )



//        mMessageRecycler = findViewById(R.id.reyclerview_message_list)
//        mMessageAdapter = MessageListAdapter(this, messageList)
//        mMessageRecycler?.layoutManager = LinearLayoutManager(this)
//        mMessageRecycler?.adapter = mMessageAdapter
    }

    fun sendMessage(view: View){

        var editText: EditText = findViewById(R.id.edittext_chatbox)
        messageList.add(
            Message(
                editText.text.toString(),
                User("bob"),
                System.currentTimeMillis()
            )
        )
        editText.setText("")
        mMessageAdapter?.notifyDataSetChanged()
    }
}
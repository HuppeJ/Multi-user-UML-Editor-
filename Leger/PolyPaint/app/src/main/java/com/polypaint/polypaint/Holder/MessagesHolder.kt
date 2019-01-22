package com.polypaint.polypaint.Holder

import com.polypaint.polypaint.Model.Message
import com.polypaint.polypaint.Model.User

class MessagesHolder() {
    var messagesByRoom: MutableMap<String, MutableList<Message>> = mutableMapOf()

    init {
        var list1: MutableList<Message> = mutableListOf(
            Message("allo11", User("bob"), System.currentTimeMillis()),
            Message("allo2", User("bob"), System.currentTimeMillis()),
            Message("allo3", User("bobby"), System.currentTimeMillis())
        )
        var list2: MutableList<Message> = mutableListOf(
            Message("allo21", User("bob"), System.currentTimeMillis()),
            Message("allo2", User("bob"), System.currentTimeMillis()),
            Message("allo3", User("bobby"), System.currentTimeMillis())
        )
        var list3: MutableList<Message> = mutableListOf(
            Message("allo31", User("bob"), System.currentTimeMillis()),
            Message("allo2", User("bob"), System.currentTimeMillis()),
            Message("allo3", User("bobby"), System.currentTimeMillis())
        )

        messagesByRoom["1"] = list1
        messagesByRoom["2"] = list2
        messagesByRoom["3"] = list3

    }
    companion object {
        private val messagesHolder: MessagesHolder = MessagesHolder()

        fun getInstance(): MessagesHolder{
            return messagesHolder
        }
    }

}
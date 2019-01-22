package com.polypaint.polypaint.Holder

import com.polypaint.polypaint.Model.Message

class MessagesHolder {
    var messagesByRoom: MutableMap<String, List<Message>> = mutableMapOf()
    private val messagesHolder: MessagesHolder = MessagesHolder()

    fun getInstance(): MessagesHolder{
        return messagesHolder
    }
}
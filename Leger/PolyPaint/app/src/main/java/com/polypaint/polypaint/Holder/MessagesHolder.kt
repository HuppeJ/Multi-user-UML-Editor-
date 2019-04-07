package com.polypaint.polypaint.Holder

import com.polypaint.polypaint.Model.Message
import com.polypaint.polypaint.Model.User

class MessagesHolder() {
    var messages: ArrayList<Message> = ArrayList<Message>()

    companion object {
        private val messagesHolder: MessagesHolder = MessagesHolder()

        fun getInstance(): MessagesHolder{
            return messagesHolder
        }
    }

}
package com.polypaint.polypaint.Adapter

import android.content.Context
import android.graphics.Color
import android.graphics.PorterDuff
import android.graphics.drawable.Drawable
import android.support.v4.content.ContextCompat
import android.support.v7.widget.RecyclerView
import android.text.format.DateUtils
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import com.polypaint.polypaint.Holder.MessagesHolder
import com.polypaint.polypaint.Model.Message
import com.polypaint.polypaint.Model.Room
import com.polypaint.polypaint.Model.User
import com.polypaint.polypaint.R

class RoomsListAdapter (var context: Context, var roomsList:MutableList<Room>, val user: User) : RecyclerView.Adapter<RecyclerView.ViewHolder>(){
    companion object {
        private const val VIEW_TYPE_MESSAGE_SENT = 1
        private const val VIEW_TYPE_MESSAGE_RECEIVED = 2
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): RecyclerView.ViewHolder {

        val view : View = LayoutInflater.from(parent.context).inflate(R.layout.item_room, parent, false)
        return if (viewType == VIEW_TYPE_MESSAGE_SENT) {
            SentMessageHolder(view)
        } else {
            ReceivedMessageHolder(view)
        }
    }

    override fun getItemCount(): Int {
        return roomsList.size
    }

    override fun onBindViewHolder(holder: RecyclerView.ViewHolder, position: Int) {
        //val message = MessagesHolder.getInstance().messagesByRoom[roomsList[position].name]?.last()
        val message: Message = Message("allo", User("bob"), System.currentTimeMillis())
        when (holder.itemViewType) {
            VIEW_TYPE_MESSAGE_SENT -> (holder as SentMessageHolder).bind(message)
            VIEW_TYPE_MESSAGE_RECEIVED -> (holder as ReceivedMessageHolder).bind(message)
        }
    }

    override fun getItemViewType(position: Int): Int {
        val room = roomsList[position]

        return VIEW_TYPE_MESSAGE_SENT

        /*return if (MessagesHolder.getInstance().messagesByRoom[room.name]?.last()?.sender?.username  == user.username) {
            VIEW_TYPE_MESSAGE_SENT
        } else {
            VIEW_TYPE_MESSAGE_RECEIVED
        }*/
    }

    private inner class SentMessageHolder internal constructor(itemView: View) : RecyclerView.ViewHolder(itemView) {
        internal var messageText: TextView = itemView.findViewById(R.id.text_message_body) as TextView
        internal var timeText: TextView = itemView.findViewById(R.id.text_message_time) as TextView


        internal fun bind(message: Message) {
            messageText.text = message.text
            timeText.text = DateUtils.formatDateTime(context, message.createdAt, DateUtils.FORMAT_SHOW_TIME)
        }
    }

    private inner class ReceivedMessageHolder internal constructor(itemView: View) : RecyclerView.ViewHolder(itemView) {
        internal var messageText: TextView = itemView.findViewById(R.id.text_message_body) as TextView
        internal var timeText: TextView = itemView.findViewById(R.id.text_message_time) as TextView
        internal var nameText:TextView = itemView.findViewById(R.id.text_message_name) as TextView

        internal fun bind(message: Message) {
            messageText.text = message.text
            timeText.text = DateUtils.formatDateTime(context, message.createdAt, DateUtils.FORMAT_SHOW_TIME)
            nameText.text = message.sender.username
        }
    }

}
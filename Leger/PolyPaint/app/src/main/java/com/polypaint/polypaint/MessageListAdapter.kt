package com.polypaint.polypaint

import android.content.Context
import android.support.v7.widget.RecyclerView
import android.text.format.DateUtils
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView

class MessageListAdapter (var context: Context, var messageList: List<Message>) : RecyclerView.Adapter<RecyclerView.ViewHolder>(){

    companion object {
        private const val VIEW_TYPE_MESSAGE_SENT = 1
        private const val VIEW_TYPE_MESSAGE_RECEIVED = 2
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): RecyclerView.ViewHolder {
        val view: View

        return if (viewType == VIEW_TYPE_MESSAGE_SENT) {
            view = LayoutInflater.from(parent.context).inflate(R.layout.item_message_sent, parent, false)
            SentMessageHolder(view)
        } else {
            view = LayoutInflater.from(parent.context).inflate(R.layout.item_message_received, parent, false)
            ReceivedMessageHolder(view)
        }
    }

    override fun getItemCount(): Int {
        return messageList.size
    }

    override fun onBindViewHolder(holder: RecyclerView.ViewHolder, position: Int) {
        val message = messageList[position]

        when (holder.itemViewType) {
            VIEW_TYPE_MESSAGE_SENT -> (holder as SentMessageHolder).bind(message)
            VIEW_TYPE_MESSAGE_RECEIVED -> (holder as ReceivedMessageHolder).bind(message)
        }
    }

    override fun getItemViewType(position: Int): Int {
        val message = messageList[position]

        return if (message.sender.id == 1L) {
            VIEW_TYPE_MESSAGE_SENT
        } else {
            VIEW_TYPE_MESSAGE_RECEIVED
        }
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
            nameText.text = message.sender.nickname
        }
    }

}
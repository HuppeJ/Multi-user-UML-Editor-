package com.polypaint.polypaint.Adapter

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import com.polypaint.polypaint.Enum.AccessibilityTypes
import com.polypaint.polypaint.Model.Canevas
import com.polypaint.polypaint.R


class ImageListAdapter (var context: Context, var canevasList: List<Canevas>, var user: String) : androidx.recyclerview.widget.RecyclerView.Adapter<androidx.recyclerview.widget.RecyclerView.ViewHolder>(){

    companion object {
        private const val VIEW_TYPE_PRIVATE = 1
        private const val VIEW_TYPE_PUBLIC = 2
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): androidx.recyclerview.widget.RecyclerView.ViewHolder {
        val view: View
        view = LayoutInflater.from(parent.context).inflate(R.layout.item_drawing, parent, false)
        return if(viewType == VIEW_TYPE_PRIVATE){
            PrivateImageHolder(view)
        } else {
            PublicImageHolder(view)
        }

    }

    override fun getItemCount(): Int {
        return canevasList.size
    }

    override fun onBindViewHolder(holder: androidx.recyclerview.widget.RecyclerView.ViewHolder, position: Int) {
        val canevas = canevasList[position]
        when (holder.itemViewType) {
            ImageListAdapter.VIEW_TYPE_PRIVATE -> (holder as ImageListAdapter.PrivateImageHolder).bind(canevas)
            ImageListAdapter.VIEW_TYPE_PUBLIC -> (holder as ImageListAdapter.PublicImageHolder).bind(canevas)
        }
    }

    override fun getItemViewType(position: Int): Int {
        val canevas = canevasList[position]

        return if (canevas.accessibility == AccessibilityTypes.PRIVATE.value()) {
            ImageListAdapter.VIEW_TYPE_PRIVATE
        } else {
            ImageListAdapter.VIEW_TYPE_PUBLIC
        }
    }
    private inner class PrivateImageHolder internal constructor(itemView: View) : androidx.recyclerview.widget.RecyclerView.ViewHolder(itemView) {

        internal var nameText: TextView = itemView.findViewById(R.id.canevas_name_text) as TextView
        /*
        internal var timeText: TextView = itemView.findViewById(R.id.text_message_time) as TextView
        */

        internal fun bind(canevas: Canevas) {

            nameText.text = canevas.name
            //var rectangle: Drawable? = messageText.background
            //rectangle?.mutate()?.setColorFilter(R.color.colorOutlineSendMessage, PorterDuff.Mode.SRC_ATOP)
            //messageText.text = message.text
            //val formatter = SimpleDateFormat("HH:mm:ss")
            //timeText.text = formatter.format( message.createdAt)
            //DateUtils.formatDateTime(context, message.createdAt, DateUtils.FORMAT_SHOW_TIME)
        }

    }
    private inner class PublicImageHolder internal constructor(itemView: View) : androidx.recyclerview.widget.RecyclerView.ViewHolder(itemView) {

        internal var nameText: TextView = itemView.findViewById(R.id.canevas_name_text) as TextView
        internal var imageView: ImageView = itemView.findViewById(R.id.canevas_preview) as ImageView


        internal fun bind(canevas: Canevas) {
            if(canevas.owner == user){
                nameText.text = canevas.name + " owned by you"
            }else{
                nameText.text = canevas.name + " owned by " + canevas.owner
            }
            imageView.setImageResource(R.drawable.ic_delete)
            //var rectangle: Drawable? = messageText.background
            //rectangle?.mutate()?.setColorFilter(R.color.colorOutlineSendMessage, PorterDuff.Mode.SRC_ATOP)
            //messageText.text = message.text
            //val formatter = SimpleDateFormat("HH:mm:ss")
            //timeText.text = formatter.format( message.createdAt)
            //DateUtils.formatDateTime(context, message.createdAt, DateUtils.FORMAT_SHOW_TIME)
        }

    }
}

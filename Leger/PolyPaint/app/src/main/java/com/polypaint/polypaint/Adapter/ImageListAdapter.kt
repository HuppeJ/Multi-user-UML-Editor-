package com.polypaint.polypaint.Adapter

import android.content.Context
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import java.io.ByteArrayOutputStream
import android.util.Base64
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import com.polypaint.polypaint.Enum.AccessibilityTypes
import android.graphics.drawable.BitmapDrawable
import com.polypaint.polypaint.Holder.UserHolder
import com.polypaint.polypaint.Model.Canevas
import com.polypaint.polypaint.Model.Room
import com.polypaint.polypaint.R


class ImageListAdapter (var context: Context, var canevasList: List<Canevas>,  var listener: OnItemClickListener) : androidx.recyclerview.widget.RecyclerView.Adapter<androidx.recyclerview.widget.RecyclerView.ViewHolder>(){

    companion object {
        private const val VIEW_TYPE_PRIVATE = 1
        private const val VIEW_TYPE_PUBLIC = 2
    }

    interface OnItemClickListener {
        fun onItemClick(canevas: Canevas)
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
            ImageListAdapter.VIEW_TYPE_PRIVATE -> (holder as ImageListAdapter.PrivateImageHolder).bind(canevas, listener)
            ImageListAdapter.VIEW_TYPE_PUBLIC -> (holder as ImageListAdapter.PublicImageHolder).bind(canevas, listener)
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
        internal var lockImage: ImageView = itemView.findViewById(R.id.lock) as ImageView
        internal var imageView: ImageView = itemView.findViewById(R.id.canevas_preview) as ImageView
        /*
        internal var timeText: TextView = itemView.findViewById(R.id.text_message_time) as TextView
        */

        internal fun bind(canevas: Canevas, listener: OnItemClickListener) {
            itemView.setOnClickListener { listener.onItemClick(canevas) }
            nameText.text = canevas.name
            imageView.setImageResource(R.drawable.ic_picture)
            imageView.layoutParams.width = 100
            imageView.layoutParams.height = 100
            if(canevas.password != ""){
                lockImage.setImageResource(R.drawable.ic_padlock)
            }
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
        internal var lockImage: ImageView = itemView.findViewById(R.id.lock) as ImageView

        internal fun bind(canevas: Canevas, listener: OnItemClickListener) {
            itemView.setOnClickListener { listener.onItemClick(canevas) }
            if(canevas.owner == UserHolder.getInstance().username){
                nameText.text = canevas.name + " owned by you"
            } else {
                nameText.text = canevas.name + " owned by " + canevas.owner
            }


            if(canevas.thumbnailLeger != "" && canevas.thumbnailLeger != null) {
                imageView.setImageDrawable(getDrawableThumbnail(canevas.thumbnailLeger))
            } else {
                imageView.setImageResource(R.drawable.ic_picture)
            }

            if(canevas.password != ""){
                lockImage.setImageResource(R.drawable.ic_padlock)
            }
            //var rectangle: Drawable? = messageText.background
            //rectangle?.mutate()?.setColorFilter(R.color.colorOutlineSendMessage, PorterDuff.Mode.SRC_ATOP)
            //messageText.text = message.text
            //val formatter = SimpleDateFormat("HH:mm:ss")
            //timeText.text = formatter.format( message.createdAt)
            //DateUtils.formatDateTime(context, message.createdAt, DateUtils.FORMAT_SHOW_TIME)
        }

    }

    private fun getDrawableThumbnail(thumbnailString: String): BitmapDrawable {
        val bitmap: Bitmap? = stringToBitMap(thumbnailString)
        val resized = Bitmap.createScaledBitmap(bitmap, (bitmap!!.width!!.times(0.6)).toInt(), (bitmap!!.height!!.times(0.6)).toInt(), true);

        return bitmapToDrawable(resized!!)
    }

    private fun bitmapToDrawable(bitmap:Bitmap): BitmapDrawable {
        return BitmapDrawable(bitmap)
    }

    private fun stringToBitMap(encodedString: String): Bitmap? {
        try {
            val encodeByte = Base64.decode(encodedString, Base64.DEFAULT)
            return BitmapFactory.decodeByteArray(encodeByte, 0, encodeByte.size)
        } catch (e: Exception) {
            e.message
            return null
        }

    }
}

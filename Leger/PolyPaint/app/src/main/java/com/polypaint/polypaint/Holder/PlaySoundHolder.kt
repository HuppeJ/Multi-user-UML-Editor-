package com.polypaint.polypaint.Holder

import android.content.Context
import android.media.Ringtone
import android.media.RingtoneManager
import android.net.Uri

class PlaySoundHolder(){


    fun playNotification1(context: Context){
        try {
            var notification : Uri = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_NOTIFICATION)
            var r : Ringtone = RingtoneManager.getRingtone(context,notification)
            r.play()

        } catch (e : Exception) {
            e.printStackTrace();
        }
    }

    companion object {
        private val playSoundHolder: PlaySoundHolder = PlaySoundHolder()

        fun getInstance(): PlaySoundHolder{
            return playSoundHolder
        }

    }
}
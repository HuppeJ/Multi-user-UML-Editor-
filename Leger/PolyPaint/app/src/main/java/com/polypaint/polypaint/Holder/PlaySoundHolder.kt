package com.polypaint.polypaint.Holder

import android.content.Context
import android.database.Cursor
import android.media.Ringtone
import android.media.RingtoneManager
import android.net.Uri
import android.os.Handler
import android.util.Log

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

    fun playNotification2(context: Context){
        try {
            var alarm : Uri = RingtoneManager.getDefaultUri(RingtoneManager.TYPE_ALARM)
            var r : Ringtone = RingtoneManager.getRingtone(context, alarm)
            r.play()
            Handler().postDelayed({r.stop()},100)

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
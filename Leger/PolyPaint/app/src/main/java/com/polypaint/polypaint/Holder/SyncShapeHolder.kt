package com.polypaint.polypaint.Holder

import android.os.Handler
import com.polypaint.polypaint.Activity.DrawingActivity

class SyncShapeHolder{
    var drawingActivity : DrawingActivity? = null
    var saveHandler : Handler = Handler()
    val saveRunner = object : Runnable {
        override fun run() {
            saveHandler.removeCallbacks(this)
            if(drawingActivity != null){
                drawingActivity!!.saveCanevas()
            }

        }
    }
    fun saveCanevas(){
        saveHandler.removeCallbacks(saveRunner)
        saveHandler.post(saveRunner)
    }
    companion object {
        private val syncShapeHolder: SyncShapeHolder = SyncShapeHolder()

        fun getInstance(): SyncShapeHolder{
            return syncShapeHolder
        }

    }
}
package com.polypaint.polypaint.Holder

import com.polypaint.polypaint.Activity.DrawingActivity

class SyncShapeHolder{
    var drawingActivity : DrawingActivity? = null
    companion object {
        private val syncShapeHolder: SyncShapeHolder = SyncShapeHolder()

        fun getInstance(): SyncShapeHolder{
            return syncShapeHolder
        }

    }
}
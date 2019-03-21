package com.polypaint.polypaint.View

import android.content.Context
import android.graphics.Canvas
import android.view.View
import androidx.appcompat.app.AppCompatActivity
import com.polypaint.polypaint.Application.PolyPaint
import com.polypaint.polypaint.Holder.VFXHolder
import com.polypaint.polypaint.R
import kotlinx.android.synthetic.main.activity_drawing.view.*

class VfxView : View {
    constructor(context: Context) : super(context)

    override fun onAttachedToWindow() {
        super.onAttachedToWindow()
        val activity: AppCompatActivity = context as AppCompatActivity
        activity.layoutInflater.inflate(R.layout.view_vfx, null)
    }
    override fun onDraw(canvas: Canvas) {
        if (VFXHolder.getInstance().particleSystem != null && !VFXHolder.getInstance().particleSystem!!.isDead()) {
            VFXHolder.getInstance().particleSystem!!.update(canvas)
            VFXHolder.getInstance().animationHandler.removeCallbacks(VFXHolder.getInstance().animationRunner)
            VFXHolder.getInstance().animationHandler.postDelayed(VFXHolder.getInstance().animationRunner, VFXHolder.getInstance().ANIMATION_SPEED)
        }
        super.onDraw(canvas)
    }
}
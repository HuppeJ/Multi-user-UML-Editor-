package com.polypaint.polypaint.Model

import android.content.Context
import android.graphics.Canvas
import android.graphics.drawable.ShapeDrawable
import android.graphics.drawable.shapes.OvalShape
import android.view.View

class BasicShape(context: Context) : View(context) {
    private val mDrawable: ShapeDrawable = run {
        val x = 10
        val y = 10
        val width = 300
        val height = 50

        ShapeDrawable(OvalShape()).apply {
            // If the color isn't set, the shape uses black as the default.
            paint.color = 0xff74AC23.toInt()
            // If the bounds aren't set, the shape can't be drawn.
            setBounds(x, y, x + width, y + height)
        }
    }

    override fun onDraw(canvas: Canvas) {
        mDrawable.draw(canvas)
    }


}
package com.polypaint.polypaint.Particles

import android.content.Context
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.Paint

class DeleteParticle (override var xOrigin: Float, override var yOrigin:Float, override var maxSpeed :Float, override var context: Context): BasicParticle(xOrigin,yOrigin,maxSpeed,context) {
    private var mState: Int = ALIVE
    //private var mBitmap: Bitmap? = null
    private var mAge: Int = 0
    private var mLifetime: Int = 25
    private var mPaint: Paint = Paint()
    //private var mBase: Bitmap? = null
    private var size : Float = 5F
    override fun initialize() {
        //Make Bitmap based on drawable
        //mBase = BitmapFactory.decodeResource(context.resources, R.drawable.ic_plus)
        //set random scale
        //Make new bitmap with new width and height
        //mBitmap = Bitmap.createScaledBitmap(mBase,100,100,true)
        //mBitmap = mBase
        //Set Random direction
        mPaint.color = Color.parseColor("@color/colorRedGray")

        this.x = xOrigin
        this.y = yOrigin

        size = size()
        //Square splat
        vX = maxSpeed * initDirection()
        vY = maxSpeed * initDirection()

        //Circle splat
        /*
        if(vX*vX + vY*vY >= maxSpeed * maxSpeed){
            vX *= 0.7F
            vY *= 0.7F
        }

        */


    }
    //Splatter Effect
    override fun initDirection (): Float{
        return ((Math.random()*2)-1).toFloat()
    }

    private fun size() : Float {
        return (5+10*Math.random()).toFloat()
    }
    override fun draw(canvas: Canvas){
        canvas.drawRect(this.x,this.y,this.x+size,this.y+size, mPaint)
    }
}
package com.polypaint.polypaint.Particles

import android.content.Context
import android.graphics.Canvas
import android.util.Log
import android.view.View
import androidx.appcompat.app.AppCompatActivity
import com.polypaint.polypaint.Application.PolyPaint

class ParticleSystem(nParicles : Int,xOrigin: Float, yOrigin:Float,context: Context) {
    val ALIVE : Int = 0
    val DEAD : Int = 1

    var mState = ALIVE
    private val particles : ArrayList<BasicParticle> = ArrayList()

    init {
        mState = ALIVE
        for (i in 0..nParicles){
            particles.add(BasicParticle(xOrigin, yOrigin,10F,context))
        }
    }

    fun update(canvas: Canvas){
        if (mState != DEAD) {
            var isDead = true
            for (particle in particles) {
                if (particle.isAlive()) {
                    particle.update()
                    isDead = false
                }
            }
            if (isDead) {
                Log.d("anim","SystemDead")
                mState = DEAD
            }
        }
        draw(canvas)
    }
    fun draw(canvas: Canvas){
        for (particle in particles) {
            if (particle.isAlive()) {
                particle.draw(canvas)
            }
        }
    }

    fun isDead():Boolean{
        return mState == DEAD
    }
}
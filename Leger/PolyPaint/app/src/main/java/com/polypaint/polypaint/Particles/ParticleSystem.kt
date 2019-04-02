package com.polypaint.polypaint.Particles

import android.content.Context
import android.graphics.Canvas
import android.util.Log
import android.view.View
import androidx.appcompat.app.AppCompatActivity
import com.polypaint.polypaint.Application.PolyPaint

open class ParticleSystem(nParicles : Int,xOrigin: Float, yOrigin:Float,context: Context) {
    val ALIVE : Int = 0
    val DEAD : Int = 1

    var mState = ALIVE
    var particles : ArrayList<BasicParticle> = ArrayList()
    val nParicles = nParicles
    val xOrigin = xOrigin
    val yOrigin = yOrigin
    val context = context
    init {

    }
    open fun initialize() {
        mState = ALIVE
        for (i in 0..nParicles){
            particles.add(BasicParticle(xOrigin, yOrigin,10F,context))
            particles[i].initialize()
        }
    }

    open fun update(canvas: Canvas){
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
    open fun draw(canvas: Canvas){
        for (particle in particles) {
            if (particle.isAlive()) {
                particle.draw(canvas)
            }
        }
    }

    open fun isDead():Boolean{
        return mState == DEAD
    }
}
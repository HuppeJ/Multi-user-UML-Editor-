package com.polypaint.polypaint.Particles

import android.content.Context
import android.graphics.Canvas
import android.util.Log

class DeleteParticleSystem(nParicles : Int,xOrigin: Float, yOrigin:Float,context: Context):ParticleSystem(nParicles,xOrigin,yOrigin,context) {

    override fun initialize(){
        mState = ALIVE
        for (i in 0..nParicles){
            particles.add(DeleteParticle(xOrigin, yOrigin,25F,context))
            particles[i].initialize()
        }
    }

}
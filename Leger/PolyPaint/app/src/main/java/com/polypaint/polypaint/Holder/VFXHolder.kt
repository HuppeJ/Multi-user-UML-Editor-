package com.polypaint.polypaint.Holder

import android.content.Context
import android.os.Handler
import android.view.View
import com.polypaint.polypaint.Particles.DeleteParticleSystem
import com.polypaint.polypaint.Particles.ParticleSystem

class VFXHolder (){
    var particleSystem  : ParticleSystem? = null
    var animationHandler : Handler = Handler()
    var vfxView : View? = null
    val ANIMATION_SPEED : Long = 15
    val animationRunner = object : Runnable {
        override fun run() {
            animationHandler.removeCallbacks(this)
            vfxView!!.invalidate()
        }
    }

    fun fireVFX(x :Float,y : Float, context: Context){
        if(particleSystem == null || particleSystem!!.isDead()){
            particleSystem = ParticleSystem(50,x,y,context)
            particleSystem?.initialize()
            animationHandler.removeCallbacks(animationRunner)
            animationHandler.post(animationRunner)
        }
    }

    fun fireDeleteVFX(x :Float, y : Float, context: Context){
        if(particleSystem == null || particleSystem!!.isDead()){
            particleSystem = DeleteParticleSystem(20,x,y,context)
            particleSystem?.initialize()
            animationHandler.removeCallbacks(animationRunner)
            animationHandler.post(animationRunner)
        }
    }

    companion object {
        private val vfxHolder: VFXHolder = VFXHolder()

        fun getInstance(): VFXHolder{
            return vfxHolder
        }

    }
}